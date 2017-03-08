using Busey.Core.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Busey.Core.Event
{
    public interface IEventHandler<TEvent> : IHandler<TEvent> where TEvent : IEvent
    {

    }
}
