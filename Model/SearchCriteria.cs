using System;

namespace Tube_Traveller.Model
{
    internal class SearchCriteria
    {
        public string? Type { get; set; }
        public DateTime DateTime { get; set; }
        public string? DateTimeType { get; set; }
        public TimeAdjustments? TimeAdjustments { get; set; }
    }
}
