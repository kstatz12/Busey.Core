using Buse.RabbitMq.Test.Contracts;
using Busey.Core.RabbitMq;
using System.Collections.Generic;

namespace Busey.RabbitMq.Test.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new RabbitMqHost("localhost", "guest", "guest", new Dictionary<string, object>
            {
                { "prefetch", 3 }
            });
            var bus = new RabbitMqBootstrapper().Init(host).Start();

            bus.Send<TestCommand>(new TestCommand
            {
                Message = "Hello World"
            });
        }
    }
}