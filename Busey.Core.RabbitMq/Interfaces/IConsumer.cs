using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace Busey.Core.RabbitMq.Interfaces
{
    public interface IConsumer
    {
        Tuple<string, Func<IModel, EventingBasicConsumer>> AdvancedConsume<T>(Action<T> action, Dictionary<string, object> args);
        Tuple<string, Func<IModel, EventingBasicConsumer>> BasicConsume<T>(Action<T> action);
    }
}
