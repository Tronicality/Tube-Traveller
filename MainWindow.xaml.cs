using System;
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


            //Getting stations from api - 1
            /*
            List<object> tempStations = new List<object>();
            var modes = await _client.GetAllModesAsync();

            //Dictionary<string, Dictionary<string, List<OrderedLineRoute>>> orderedStationsByMode = new Dictionary<string, Dictionary<string, List<OrderedLineRoute>>>();
            
            
            foreach (var mode in modes) //per mode
            {
                if (mode.GetModeName() == "tube" | mode.GetModeName() == "dlr" | mode.GetModeName() == "elizabeth-line" | mode.GetModeName() == "overground" | mode.GetModeName() == "tram" | mode.GetModeName() == "cable-car")
                {
                    var lines = await _client.GetAllLinesByModeAsync(mode.GetModeName()); 
                    foreach (var line in lines) //per line
                    {
                        var lineRoute = await _client.GetLineRouteAsync(line.GetId(),"inbound"); //Gets information but is used for just getting stations on the line
                        foreach (var station in lineRoute.Stations!) //per station
                        {
                            //stations.Add(station.Name,);
                            if (!tempStations.Contains(station.Name!))
                            {
                                _stations.Add(station.Name!, station.Id!);
                                tempStations.Add(station.Name!);
                            }
                        }

                        lineRoute = await _client.GetLineRouteAsync(line.GetId(), "outbound"); //Gets information but is used for just getting stations on the line
                        foreach (var station in lineRoute.Stations!) //per station
                        {
                            //stations.Add(station.Name,);
                            if (!tempStations.Contains(station.Name!))
                            {
                                _stations.Add(station.Name!, station.StationId!);
                                tempStations.Add(station.Name!);
                            }
                        }
                    }
                }
            }
            tempStations.Sort();
            FromComboBox.ItemsSource = tempStations;
            ToComboBox.ItemsSource = tempStations;
            */


            //Getting stations from api - 2
            
            try
            {
                List<Root> modes = await _client.GetAllModesAsync();
                List<string> tempStations = new();
                
                foreach (Root mode in modes)
                {
                    if (mode.GetModeName() == "tube" | mode.GetModeName() == "dlr" | mode.GetModeName() == "elizabeth-line" | mode.GetModeName() == "overground" | mode.GetModeName() == "tram")
                    {
                        List<Root> lines = await _client.GetAllLinesByModeAsync(mode.GetModeName()); 
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
                    }
                }

                tempStations.Sort();
                FromComboBox.ItemsSource = tempStations;
                ToComboBox.ItemsSource = tempStations;

                //Should probably statuses per station only if it's bad

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

        private string RouteByLines(Root station, Root matchingStation, List<string> stations, bool sameLine)
        {
            Root newStation = new();
            while (sameLine == false)
            {
                foreach (Line line in station.GetLines())
                {
                    if (_lines.ContainsKey(line.GetId())) //Checking for unwanted lines, (for example national rail lines)
                    {
                        foreach (Root lineStation in _lines[line.GetId()]) //All stations on the station line
                        {

                            /*
                            foreach (var station in _stations[newLineStation]) //Would need to establish my own Enumerator, not worth as I wouldn't get any marks, will probably do it anyways for readablility
                            {

                            }
                            */
                            int differentKnownLines = -1;
                            for (var i = 0; i < lineStation.GetLines().Count; i++) //All lines from chosen station
                            {
                                for (var j = 0; j < matchingStation.GetLines().Count; j++) //All lines on the matching station
                                {
                                    if (lineStation.GetLines()[i].GetId() == matchingStation.GetLines()[j].GetId() && _lines.ContainsKey(lineStation.GetId())) //Check if there's a station that shares the same line to the matching station
                                    {
                                        TestBox.Text += $"From Routing: Matched {lineStation.GetLines()[i].Name} at {lineStation.GetCommonName()}";
                                        TestBox.Text += Environment.NewLine;

                                        sameLine = true;
                                        return lineStation.GetCommonName();
                                    }
                                    else if (station.GetId() != lineStation.GetId() && lineStation.GetLines().Count > 1 && sameLine == false) //Finding all other lines from the from station line
                                    {
                                        if (differentKnownLines > 0) //Checking for unwanted lines
                                        {
                                            newStation = _stations[lineStation.GetCommonName()];
                                        }
                                        else if (_lines.ContainsKey(lineStation.GetLines()[i].GetId()))
                                        {
                                            differentKnownLines += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                stations.Add(RouteByLines(newStation, matchingStation, stations, sameLine));
            }
            return newStation.GetName();
        }

        private void Route()
        {
            bool sameLine = false;

            //Method 1 - Compare by lines
            /*
             * Find lines that both stations are on
             */
            Root fromStation = _stations[FromComboBox.SelectionBoxItem.ToString()!];
            Root toStation = _stations[ToComboBox.SelectionBoxItem.ToString()!];
            List<string> route = new();


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

            route.Add(fromStation.GetCommonName());
            RouteByLines(fromStation,toStation, route, sameLine);
            route.Add(toStation.GetCommonName());

            TestBox.Text += Environment.NewLine + "From Route";
            foreach (var station in route)
            {
                TestBox.Text += station;
                TestBox.Text += Environment.NewLine;
            }
            /*
             * if (!To and From station on same line)
             *      while (!Unknown stations on the same line)
             *      get all stations on new lines
             *      find 1 new line from found station
             *      (possibility) - longitude and latitudinally find the closest station chosen for new station if more than one station is matched
             * Iterate atleast 3 times and save into list of solutions - should seperately attempt to find a route for disabled people
             * find time taken for all - can be through timetable call
             * use the smallest time taken solution
             * return route
             */


            //Method 2 - Use Djisktra
            /*
             * Make table of all stations with a distance of 1

            var modes = await _client.GetAllModesAsync();


            Dictionary<string, Dictionary<string, List<OrderedLineRoute>>> orderedStationsByMode = new(); //mode<line<stations>>

            foreach (var mode in modes)
            {
                if (mode.GetModeName() == "tube" | mode.GetModeName() == "dlr" | mode.GetModeName() == "elizabeth-line" | mode.GetModeName() == "overground" | mode.GetModeName() == "tram" | mode.GetModeName() == "cable-car")
                {
                    var lines = await _client.GetAllLinesByModeAsync(mode.GetModeName());
                    Dictionary<string, List<OrderedLineRoute>> orderedStations = new Dictionary<string, List<OrderedLineRoute>>();

                    foreach (var line in lines)
                    {
                        var lineRoute = await _client.GetLineRouteAsync(line.GetId());
                        orderedStations.Add(line.GetName(), lineRoute.GetOrderedLineRoutes());
                        ResultListBox.Items.Add(line.GetName());
                    }
                    orderedStationsByMode.Add(mode.GetModeName(), orderedStations);
                }
            //unfinished

            *use djisktra's algorithm to find shortest route
             * return route
             */
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
                TestBox.Text = FromComboBox.SelectedItem.ToString() + "lines:";
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
                TestBox.Text = ToComboBox.SelectedItem.ToString() + "lines:";
                for (int i = 0; i < _stations[ToComboBox.SelectedItem.ToString()].GetLines().Count; i++)
                {
                    TestBox.Text += $"{Environment.NewLine}{_stations[ToComboBox.SelectedItem.ToString()].GetLines()[i].Name}"; //Finding out what lines are on the chosen station
                }
            }
        }
    }

//Ideas
/* Make a station/line viewer that displays info about it
 * Accounts
 * Save data to either a server or a database
 */
}