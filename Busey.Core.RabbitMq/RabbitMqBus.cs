using Busey.Core.Bus;
using System;
using Busey.Core.Command;
using Busey.Core.Event;
using RabbitMQ.Client;
using System.Collections.Generic;
using RabbitMQ.Client.Events;
using Busey.Core.Messaging;

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

        public void Publish<T>(T @event) where T : IEvent
        {
            var queueName = InitQueue(typeof(T));
            var body = @event.ToMessage();
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            _channel.BasicPublish("", queueName, false, properties, body); ;
        }

        public void RegisterHandler<T>(Action<T> action, ushort prefetchCount = 1)
        {
            string queueName = queueName = typeof(T).ToQueue().CreateQueue(_channel);
            Func<IModel, EventingBasicConsumer> consumer = (channel) =>
            {
                InitQos(channel, prefetchCount);
                var basicConsumer = new EventingBasicConsumer(channel);
                basicConsumer.Received += (sender, args) =>{
                    var body = args.Body.Convert<T>();
                    action(body);
                    channel.BasicAck(args.DeliveryTag, false);
                };
                return basicConsumer;
            };
            _handlers.Add(new Tuple<string, Func<IModel, EventingBasicConsumer>>(queueName, consumer));
        }

        public void Send<T>(T Command) where T : ICommand
        {
            var queueName = InitQueue(typeof(T));
            var body = Command.ToMessage();
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            _channel.BasicPublish("", queueName, false, properties, body);
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

        private void InitQos(IModel channel, ushort prefetch)
        {
            channel.BasicQos(0, prefetch, false);
        }
    }
}
