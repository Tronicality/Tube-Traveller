using Newtonsoft.Json;

namespace Tube_Traveller.Model
{
    internal class Caveat
    {
        [JsonProperty("$type")]
        public string? Type { get; set; }
        public string? Text { get; set; }

        //[JsonProperty("type")]
        //public string Type { get; set; }
    }
}
