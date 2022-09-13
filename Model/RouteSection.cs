using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class RouteSection
    {
        public string Type { get; set; }
        public string name { get; set; }
        public string direction { get; set; }
        public string originationName { get; set; }
        public string destinationName { get; set; }
        public string originator { get; set; }
        public string destination { get; set; }
        public string serviceType { get; set; }
        public DateTime validTo { get; set; }
        public DateTime validFrom { get; set; }
    }
}
