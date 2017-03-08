using Busey.Core.Command;
using Busey.Core.Event;
using System;

namespace Busey.Core.Bus
{
    public interface IBus
    {
        void Start();
        void Stop();
        void Init(IHost host);
        void Publish<T>(T @event) where T : IEvent;
        void Send<T>(T Command) where T : ICommand;
        void RegisterHandler<T>(Action<T> action, ushort prefetchCount = 1);
    }
}
