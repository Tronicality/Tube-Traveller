using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Journey
    {
        //public string? Type { get; set; }
        //public DateTime StartDateTime { get; set; }
        public int Duration { get; set; }
        //public DateTime ArrivalDateTime { get; set; }
        //public List<Leg> Legs { get; set; }
        public Fare? Fare { get; set; }
    }
}
