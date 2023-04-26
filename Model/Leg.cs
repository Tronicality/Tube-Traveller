using System;
using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class Leg
    {
        public string? Type { get; set; }
        public int Duration { get; set; }
        public Instruction? Instruction { get; set; }
        public List<Obstacle>? Obstacles { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DeparturePoint? DeparturePoint { get; set; }
        public ArrivalPoint? ArrivalPoint { get; set; }
        public Path? Path { get; set; }
        public List<RouteOption>? RouteOptions { get; set; }
        public Mode? Mode { get; set; }
        public List<object>? Disruptions { get; set; }
        public List<object>? PlannedWorks { get; set; }
        public bool IsDisrupted { get; set; }
        public bool HasFixedLocations { get; set; }
        public DateTime ScheduledDepartureTime { get; set; }
        public DateTime ScheduledArrivalTime { get; set; }
        public string? InterChangeDuration { get; set; }
        public string? InterChangePosition { get; set; }
        public double? Distance { get; set; }
    }
}
