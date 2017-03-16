using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Busey.Core.RabbitMq.Interfaces
{
    public interface IEmitter
    {
        void AdvancedEmit<T>(Dictionary<string, object> args, T message, IModel channel, IConnection connection);
        void BasicEmit<T>(T message, IModel channel, IConnection connection);
    }
}
