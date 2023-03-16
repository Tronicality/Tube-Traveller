using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class RouteOption
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public List<string> Directions { get; set; }
        public LineIdentifier LineIdentifier { get; set; }
        public string Direction { get; set; }
    }
}
