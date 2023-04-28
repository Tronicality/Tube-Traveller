using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class OrderedLineRoute
    {
        public string? Name { internal get; set; }
        public string? GetName() => Name;

        public List<string>? NaptanIds { internal get; set; }
        public List<string>? GetNaptanIds() => NaptanIds;

        public string? ServiceType { internal get; set; }
        public string? GetServiceType() => ServiceType;
    }
}
