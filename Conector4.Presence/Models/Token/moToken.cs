using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Models.Token
{
    public class moToken
    {

        [Newtonsoft.Json.JsonProperty("token")]
        public string token { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonProperty("access")]
        public string access { get; set; } = string.Empty;
    }
}
