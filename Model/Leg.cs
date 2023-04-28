using System;

namespace Tube_Traveller.Model
{
    internal class Leg
    {
        public DateTime DepartureTime { internal get; set; }
        public DateTime GetDepartureTime() => DepartureTime;
    }
}
