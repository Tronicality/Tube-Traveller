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
        //General
        public string Type { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string modeName { get; set; }

        //Mode and Route
        public List<object> disruptions { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public List<object> lineStatuses { get; set; }
        public List<RouteSection> routeSections { get; set; }
        public List<ServiceType> serviceTypes { get; set; }
        public Crowding crowding { get; set; }

        //Line Arrivals
        public int operationType { get; set; }
        public string vehicleId { get; set; }
        public string naptanId { get; set; }
        public string stationName { get; set; }
        public string lineId { get; set; }
        public string lineName { get; set; }
        public string platformName { get; set; }
        public string direction { get; set; }
        public string bearing { get; set; }
        public string destinationNaptanId { get; set; }
        public string destinationName { get; set; }
        public DateTime timestamp { get; set; }
        public int timeToStation { get; set; }
        public string currentLocation { get; set; }
        public string towards { get; set; }
        public DateTime expectedArrival { get; set; }
        public DateTime timeToLive { get; set; }
        public Timing timing { get; set; }
    }
}
