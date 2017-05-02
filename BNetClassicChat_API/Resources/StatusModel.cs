using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BNetClassicChat_API.Resources
{
    internal class StatusModel
    {
        [JsonProperty("area")]
        public int area { get; set; }

        [JsonProperty("code")]
        public int code { get; set; }
    }
}
