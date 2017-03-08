using Buse.RabbitMq.Test.Contracts;
using Busey.Core.RabbitMq;

namespace Busey.RabbitMq.Test.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new RabbitMqHost("localhost", "guest", "guest");
            var bus = new RabbitMqBootstrapper().Init(host).Start().GetBus();

            bus.Send<TestCommand>(new TestCommand
            {
                Message = "Hello World"
            });
        }
    }
}