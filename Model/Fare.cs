using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Fare
    {
        public string Type { get; set; }
        public int TotalCost { get; set; }
        public List<Fare> Fares { get; set; }
        public List<Caveat> Caveats { get; set; }
    }
}
