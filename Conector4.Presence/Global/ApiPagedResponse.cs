using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Global
{
    public class ApiPagedResponse
    {
        public string status { get; set; } = "";
        public int nextOffSet { get; set; }
        public List<object>? data { get; set; }
    }
}
