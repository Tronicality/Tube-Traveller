using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Tap
    {
        public string Type { get; set; }
        public string AtcoCode { get; set; }
        public TapDetails TapDetails { get; set; }
    }
}
