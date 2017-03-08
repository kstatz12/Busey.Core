using RabbitMQ.Client;
using System;

namespace Busey.Core.RabbitMq
{
    internal static class QueueHelper
    {
        public static string ToQueue(this Type type)
        {
            return $"q.{type.Name}";
        }

        public static string CreateQueue(this string queue, IModel channel)
        {
            channel.QueueDeclare(queue, true, false, false, null);
            return queue;
        }
    }
}
