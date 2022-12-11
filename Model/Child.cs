using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    public class Child
    {
        public string? Type { get; set; }
        public string? NaptanId { get; set; }
        public List<object>? Modes { get; set; }
        public string? IcsCode { get; set; }
        public string? StationNaptan { get; set; }
        public string? HubNaptanCode { get; set; }
        public List<object>? Lines { get; set; }
        public List<object>? LineGroup { get; set; }
        public List<object>? LineModeGroups { get; set; }
        public bool Status { get; set; }
        public string? Id { get; set; }
        public string? CommonName { get; set; }
        public string? PlaceType { get; set; }
        public List<object>? AdditionalProperties { get; set; }
        public List<object>? Children { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
