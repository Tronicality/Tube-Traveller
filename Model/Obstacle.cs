using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Obstacle
    {
        [JsonProperty("$type")]
        public string Type { get; set; }

        [JsonProperty("type")]
        //public string Type { get; set; }
        public string Incline { get; set; }
        public int StopId { get; set; }
        public string Position { get; set; }
    }
}
