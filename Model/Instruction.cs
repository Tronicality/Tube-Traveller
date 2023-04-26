using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class Instruction
    {
        public string? Type { get; set; }
        public string? Summary { get; set; }
        public string? Detailed { get; set; }
        public List<Step>? Steps { get; set; }   
    }
}
