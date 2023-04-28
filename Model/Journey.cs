using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class Journey
    {
        public int Duration { internal get; set; }
        public int GetDuration() => Duration;

        public List<Leg>? Legs { internal get; set; }
        public List<Leg>? GetLegs() => Legs;

        public Fare? Fare { internal get; set; }
        public Fare? GetFare() => Fare;
    }
}
