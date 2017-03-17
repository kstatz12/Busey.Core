using Busey.Core.Bus;
using System;
using Busey.Core.Command;
using Busey.Core.Event;
using RabbitMQ.Client;
using System.Collections.Generic;
using RabbitMQ.Client.Events;
using Busey.Core.RabbitMq.Interfaces;

namespace Busey.Core.RabbitMq
{
    public class RabbitMqBus : IBus
    {
        private IConnection _connection;
        private IModel _channel;

        private readonly List<Tuple<string, Func<IModel, EventingBasicConsumer>>> _handlers;
        private ConnectionFactory _factory;
        private readonly IEmitter _emitter;
        private readonly IConsumer _consumer;

        public RabbitMqBus(IEmitter emitter, IConsumer consumer)
        {
            _handlers = new List<Tuple<string, Func<IModel, EventingBasicConsumer>>>();
            _emitter = emitter ;
            _consumer = consumer;
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
                _emitter.BasicEmit(@event, _channel, _connection);
            }
            else
            {
                _emitter.AdvancedEmit(args, @event, _channel, _connection);
            }
        }

        public void Send<T>(T command, Dictionary<string, object> args = null) where T : ICommand
        {
            if(args == null)
            {
                _emitter.BasicEmit(command, _channel, _connection);
            }
            else
            {
                _emitter.AdvancedEmit(args, command, _channel, _connection);
            }
        }

        public void RegisterHandler<T>(Action<T> action, Dictionary<string, object> args = null)
        {
            var handler = args == null ? _consumer.BasicConsume(action) : _consumer.AdvancedConsume(action, args);
            _handlers.Add(handler);
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

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}
