using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
