using Newtonsoft.Json;

namespace NTLauncher.Models
{
    public class NTPipeInPayload
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string JsonRPC { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public NTPipeInParams Params { get; set; }
    }

    public class NTPipeInParams
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }

    public class NTPipeOutPayload
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string JsonRPC { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }
    }
}
