using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BNetClassicChat_API.Resources
{
    internal class RequestResponseModel
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("request_id")]
        public int RequestId { get; set; }

        [JsonProperty("payload")]
        public Dictionary<string, string> Payload { get; set; }

        [JsonProperty("status")]
        public StatusModel Status { get; set; }
    }
}
