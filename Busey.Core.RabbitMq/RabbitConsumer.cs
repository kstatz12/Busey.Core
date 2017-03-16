using Busey.Core.Messaging;
using Busey.Core.RabbitMq.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace Busey.Core.RabbitMq
{
    internal sealed class RabbitConsumer : IConsumer
    {

        public Tuple<string, Func<IModel, EventingBasicConsumer>> AdvancedConsume<T>(Action<T> action, Dictionary<string, object> args)
        {
            var queueName = args.GetAdvancedQueueName(typeof(T));
            Func<IModel, EventingBasicConsumer> consumer = (channel) =>
            {
                queueName.CreateQueue(channel);
                var exchange = args.ConfigureExchange(channel);
                var routingKey = args.ConfigureRoutingKey();
                RabbitMqHelper.BindQueue(queueName, exchange, routingKey, channel);
                var basicConsumer = new EventingBasicConsumer(channel);
                var prefetch = args.GetPrefetch();
                RabbitMqHelper.InitQos(channel, prefetch);
                basicConsumer.Received += (sender, arguments) =>
                {
                    var body = arguments.Body.Convert<T>();
                    action(body);
                    channel.BasicAck(arguments.DeliveryTag, false);
                };
                return basicConsumer;
            };
            return new Tuple<string, Func<IModel, EventingBasicConsumer>>(queueName, consumer);
        }


        public Tuple<string, Func<IModel, EventingBasicConsumer>> BasicConsume<T>(Action<T> action)
        {
            var queueName = typeof(T).ToQueue();
            Func<IModel, EventingBasicConsumer> consumer = (channel) =>
            {
                queueName.CreateQueue(channel);
                var basicConsumer = new EventingBasicConsumer(channel);
                RabbitMqHelper.InitQos(channel);
                basicConsumer.Received += (sender, args) => {
                    var body = args.Body.Convert<T>();
                    action(body);
                    channel.BasicAck(args.DeliveryTag, false);
                };
                return basicConsumer;
            };
            return new Tuple<string, Func<IModel, EventingBasicConsumer>>(queueName, consumer);
        }
    }
}
