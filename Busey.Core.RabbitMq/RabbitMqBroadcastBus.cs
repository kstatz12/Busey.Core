using Busey.Core.Broadcaster;
using Busey.Core.Messaging;
using Busey.Core.RabbitMq.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Busey.Core.RabbitMq
{
    public abstract class RabbitMqBroadcastBus : IBroadcastBus
    {
        private IConnection _connection;
        private IModel _channel;

        private readonly List<Tuple<string, Func<IModel, EventingBasicConsumer>>> _handlers;
        private readonly IEmitter _emitter;
        private readonly List<string> _queues;
        private string _exchange;
        private readonly List<Tuple<Type, int>> _quorums;
        private readonly List<Tuple<Type, object>> _responses;
        public RabbitMqBroadcastBus(IEmitter emitter)
        {
            _handlers = new List<Tuple<string, Func<IModel, EventingBasicConsumer>>>();
            _queues = new List<string>();
            _emitter = emitter;
            _quorums = new List<Tuple<Type, int>>();
            _responses = new List<Tuple<Type, object>>();
        }
        public void Init(IHost host)
        {
            var factory = new ConnectionFactory()
            {
                HostName = host.HostName,
                UserName = host.UserName,
                Password = host.Password,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        public void RegisterExchange(string exchange)
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false, null);
            _queues.ForEach(x =>
            {
                _channel.QueueBind(x, exchange, "");
            });
            _exchange = exchange;
        }
        public void RegisterQueue(string queue)
        {
            _channel.QueueDeclare(queue, true, false, false, null);
            _queues.Add(queue);
        }

        public void RegisterQuorum(Type type, int quorum)
        {
            _quorums.Add(new Tuple<Type, int>(type, quorum));
        }

        public void Start()
        {
            _handlers.ForEach(x =>
            {
                if (_connection.IsOpen)
                {
                    _channel.BasicConsume(x.Item1, false, x.Item2(_channel));
                }
            });
        }

        public void Stop()
        {
            _connection.Dispose();
            _channel.Dispose();
        }

        public void Publish<T>(T message)
        {

            var responseQueue = typeof(T).ToQueue();
            var body = message.ToMessage();
            var correlationId = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = responseQueue;
            props.CorrelationId = correlationId;

            _channel.BasicPublish(_exchange, "", props, body);
            Func<IModel, EventingBasicConsumer> consumer = (channel) =>
            {
                var basicConsumer = new EventingBasicConsumer(_channel);
                basicConsumer.Received += (sender, args) =>
                {
                    var quorum = _quorums.First(x => x.Item1 == typeof(T));
                    var response = args.Body.Convert<T>();
                
                    _responses.Add(new Tuple<Type, object>(typeof(T), response));
                    if(responses.Count >= qu)
                };
                return basicConsumer;
            };
            _handlers.Add(new Tuple<string, Func<IModel, EventingBasicConsumer>>(responseQueue, consumer));
        }

        public void RegisterAggregator<T>(Action<T> action)
        {

        }

        public void RegisterBroadcastHandler<T, TResult>(Func<T, TResult> action, string recieveQueue, Dictionary<string, object> arguments)
        {
            Func<IModel, EventingBasicConsumer> consumer = (channel) =>
            {
                var basicConsumer = new EventingBasicConsumer(channel);
                basicConsumer.Received += (sender, args) =>
                {
                    channel.BasicAck(args.DeliveryTag, false);
                    var body = args.Body.Convert<T>();
                    var response = action(body);
                    var props = channel.CreateBasicProperties();
                    props.CorrelationId = args.BasicProperties.CorrelationId;
                    var responseMessage = response.ToMessage<TResult>();
                    channel.BasicPublish("", args.BasicProperties.ReplyTo, props, responseMessage);
                };
                return basicConsumer;
            };
            _handlers.Add(new Tuple<string, Func<IModel, EventingBasicConsumer>>(recieveQueue, consumer));
        }
    }
}
