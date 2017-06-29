using System.Collections.Generic;
using Newtonsoft.Json;

namespace BNetClassicChat_ClientAPI.Resources.Models
{
    internal class RequestResponseModel
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("request_id")]
        public int RequestId { get; set; }

        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; }

        [JsonProperty("status")]
        public StatusModel Status { get; set; }
    }
}
