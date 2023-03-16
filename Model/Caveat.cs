using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Caveat
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public string Text { get; set; }

        //[JsonProperty("type")]
        //public string Type { get; set; }
    }
}
