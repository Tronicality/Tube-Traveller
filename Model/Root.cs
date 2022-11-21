using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tube_Traveller.Model
{
    internal class Root
    {
        [JsonProperty("$type")]

        //General - List<Root>
        public string? Type { get; set; }
        public string? Id {internal get; set; }
        public string GetId() => Id!;
        public string? ModeName {internal get; set; }
        public string GetModeName() => ModeName!;

        //https://api.tfl.gov.uk/Line/Meta/Modes - List<Root>
        public bool IsTflService { get; set; }
        public bool IsFarePaying { get; set; }
        public bool IsScheduledService { get; set; }
        //ModeName

        //https://api.tfl.gov.uk/Line/Mode/{modes}/Route[?serviceTypes] - List<Root>
        public string? Name { internal get; set; }
        public string GetName() => Name!;

        public List<object>? Disruptions { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public List<object>? LineStatuses { get; set; }
        public List<RouteSection>? RouteSections { get; set; }
        public List<ServiceType>? ServiceTypes { get; set; }
        public Crowding? Crowding { get; set; }

        //https://api.tfl.gov.uk/Line/{id}/Arrivals - List<Root>
        public int OperationType { get; set; }
        public string? VehicleId { get; set; }
        public string? NaptanId { get; set; }
        public string? StationName { get; set; }
        public string? LineId { get; set; }
        public string? LineName { get; set; }
        public string? PlatformName { get; set; }
        public string? Direction { get; set; }
        public string? Bearing { get; set; }
        public string? DestinationNaptanId { get; set; }
        public string? DestinationName { get; set; }
        public DateTime Timestamp { get; set; }
        public int TimeToStation { get; set; }
        public string? CurrentLocation { get; set; }
        public string? Towards { get; set; }
        public DateTime ExpectedArrival { get; set; }
        public DateTime TimeToLive { get; set; }
        public Timing? Timing { get; set; }

        //https://api.tfl.gov.uk/Line/{id}/StopPoints[?includeCrowdingData] - Root
        //public string? NaptanId { get; set; }
        public List<string>? Modes { get; set; }
        public string? IcsCode { get; set; }
        public string? StopType { get; set; }
        public string? StationNaptan { get; set; }
        public string? HubNaptanCode { get; set; }
        public List<Line>? Lines { get; set; }
        public List<LineGroup>? LineGroup { get; set; }
        public List<LineModeGroup>? LineModeGroups { get; set; }
        public bool Status { get; set; }
        public string? CommonName { get; set; }
        public string? PlaceType { get; set; }
        public List<AdditionalProperty>? AdditionalProperties { get; set; }
        public List<Child>? Children { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        //https://api.tfl.gov.uk/Line/{id}/Route/Sequence/{direction} - Root
        //public string LineId { get; set; }
        //public string LineName { get; set; }
        //public string Direction { get; set; }
        public bool IsOutboundOnly { get; set; }
        public string? Mode { get; set; }
        public List<string>? LineStrings { get; set; }
        public List<Station>? Stations { get; set; }
        //public List<StopPointSequence> StopPointSequences { get; set; }
        public List<OrderedLineRoute>? OrderedLineRoutes {internal get; set; }
        public List<OrderedLineRoute> GetOrderedLineRoutes() => OrderedLineRoutes!;
    }
}
