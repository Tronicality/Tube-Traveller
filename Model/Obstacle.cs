using Newtonsoft.Json;

namespace Tube_Traveller.Model
{
    internal class Obstacle
    {
        [JsonProperty("$type")]
        public string? Type { get; set; }

        //[JsonProperty("type")]
        //public string Type { get; set; }
        public string? Incline { get; set; }
        public int StopId { get; set; }
        public string? Position { get; set; }
    }
}
