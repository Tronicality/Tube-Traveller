using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    public partial class MainWindow : Window
    {
        TflClient _client;
        Dictionary<string, Root> _stations = new();

        public MainWindow()
        {
            _client = new TflClient();
            LoadStations();
            InitializeComponent();
        }

        private async void LoadStations()
        {
            TestBox.Text = "Loading...";
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
            catch (Exception)
            {
                TestBox.Text = "Error, maybe not connected to the internet?";
            }
        }

        private bool CheckLineStatus(string lineId) //Whether station or line
        {
            return true;
        }

        private bool CheckStationStatus(string stationId)
        {
            return true;
        }

        private void Route()
        {
            bool sameLine = false;

            //Method 1 - Compare by lines
            /*
             * Find lines that both stations are on
             */
            Root fromStation = _stations[FromComboBox.SelectionBoxItem.ToString()!]; //would like to make it station but is impossible to get without missing information
            Root toStation = _stations[ToComboBox.SelectionBoxItem.ToString()!];

            foreach (Line fromLine in fromStation.GetLines())
            {
                foreach (Line toLine in toStation.GetLines())
                {
                    if (fromLine.Equals(toLine))
                    {
                        sameLine = true;
                    }
                }
            }

            TestBox.Text = $"Are they on the same line: {sameLine}";

            /*
             * if (!To and From station on same line)
             *      while (!Unknown stations on the same line)
             *      get all stations on new lines
             *      find 1 new line from found station
             *      (possibility) - longitude and latitudinally find the closest station chosen for new station
             * Iterate atleast 3 times and save into list of solutions - should seperately attempt to find a route for disabled people
             * find time taken for all - can be through timetable call
             * use the smallest time taken solution
             * return route
             */


            //Method 2 - Use Djisktra
            /*
             * Make table of all stations with a distance of 1

            var modes = await _client.GetAllModesAsync();


            Dictionary<string, Dictionary<string, List<OrderedLineRoute>>> orderedStationsByMode = new Dictionary<string, Dictionary<string,List<OrderedLineRoute>>>();

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
    }

//Ideas
/* Make a station/line viewer that displays info about it
 * Accounts
 * Save data to either a server or a database
 */
}