using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Fare2
    {
        public string Type { get; set; }
        public int LowZone { get; set; }
        public int HighZone { get; set; }
        public int Cost { get; set; }
        public string ChargeProfileName { get; set; }
        public bool IsHopperFare { get; set; }
        public string ChargeLevel { get; set; }
        public int Peak { get; set; }
        public int OffPeak { get; set; }
        public List<Tap> Taps { get; set; }
    }
}
