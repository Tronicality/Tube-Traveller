using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class OrderedLineRoute
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public List<string>? NaptanIds { get; set; }
        public string? ServiceType { get; set; }
    }
}
