using System;
using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class LineStatus
    {
        public string? Type { get; set; }
        public int Id { get; set; }
        public int StatusSeverity { get; set; }
        public string? StatusSeverityDescription { get; set; }
        public DateTime Created { get; set; }
        public List<object>? ValidityPeriods { get; set; }
    }
}
