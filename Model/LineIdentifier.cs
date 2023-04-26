using Newtonsoft.Json;

namespace Tube_Traveller.Model
{
    internal class LineIdentifier
    {
        [JsonProperty("$type")]
        public string? Type { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Uri { get; set; }

        //[JsonProperty("type")]
        //public string Type { get; set; }
        public string? Crowding { get; set; } //Was meant to be type crowding?
        public string? RouteType { get; set; }
        public string? Status { get; set; }
    }
}
