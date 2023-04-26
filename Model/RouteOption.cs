using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class RouteOption
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public List<string>? Directions { get; set; }
        public LineIdentifier? LineIdentifier { get; set; }
        public string? Direction { get; set; }
    }
}
