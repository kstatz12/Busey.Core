using System;
using System.Collections.Generic;

namespace Busey.Core.Broadcaster
{
    public interface IAggregator
    {
        T Aggregate<T>(Func<IEnumerable<T>, T> action);
    }
}
