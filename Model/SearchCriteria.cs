using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class SearchCriteria
    {
        public string Type { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeType { get; set; }
        public TimeAdjustments TimeAdjustments { get; set; }
    }
}
