using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS_parser.Config
{
    internal class Config
    {
    }

    internal class Proxy
    {
        [JsonProperty("proxyUri")]
        public string ProxyUri { get; set; }
        [JsonProperty("proxyLogin")]
        public string ProxyLogin { get; set; }
        [JsonProperty("proxyPassword")]
        public string ProxyPassword { get; set; }
    }
}
