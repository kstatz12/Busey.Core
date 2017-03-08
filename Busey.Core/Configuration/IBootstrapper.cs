using Busey.Core.Bus;
using Busey.Core.Command;
using Busey.Core.Event;

namespace Busey.Core.Configuration
{
    public interface IBootstrapper
    {
        IBootstrapper Init(IHost host);
        IBootstrapper WithCommandHandler<T>(ICommandHandler<T> handler) where T : ICommand;
        IBootstrapper WithEventHandler<T>(IEventHandler<T> handler) where T : IEvent;
        IBootstrapper Start();
        IBootstrapper Stop();
        IBus GetBus();
    }
}
