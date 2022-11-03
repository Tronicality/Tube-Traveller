using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    public class LineModeGroup
    {
        public string? Type { get; set; }
        public string? ModeName { get; set; }
        public List<string>? LineIdentifier { get; set; }
    }
}
