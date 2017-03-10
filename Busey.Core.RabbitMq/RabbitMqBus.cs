using Busey.Core.Bus;
using System;
using Busey.Core.Command;
using Busey.Core.Event;
using RabbitMQ.Client;
using System.Collections.Generic;
using RabbitMQ.Client.Events;
using Busey.Core.Messaging;
using System.Linq;

namespace Busey.Core.RabbitMq
{
    public class RabbitMqBus : IBus
    {
        private IConnection _connection;
        private IModel _channel;

        private readonly List<Tuple<string, Func<IModel, EventingBasicConsumer>>> _handlers;
        private ConnectionFactory _factory;

        public RabbitMqBus()
        {
            _handlers = new List<Tuple<string, Func<IModel, EventingBasicConsumer>>>();
        }

        public void Init(IHost host)
        {
            _factory = new ConnectionFactory()
            {
                HostName = host.HostName,
                UserName = host.UserName,
                Password = host.Password
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }


        public void Publish<T>(T @event, Dictionary<string, object> args = null) where T : IEvent
        {
            if (args == null)
            {
                BasicEmit<T>(@event, _channel);
            }
            else
            {
                AdvancedEmit<T>(args, @event, _channel);
            }
        }

        public void Send<T>(T command, Dictionary<string, object> args = null) where T : ICommand
        {
            if(args == null)
            {
                BasicEmit<T>(command, _channel);
            }
            else
            {
                AdvancedEmit<T>(args, command, _channel);
            }
        }

        public void RegisterHandler<T>(Action<T> action, Dictionary<string, object> args = null)
        {
            if(args == null)
            {
                BasicConsume<T>(action);
            }
            else
            {
                AdvancedConsume<T>(action, args);
            }
        }

        public void Start()
        {
            _handlers.ForEach(x =>
            {
                if (_connection.IsOpen){
                    _channel.BasicConsume(x.Item1, true, x.Item2(_channel));
                }
            });
        }

        public void Stop()
        {
            _connection.Dispose();
            _channel.Dispose();
        }

        private string InitQueue(Type input)
        {
            return input.ToQueue().CreateQueue(_channel);
        }

        private void InitQos(IModel channel, ushort prefetch = 1, bool global = false)
        {
            channel.BasicQos(0, prefetch, global);
        }

        private void AdvancedEmit<T>(Dictionary<string, object> args, T message, IModel channel)
        {
            var exchange = ConfigureExchange(args, channel);
            var routingKey = ConfigureRoutingKey(args);
            var body = message.ToMessage();
            var properties = GetBasicProperties();
            channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: properties, body: body);
        }

        private void BasicEmit<T>(T message, IModel channel)
        {
            var queueName = typeof(T).ToQueue().CreateQueue(channel);
            var body = message.ToMessage();
            var properties = GetBasicProperties();
            if (_connection.IsOpen)
            {
                _channel.BasicPublish("", queueName, false, properties, body);
            }
        }

        private void AdvancedConsume<T>(Action<T> action, Dictionary<string, object> args)
        {
            var queueName = typeof(T).ToQueue();
            Func<IModel, EventingBasicConsumer> consumer = (channel) =>
            {
                queueName.CreateQueue(channel);
                var exchange = ConfigureExchange(args, channel);
                var routingKey = ConfigureRoutingKey(args);
                BindQueue(queueName, exchange, routingKey, channel);
                var basicConsumer = new EventingBasicConsumer(channel);
                var prefetch = GetPrefetch(args);
                InitQos(channel, prefetch);
                basicConsumer.Received += (sender, arguments) =>
                {
                    var body = arguments.Body.Convert<T>();
                    action(body);
                    channel.BasicAck(arguments.DeliveryTag, false);
                };
                return basicConsumer;
            };
            _handlers.Add(new Tuple<string, Func<IModel, EventingBasicConsumer>>(queueName, consumer));
        }

        private void BasicConsume<T>(Action<T> action)
        {
            string queueName = queueName = typeof(T).ToQueue();
            Func<IModel, EventingBasicConsumer> consumer = (channel) =>
            {
                queueName.CreateQueue(channel);
                var basicConsumer = new EventingBasicConsumer(channel);
                InitQos(channel);
                basicConsumer.Received += (sender, args) => {
                    var body = args.Body.Convert<T>();
                    action(body);
                    channel.BasicAck(args.DeliveryTag, false);
                };
                return basicConsumer;
            };
            _handlers.Add(new Tuple<string, Func<IModel, EventingBasicConsumer>>(queueName, consumer));
        }
        private string ConfigureExchange(Dictionary<string, object> args, IModel channel)
        {
            var exchangeName = args.Where(x => x.Key.ToLower().Equals("exchangename")).Select(x=>x.Value).FirstOrDefault().ToString();
            var exchangeType = args.Where(x => x.Key.ToLower().Equals("exchangetype")).Select(x => x.Value).FirstOrDefault().ToString();

            if(!string.IsNullOrEmpty(exchangeName) && !string.IsNullOrEmpty(exchangeType))
            channel.ExchangeDeclare(exchangeName, exchangeType, durable: true);

            return exchangeName;
        }

        private string ConfigureRoutingKey(Dictionary<string, object> args)
        {
            return args.Where(x => x.Key.ToLower().Equals("routingkey")).Select(x => x.Value).FirstOrDefault().ToString();
        }
        
        private IBasicProperties GetBasicProperties()
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }
        
        private void BindQueue(string queue, string exchange, string routingKey, IModel channel)
        {
            channel.QueueBind(queue, exchange, routingKey);
        }

        private ushort GetPrefetch(Dictionary<string, object> args)
        {
            var prefetchRaw = args.Where(x => x.Key.ToLower().Equals("prefetch"))
                                       .Select(x => x.Value)
                                       .FirstOrDefault();
            if (!ushort.TryParse(prefetchRaw.ToString(), out ushort prefetch))
            {
                prefetch = 1;
            };
            return prefetch;
        }

    }
}
