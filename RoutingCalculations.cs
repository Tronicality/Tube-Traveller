using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    internal class RoutingCalculations
    {
        TflClient _client;
        Dictionary<string, Root> _stations = new(); //key: stationCommonName, value: station
        Dictionary<string, List<Root>> _lines = new(); //key: lineId, value: station


        public RoutingCalculations(TflClient givenclient, Dictionary<string, Root> allStations, Dictionary<string, List<Root>> allLines)
        {
            _client = givenclient;
            _stations= allStations;
            _lines= allLines;
        }

        /// <summary>
        /// An approach of finding the route of stations 
        /// </summary>
        /// <param name="givenStation">The station that the user starts at</param>
        /// <param name="destinationStation">The station that the user ends at</param>
        /// <param name="stations">A list of stations where the user would start, end or switch to a different line</param>
        /// <param name="sameLine">Whether 2 lines are equal</param>
        /// <param name="preCheckedLineIds">The line IDs of lines that have already been checked or not to be included upon searching for a station that shares the same line as the destinationStation</param>
        /// <param name="iteration">How many times this method has called itself representing which line it is currently checking</param>
        /// <returns><paramref name="stations"/></returns>
        public List<Root> RouteByLines(Root givenStation, Root destinationStation, List<Root> stations, bool sameLine, List<string> preCheckedLineIds, int iteration) // problem if line isn't fully connected (for example overground)
        {
            //todo Find a better method to decide best route instead of lon & lat
            //todo Should rather return a list of routes rather than doing one route without full confirmation that it's the best (could do 0 for no new route found and 1 for new route found)
            //todo Priority system to include crowding or "dead" stations (through station/line status)
            //todo \Disabled (people) option

            iteration += 1;
            Root newStation = new();

            if (iteration == 1)
            {
                stations.Add(givenStation);
                stations.Add(destinationStation);
            }
            
            if (sameLine == false)
            {
                foreach (Line givenStationLine in givenStation.GetLines())
                {
                    if (givenStation.Id != null)
                    if (_lines.ContainsKey(givenStationLine.GetId()) && preCheckedLineIds.Contains(givenStation.Id) == false) //Checking for unwanted and pre-checked lines, (for example national rail lines)
                    {
                        foreach (Root currentStation in _lines[givenStationLine.GetId()]) //All stations on the givenStation line
                        {
                            sameLine = FindMatchingLine(currentStation, destinationStation);
                            newStation = FindSwitchStation(givenStation, destinationStation, newStation, currentStation, iteration);
                            AddStationToRoute(newStation, destinationStation, stations, iteration);
                        }
                        preCheckedLineIds.Add(givenStationLine.GetId()); //Adding to a list of lines so that the current line doesn't get checked again
                    }
                }

                if (sameLine == false) //Only used for when there's more than 1 change of lines
                {
                    stations = RouteByLines(newStation, destinationStation, stations, sameLine, preCheckedLineIds, iteration);
                }
            }
            return stations;
        }

        /// <summary>
        /// Comparing the lines of 2 stations. 
        /// </summary>
        /// <param name="chosenStation">The station that the user would match to</param>
        /// <param name="matchingStation">The station that the user would compare with</param>
        /// <returns>true if any line is the same, else false</returns>
        private static bool FindMatchingLine(Root chosenStation, Root matchingStation)
        {
            /* WIP, currently breaks things
            HashSet<Line> chosenLines = chosenStation.Lines.ToHashSet();
            HashSet<Line> matchingLines = matchingStation.Lines.ToHashSet();

            if (chosenLines.Overlaps(matchingLines))
            {
                return true;
            }

            return false;
            */
            return true;
        }

        
        private static void AddStationToRoute(Root newStation, Root destinationStation, List<Root> stations, int iteration)
        {
            //Debug.WriteLine($"From Routing: Matched {newStation.GetCommonName()}" + Environment.NewLine);
            if (stations.Count == 2) //First iteration
            {
                stations.RemoveAt(stations.Count - 1);
            }
            else if (stations.Count > iteration) //Used to mask considerations
            {
                stations.RemoveRange(stations.Count - 2, 2);
            }

            stations.Add(newStation);
            stations.Add(destinationStation);
        }


        /// <summary>
        /// In an attempt to find the optimal route by distance. It compares the distance between the station already considered to switch lines at with the competing station. Works given that the competing station is on the same line as the destination station
        /// </summary>
        /// <param name="givenStation">The station that the user starts at</param>
        /// <param name="destinationStation">The station that the user ends at</param>
        /// <param name="newStation">The current chosen "best" station to switch at</param>
        /// <param name="competingStation">The station in question of whether it's a better station to switch at</param>
        /// <returns>The Station you switch at being <paramref name="newStation"/></returns>
        private Root FindSwitchStation(Root givenStation, Root destinationStation, Root newStation, Root competingStation, int iteration)
        {
            double? currentStationDistance = CalculateDistance(competingStation, givenStation);
            double? newStationDistance = CalculateDistance(newStation, givenStation);

            if (givenStation.GetId() != competingStation.GetId() ) //Looking for new line from the current station
            {
                if (FindAmountOfValidLines(competingStation) > 0) //Checking for unwanted lines
                {
                    if (newStationDistance > currentStationDistance | newStationDistance == null) //null is used for first iteration, used to find the best station
                    {
                        //Debug.WriteLine($"Station Change where {currentStation.CommonName} is now the lowest with value {currentStationDistance}. Iteration - {iteration}");
                        newStation = _stations[competingStation.GetCommonName()];
                    }
                }
            }

            return newStation;
        }


        /// <summary>
        /// Finds amount of lines that are part of the mode "tube" for a given station
        /// </summary>
        /// <param name="chosenStation">The stations to find the amount of valid lines </param>
        /// <returns>An Integer</returns>
        private int FindAmountOfValidLines(Root chosenStation)
        {
            int differentTubeLines = -1;
            if (chosenStation.Lines != null)
            {
                for (int i = 0; i < chosenStation.Lines.Count; i++)
                {
                    if (chosenStation.Lines[i].Id != null)
                    if (_lines.ContainsKey(chosenStation.Lines[i].Id))
                    {
                        differentTubeLines++;
                    }
                }
            }
            

            return differentTubeLines;
        }

        /// <summary>
        /// Finds distance between the chosen station per iteration and destination station
        /// </summary>
        /// <param name="destinationStation">The station that the user ends at</param>
        /// <param name="chosenStation">The station that the user starts at</param>
        /// <returns>Distance if no null values are present, else null</returns>
        private static double? CalculateDistance(Root chosenStation, Root destinationStation)
        {
            if (chosenStation.GetCommonName() != null && destinationStation.GetCommonName() != null)
            {
                return Math.Sqrt(Math.Pow(destinationStation.Lon - chosenStation.Lon, 2) + Math.Pow(destinationStation.Lat - chosenStation.Lat, 2));
            }

            return null;
        }

        public async Task<List<Root>> RouteByDjikstra() //unfinished
        {
            //todo Make table of all stations with a distance of 1
            //todo Idea: await FastRouting_Unchecked() - used as check for whether station table has been created

            var allStations = await FindAllValidLines();
            return new List<Root>();
            // *use djisktra's algorithm to find shortest route
            //* return route
        }

        private async Task<Dictionary<string, List<OrderedLineRoute>>> FindAllValidLines()
        {
            var lines = await _client.GetAllLinesByModeAsync("tube");
            Dictionary<string, List<OrderedLineRoute>> orderedStations = new Dictionary<string, List<OrderedLineRoute>>();

            foreach (Root line in lines)
            {
                var lineRoute = await _client.GetLineRouteByLineAsync(line.GetId(), "inbound");
                orderedStations.Add(line.GetId(), lineRoute.GetOrderedLineRoutes());

                //ResultListBox.Items.Add(line.GetName());
            }
            return orderedStations;
        }
    }
}
