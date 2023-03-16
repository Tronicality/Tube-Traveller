using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class TapDetails
    {
        public string Type { get; set; }
        public string ModeType { get; set; }
        public string ValidationType { get; set; }
        public string HostDeviceType { get; set; }
        public int NationalLocationCode { get; set; }
        public DateTime TapTimestamp { get; set; }
    }
}
