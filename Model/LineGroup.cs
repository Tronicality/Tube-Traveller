using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
