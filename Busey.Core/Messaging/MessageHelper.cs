using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Busey.Core.Messaging
{
    public static class MessageHelper
    {
        public static T Convert<T>(this byte[] input)
        {
            var json = Encoding.UTF8.GetString(input);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static byte[] ToMessage<T>(this T input)
        {
            var json = JsonConvert.SerializeObject(input);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
