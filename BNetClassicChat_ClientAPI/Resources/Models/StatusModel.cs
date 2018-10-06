using BNetClassicChat_ClientAPI.Resources.EArgs;
using Newtonsoft.Json;

namespace BNetClassicChat_ClientAPI.Resources.Models
{
    internal class StatusModel
    {
        [JsonProperty("area")]
        public ErrorArgs.AreaCode Area { get; set; }

        [JsonProperty("code")]
        public ErrorArgs.ErrorCode Code { get; set; }
    }
}
