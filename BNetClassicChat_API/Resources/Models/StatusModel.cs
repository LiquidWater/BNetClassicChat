using Newtonsoft.Json;
using static BNetClassicChat_API.Resources.Constants;

namespace BNetClassicChat_API.Resources.Models
{
    internal class StatusModel
    {
        [JsonProperty("area")]
        public AreaCode Area { get; set; }

        [JsonProperty("code")]
        public ErrorCode Code { get; set; }
    }
}
