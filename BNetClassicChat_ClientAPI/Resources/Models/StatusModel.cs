using Newtonsoft.Json;
using static BNetClassicChat_ClientAPI.Resources.Constants;

namespace BNetClassicChat_ClientAPI.Resources.Models
{
    internal class StatusModel
    {
        [JsonProperty("area")]
        public AreaCode Area { get; set; }

        [JsonProperty("code")]
        public ErrorCode Code { get; set; }
    }
}
