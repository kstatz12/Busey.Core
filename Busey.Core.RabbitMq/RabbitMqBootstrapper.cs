using Busey.Core.Configuration;
using Busey.Core.Bus;
using Busey.Core.Command;
using Busey.Core.Event;

namespace Busey.Core.RabbitMq
{
    public class RabbitMqBootstrapper : IBootstrapper
    {
        private IBus _bus;

        public IBootstrapper Init(IHost host)
        {
            var rabbitEmitter = new RabbitEmitter();
            var rabbitConsumer = new RabbitConsumer();
            _bus = new RabbitMqBus(rabbitEmitter, rabbitConsumer);
            _bus.Init(host);
            return this;
        }

        public IBus Start()
        {
            _bus.Start();
            return _bus;
        }

        public IBootstrapper Stop()
        {
            _bus.Dispose();
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
