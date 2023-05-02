using System.Collections.Generic;

namespace Tube_Traveller.Model
{
    internal class Root
    {
        //General
        public string? Id { internal get; set; }
        public string GetId() => Id!;

        //https://api.tfl.gov.uk/Line/{id}/StopPoints[?includeCrowdingData] - List<Root>
        public List<Line>? Lines { internal get; set; }
        public List<Line> GetLines() => Lines!;

        public string? CommonName { internal get; set; }
        public string GetCommonName() => CommonName!;

        public double Lat { internal get; set; }
        public double GetLat() => Lat;

        public double Lon { internal get; set; }
        public double GetLon() => Lon;

        //https://api.tfl.gov.uk/Line/{id}/Route/Sequence/{direction} - Root
        public List<OrderedLineRoute>? OrderedLineRoutes { internal get; set; }
        public List<OrderedLineRoute> GetOrderedLineRoutes() => OrderedLineRoutes!;

        //Journey - Root
        public List<Journey>? Journeys { internal get; set; }
        public List<Journey>? GetJourneys() => Journeys;
    }
}
