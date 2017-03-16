using System;
using System.Collections.Generic;

namespace Busey.Core.Broadcaster
{
    public interface IBroadcastBus
    {
        void Init(IHost host);
        void Start();
        void Stop();
        void Publish<T>(T message, Dictionary<string, object> arguments = null);
        void RegisterBroadcastHandler<T, Tresult>(Func<T, Tresult> action, Dictionary<string, object> arguments = null);
        void RegisterAggregator<T>(Func<IEnumerable<T>, T> aggregator, Dictionary<string, object> arguments = null);
    }
}
