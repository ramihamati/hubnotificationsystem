using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Digitteck.HubNotificationSystem
{
    public class RedisExchangeModel
    {
        [JsonProperty("Properties")]
        [JsonPropertyName("Properties")]
        public Dictionary<string, string> Properties { get; set; }

        [JsonProperty("Body")]
        [JsonPropertyName("Body")]
        public byte[] Body { get; set; }
    }
}
