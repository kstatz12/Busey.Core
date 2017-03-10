using System;
using System.Collections.Generic;
using System.Text;

namespace Busey.Core
{
    public interface IHost
    {
        string HostName { get; }
        string UserName { get; }
        string Password { get; }
        Dictionary<string, object> Args { get; }
    }
}
