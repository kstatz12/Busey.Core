namespace Busey.Core.RabbitMq
{
    public class RabbitMqHost : IHost
    {
        public RabbitMqHost(string hostName, string userName, string password)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }
        public string HostName { get; }

        public string UserName { get; }

        public string Password { get; }
    }
}
