using System;
using System.Collections.Generic;

namespace Busey.Core.RabbitMq
{
    public class RabbitMqHost : IHost
    {
        public RabbitMqHost(string hostName, string userName, string password, Dictionary<string, object> args)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
            Args = args;
        }
        public string HostName { get; }

        public string UserName { get; }

        public string Password { get; }

        public Dictionary<string, object> Args { get; }
    }
}
