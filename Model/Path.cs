using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class Path
    {
        public string? Type { get; set; }
        public string? LineString { get; set; }
        public List<StopPoint>? StopPoints { get; set; }
        public List<object>? Elevation { get; set; }
    }
}
