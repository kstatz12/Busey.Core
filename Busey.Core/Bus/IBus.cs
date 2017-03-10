using Busey.Core.Command;
using Busey.Core.Event;
using System;
using System.Collections.Generic;

namespace Busey.Core.Bus
{
    public interface IBus
    {
        void Start();
        void Stop();
        void Init(IHost host);
        void Publish<T>(T @event, Dictionary<string, object> args = null) where T : IEvent;
        void Send<T>(T Command, Dictionary<string, object> args = null) where T : ICommand;
        void RegisterHandler<T>(Action<T> action, Dictionary<string, object> args = null);
    }
}
