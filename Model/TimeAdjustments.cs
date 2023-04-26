namespace Tube_Traveller.Model
{
    internal class TimeAdjustments
    {
        public string? Type { get; set; }
        public Earliest? Earliest { get; set; }
        public Earlier? Earlier { get; set; }
        public Later? Later { get; set; }
        public Latest? Latest { get; set; }
    }
}
