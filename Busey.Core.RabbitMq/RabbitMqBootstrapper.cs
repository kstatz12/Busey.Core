using Busey.Core.Configuration;
using Busey.Core.Bus;
using Busey.Core.Command;
using Busey.Core.Event;

namespace Busey.Core.RabbitMq
{
    public class RabbitMqBootstrapper : IBootstrapper
    {
        private IBus _bus;

        public IBus GetBus()
        {
            return _bus;
        }

        public IBootstrapper Init(IHost host)
        {
            _bus = new RabbitMqBus();
            _bus.Init(host);
            return this;
        }

        public IBootstrapper Start()
        {
            _bus.Start();
            return this;
        }

        public IBootstrapper Stop()
        {
            _bus.Stop();
            return this;
        }

        public IBootstrapper WithCommandHandler<T>(ICommandHandler<T> handler) where T : ICommand
        {
            _bus.RegisterHandler<T>(handler.Handle);
            return this;
        }


        public IBootstrapper WithEventHandler<T>(IEventHandler<T> handler) where T : IEvent
        {
            _bus.RegisterHandler<T>(handler.Handle);
            return this;
        }
    }
}
