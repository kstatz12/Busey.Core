using Busey.Core.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buse.RabbitMq.Test.Contracts
{
    public class TestCommand : ICommand
    {
        public string Message { get; set; }
    }
}
