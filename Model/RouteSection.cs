using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class RouteSection
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Direction { get; set; }
        public string? OriginationName { get; set; }
        public string? DestinationName { get; set; }
        public string? Originator { get; set; }
        public string? Destination { get; set; }
        public string? ServiceType { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime ValidFrom { get; set; }
    }
}
