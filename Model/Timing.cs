using System;

namespace Tube_Traveller.Model
{
    internal class Timing
    {
        public string? Type { get; set; }
        public string? CountdownServerAdjustment { get; set; }
        public DateTime Source { get; set; }
        public DateTime Insert { get; set; }
        public DateTime Read { get; set; }
        public DateTime Sent { get; set; }
        public DateTime Received { get; set; }
    }
}
