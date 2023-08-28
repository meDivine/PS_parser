using Newtonsoft.Json;

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
