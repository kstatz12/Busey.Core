using System;
using System.Collections.Generic;

namespace Busey.Core.Broadcaster
{
    public interface IBroadcastBus
    {
        void Init(IHost host);
        void Start();
        void Stop();
        void RegisterExchange(string exchange);
        void RegisterQueue(string queue);
        void RegisterQuorum(Type type, int quorum);
        void Publish<T>(T message, Action<IEnumerable<T>> aggregator);
        void RegisterBroadcastHandler<T, TResult>(Func<T, TResult> action, string recieveQueue);
    }
}
