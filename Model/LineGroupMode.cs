using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    public class LineModeGroup
    {
        public string? Type { get; set; }
        public string? ModeName { internal get; set; }
        public string GetModeName() => ModeName!;
        public List<string>? LineIdentifier { get; set; }
    }
}
