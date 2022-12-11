using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    public class LineGroup
    {
        public string? Type { get; set; }
        public string? StationAtcoCode { get; set; }
        public List<string>? LineIdentifier { get; set; }
        public string? NaptanIdReference { get; set; }
    }
}
