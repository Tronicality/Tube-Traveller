using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Step
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string TurnDirection { get; set; }
        public string StreetName { get; set; }
        public int Distance { get; set; }
        public int CumulativeDistance { get; set; }
        public int SkyDirection { get; set; }
        public string SkyDirectionDescription { get; set; }
        public int CumulativeTravelTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PathAttribute PathAttribute { get; set; }
        public string DescriptionHeading { get; set; }
        public string TrackType { get; set; }
    }
}
