using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Busey.Core.RabbitMq
{
    public static class RabbitMqHelper
    {
        public static string ConfigureExchange(this Dictionary<string, object> args, IModel channel)
        {
            var exchangeName = args.Where(x => x.Key.ToLower().Equals("exchangename")).Select(x => x.Value).FirstOrDefault().ToString();
            var exchangeType = args.Where(x => x.Key.ToLower().Equals("exchangetype")).Select(x => x.Value).FirstOrDefault().ToString();

            if (!string.IsNullOrEmpty(exchangeName) && !string.IsNullOrEmpty(exchangeType))
                channel.ExchangeDeclare(exchangeName, exchangeType, durable: true);

            return exchangeName;
        }

        public static string ConfigureRoutingKey(this Dictionary<string, object> args)
        {
            return args.Where(x => x.Key.ToLower().Equals("routingkey")).Select(x => x.Value).FirstOrDefault().ToString();
        }

        public static void InitQos(IModel channel, ushort prefetch = 1, bool global = false)
        {
            channel.BasicQos(0, prefetch, global);
        }

        public static IBasicProperties GetBasicProperties(IModel channel)
        {
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }

        public static void BindQueue(string queue, string exchange, string routingKey, IModel channel)
        {
            channel.QueueBind(queue, exchange, routingKey);
        }

        public static ushort GetPrefetch(this Dictionary<string, object> args)
        {
            var prefetchRaw = args.Where(x => x.Key.ToLower().Equals("prefetch"))
                                       .Select(x => x.Value)
                                       .FirstOrDefault();
            if (!ushort.TryParse(prefetchRaw.ToString(), out ushort prefetch))
            {
                prefetch = 1;
            }
            return prefetch;
        }

        public static string GetAdvancedQueueName(this Dictionary<string, object> args, Type t)
        {
            var queue =
                args.Where(x => x.Key.ToLower().Equals("queuename")).Select(x => x.Value).FirstOrDefault().ToString();
            return string.IsNullOrEmpty(queue) ? t.ToQueue() : queue;
        }
    }
}
