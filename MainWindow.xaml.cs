using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    public partial class MainWindow : Window
    {
        TflClient _client;
        Dictionary<string, Root> _stations = new(); //stationCommonName, station
        Dictionary<string, List<Root>> _lines = new(); //lineId, station

        public MainWindow()
        {
            _client = new TflClient();
            LoadStations();
            InitializeComponent();
        }

        private async void LoadStations()
        {
            /* Getting stations through Zip-File
            Stream stream = await _client.GetDetailedStationDataStreamAsync(); //Gives Zip file as a stream from memory

            using (ZipArchive archive = new ZipArchive(stream)) //Setting archive to the whole zip file
            {
                ZipArchiveEntry? zipArchiveEntry = archive.Entries.FirstOrDefault(x => x.Name == "Stations.csv"); //Finding the stations excel file within the zip file

                //List<string> listUniqueId = new List<string>();
                //List<string> listName = new List<string>();

                if (zipArchiveEntry != null)
                {
                    using (var reader = new StreamReader(zipArchiveEntry.Open())) // Opening stream to stations excel file
                    {
                        var content = await reader.ReadToEndAsync();
                        var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries); // Turning whole file into an array of rows and removing any empty rows

                        for (int i = 1; i < lines.Length; i++)
                        {
                            var column = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries); // Turning array of rows into rows and columns also removing any spaces added to end of words
                            //listUniqueId.Add(column[0]);
                            //listName.Add(column[1]);
                            FromComboBox.Items.Add(column[1]);
                            ToComboBox.Items.Add(column[1]);
                        }
                    }
                }
            */

            //Getting stations from api 
            
            try
            {
                //List<Root> modes = await _client.GetAllModesAsync();
                List<string> tempStations = new();

                //Removed lines that isn't tube as I didn't consider it being part of the underground (for example overground)

                List<Root> lines = await _client.GetAllLinesByModeAsync("tube");
                foreach (Root line in lines)
                {
                    List<Root> stations = await _client.GetAllStationsByLine(line.GetId());

                    foreach (Root station in stations)
                    {
                        if (!_stations.ContainsKey(station.GetCommonName())) //No repeating stations appear
                        {
                            _stations.Add(station.GetCommonName(), station);
                            tempStations.Add(station.GetCommonName());
                        }
                    }

                    _lines.Add(line.GetId(), stations);
                }

                tempStations.Sort();
                FromComboBox.ItemsSource = tempStations;
                ToComboBox.ItemsSource = tempStations;

                //Should probably check statuses per station only if it's bad

                //Idea 1, make the user wait but put something to entertain
                //Idea 2, use zip file first and when everything has loaded through api switch to that

                TestBox.Text = "Stations Loaded";
            }
            catch (System.Net.Http.HttpRequestException)
            {
                TestBox.Text = "Error in loading, Connect to the internet?";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TestBox.Text = "Error in loading stations";
            }
        }
        private void BtnMap_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://tfl.gov.uk/maps/track"); //An attempt at a hyperlink
        }

        private bool CheckLineStatus(string lineId)
        {
            return true;
        }

        private bool CheckStationStatus(string stationId)
        {
            return true;
        }

        public static double Squared(double x)
        {
            return x * x;
        }

        private List<string> RouteByLines(Root givenStation, Root matchingStation, List<string> stations, bool sameLine, List<string> preCheckedLineIds, ref int iteration) // problem if line isn't fully connected (for example overground)
        {
            iteration += 1;
            Root newStation = new();

            double currentStationDistance = Math.Sqrt(Squared(matchingStation.Lon - newStation.Lon) + Squared(matchingStation.Lat - newStation.Lat));
            if (newStation.GetCommonName() != null) //
            {
                currentStationDistance = Math.Sqrt(Squared(matchingStation.Lon - newStation.Lon) + Squared(matchingStation.Lat - newStation.Lat));
            }
            else
            {
                currentStationDistance = 0.0;
            }

            if (sameLine == false)
            {
                foreach (Line line in givenStation.GetLines())
                {
                    bool preCheckedLine = false; 


                    for (var i = 0; i < preCheckedLineIds.Count; i++)
                    {
                        if (line.GetId() == preCheckedLineIds[i])
                        {
                            preCheckedLine = true;
                        }
                    }

                    if (_lines.ContainsKey(line.GetId()) && preCheckedLine == false) //Checking for unwanted and pre-checked lines, (for example national rail lines)
                    {
                        foreach (Root station in _lines[line.GetId()]) //All stations on the givenStation line
                        {
                            double newStationDistance = Math.Sqrt(Squared(matchingStation.Lon - station.Lon) + Squared(matchingStation.Lat - station.Lat));
                            

                            int differentKnownLines = -1;
                            for (var i = 0; i < station.GetLines().Count; i++) //All lines from chosen station
                            {
                                for (var j = 0; j < matchingStation.GetLines().Count; j++) //All lines on the matching station
                                {
                                    if (station.GetLines()[i].GetId() == matchingStation.GetLines()[j].GetId() && _lines.ContainsKey(station.GetLines()[i].GetId())) //Check if there's a station that shares the same line to the matching station
                                    {
                                        TestBox.Text += $"From Routing: Matched {station.GetLines()[i].Name} at {station.GetCommonName()}";
                                        TestBox.Text += Environment.NewLine;

                                        sameLine = true;
                                        
                                        if (stations.Count > iteration)
                                        {
                                            stations.RemoveAt(stations.Count - 1);
                                        }
                                        
                                        stations.Add(station.GetCommonName());
                                    }
                                    else if (givenStation.GetId() != station.GetId() && station.GetLines().Count > 1 && sameLine == false) //Finding all other lines from the from station line
                                    {
                                        if (differentKnownLines > 0) //Checking for unwanted lines
                                        {
                                            if (currentStationDistance == 0.0) //Whether the newStation is null
                                            {
                                                newStation = _stations[station.GetCommonName()];
                                                currentStationDistance = newStationDistance;
                                            }
                                            else if (newStationDistance < currentStationDistance) //Whether a different station than the currentStation towards the matched station
                                            {
                                                newStation = _stations[station.GetCommonName()];
                                                currentStationDistance = newStationDistance;
                                            }
                                        }

                                        if (_lines.ContainsKey(station.GetLines()[i].GetId()));
                                        {
                                            differentKnownLines += 1;
                                        }
                                    }
                                }
                            }
                        }
                        preCheckedLineIds.Add(line.GetId());
                    }
                }

                if (sameLine == false) //Only used for when there's more than 1 change of lines
                {
                    stations.Add(newStation.GetCommonName());
                    stations = RouteByLines(newStation, matchingStation, stations, sameLine, preCheckedLineIds, ref iteration);
                }
            }
            return stations;

            /*
             * if (!To and From station on same line)
             *      while (!Unknown stations on the same line)
             *      get all stations on new lines
             *      find 1 new line from found station
             *      (possibility) - longitude and latitudinally find the closest station chosen for new station if more than one station is matched - problem that I wouldn't know if this method is truly the fastest route
             * Iterate atleast 3 times and save into list of solutions - should seperately attempt to find a route for disabled people - i forgor :skull:
             * find time taken for all - can be through timetable call
             * use the smallest time taken solution
             * return route
             */
        }

        private async System.Threading.Tasks.Task<string> RouteByDjikstra() //unfinished
        {
            // * Make table of all stations with a distance of 1
            //Idea: await FastRouting_Unchecked()
            
            var modes = await _client.GetAllModesAsync();
            Dictionary<string, Dictionary<string, List<OrderedLineRoute>>> orderedStationsByMode = new(); //mode<line<stations>>

            foreach (var mode in modes)
            {
                if (mode.GetModeName() == "tube" | mode.GetModeName() == "dlr" | mode.GetModeName() == "elizabeth-line" | mode.GetModeName() == "overground" | mode.GetModeName() == "tram" | mode.GetModeName() == "cable-car")
                {
                    var lines = await _client.GetAllLinesByModeAsync(mode.GetModeName());
                    Dictionary<string, List<OrderedLineRoute>> orderedStations = new Dictionary<string, List<OrderedLineRoute>>();

                    foreach (Root line in lines)
                    {
                        var lineRoute = await _client.GetLineRouteByLineAsync(line.GetId(), "inbound");
                        orderedStations.Add(line.GetName(), lineRoute.GetOrderedLineRoutes());
                        ResultListBox.Items.Add(line.GetName());
                    }
                    orderedStationsByMode.Add(mode.GetModeName(), orderedStations);
                }
            }

            return "";
            // *use djisktra's algorithm to find shortest route
            //* return route
        }

        private async void Route()
        {

            // I currently don't know which one is faster, by the end i'll find out which is faster and set it to that certain routing method

            if ((bool)FastRouting.IsChecked!)//Method 1 - Compare by lines
            {
                bool sameLine = false;

                
                /*
                 * Find lines that both stations are on
                 */
                Root fromStation = _stations[FromComboBox.SelectionBoxItem.ToString()!];
                Root toStation = _stations[ToComboBox.SelectionBoxItem.ToString()!];


                foreach (Line fromLine in fromStation.GetLines()) //All lines related to the from station
                {
                    foreach (Line toLine in toStation.GetLines()) //All lines related to the to station
                    {
                        if (fromLine.GetId() == toLine.GetId()) //Whether stations are on the same line
                        {
                            TestBox.Text += $"From Route: Matched {fromLine.GetId()} ";
                            sameLine = true;
                        }
                    }
                }

                List<string> route = new();
                List<string> lineIds = new();
                int iteration = 0;

                route.Add(toStation.GetCommonName());
                RouteByLines(fromStation, toStation, route, sameLine, lineIds, ref iteration);
                route.Add(fromStation.GetCommonName());
                TestBox.Text += $"Iterations - {iteration}";

                foreach (var station in route)
                {
                    ResultListBox.Items.Add(station);
                }
            }
            else //Method 2 - Use Djisktra
            {
                await RouteByDjikstra();
            }
        }

        private void BtnRoute_Click(object sender, RoutedEventArgs e)
        {
            ResultListBox.Items.Clear();
            TestBox.Clear();
            if (FromComboBox.SelectionBoxItem.ToString() == "" | ToComboBox.SelectionBoxItem.ToString() == "")
            {
                ResultListBox.Items.Add("You might be missing some value(s)");
            }
            else
            {
                if (FromComboBox.SelectionBoxItem == ToComboBox.SelectionBoxItem)
                {
                    ResultListBox.Items.Add("Nice");
                }
                else
                {
                    Route();
                    //find extra info fares, wifi, toilets, etc
                    //Display all
                }
            }
        }

        //Testing purposes from here

        private void FromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Might be used to display disruptions
        {
            TestBox.Clear();
            if (FromComboBox.SelectedItem.ToString() != null)
            {
                TestBox.Text = $"{FromComboBox.SelectedItem.ToString()} Lon - {_stations[FromComboBox.SelectedItem.ToString()].Lon} Lat - {_stations[FromComboBox.SelectedItem.ToString()].Lat}";
                for (int i = 0; i < _stations[FromComboBox.SelectedItem.ToString()].GetLines().Count; i++)
                {
                    TestBox.Text += $"{Environment.NewLine}{_stations[FromComboBox.SelectedItem.ToString()].GetLines()[i].Name}"; //Finding out what lines are on the chosen station
                }
            }
        }

        private void ToComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Might be used to display disruptions
        {
            TestBox.Clear();
            if (ToComboBox.SelectedItem.ToString() != null)
            {
                TestBox.Text = $"{ToComboBox.SelectedItem.ToString()} Lon - {_stations[ToComboBox.SelectedItem.ToString()].Lon} Lat  - {_stations[ToComboBox.SelectedItem.ToString()].Lat}";
                for (int i = 0; i < _stations[ToComboBox.SelectedItem.ToString()].GetLines().Count; i++)
                {
                    TestBox.Text += $"{Environment.NewLine}{_stations[ToComboBox.SelectedItem.ToString()].GetLines()[i].Name}"; //Finding out what lines are on the chosen station
                }
            }
        }

        private async void FastRouting_Unchecked(object sender, RoutedEventArgs e) 
        {
            //Create Djisktras table
        }
    }

//Ideas
/* Make a station/line viewer that displays info about it
 * Accounts
 * Save data to either a server or a database
 */
}