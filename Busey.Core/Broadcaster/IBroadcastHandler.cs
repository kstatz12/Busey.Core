using Busey.Core.Shared;

namespace Busey.Core.Broadcaster
{
    public interface IBroadcastHandler<T, TResult>
    {
        TResult Handle(T message);
    }
}
