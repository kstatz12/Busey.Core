using Buse.RabbitMq.Test.Contracts;
using Busey.Core.Command;
using Busey.Core.RabbitMq;
using System;

namespace Busety.RabbitMq.Test.Reviever
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new RabbitMqHost("localhost", "guets", "guest", new System.Collections.Generic.Dictionary<string, object>());
            new RabbitMqBootstrapper().Init(host).WithCommandHandler(new TestCommandHandler()).Start();
        }
    }

    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public void Handle(TestCommand input)
        {
            Console.WriteLine(input.Message);
        }
    }
}