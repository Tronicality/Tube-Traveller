using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class DeparturePoint
    {
        public string Type { get; set; }
        public string NaptanId { get; set; }
        public string PlatformName { get; set; }
        public string IcsCode { get; set; }
        public string IndividualStopId { get; set; }
        public string CommonName { get; set; }
        public string PlaceType { get; set; }
        public List<object> AdditionalProperties { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
