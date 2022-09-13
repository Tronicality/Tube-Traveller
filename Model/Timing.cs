using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Timing
    {
        public string Type { get; set; }
        public string countdownServerAdjustment { get; set; }
        public DateTime source { get; set; }
        public DateTime insert { get; set; }
        public DateTime read { get; set; }
        public DateTime sent { get; set; }
        public DateTime received { get; set; }
    }
}
