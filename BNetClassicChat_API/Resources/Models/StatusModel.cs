﻿using Newtonsoft.Json;

namespace BNetClassicChat_API.Resources.Models
{
    internal class StatusModel
    {
        [JsonProperty("area")]
        public int Area { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }
}