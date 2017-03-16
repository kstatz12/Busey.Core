using Busey.Core.Messaging;
using Busey.Core.RabbitMq.Interfaces;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace Busey.Core.RabbitMq
{
    internal sealed class RabbitEmitter : IEmitter
    {
        public void AdvancedEmit<T>(Dictionary<string, object> args, T message, IModel channel, IConnection connection)
        {
            var exchange = args.ConfigureExchange(channel);
            var routingKey = args.ConfigureRoutingKey();
            var body = message.ToMessage();
            var properties = RabbitMqHelper.GetBasicProperties(channel);
            if (connection.IsOpen)
            {
                channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: properties, body: body);
            }
        }

        public void BasicEmit<T>(T message, IModel channel, IConnection connection)
        {
            var queueName = typeof(T).ToQueue().CreateQueue(channel);
            var body = message.ToMessage();
            var properties = RabbitMqHelper.GetBasicProperties(channel);
            if (connection.IsOpen)
            {
                channel.BasicPublish("", queueName, false, properties, body);
            }
        }
    }
}
