﻿/*
    RequestResponseModel.cs: Modelling for the primary JSON messages

    Copyright (C) 2018 LiquidWater
    https://github.com/Liquidwater

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Newtonsoft.Json;
using System.Collections.Generic;

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
