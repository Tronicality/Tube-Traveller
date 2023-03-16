using Microsoft.Data.Sqlite;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    //Ideas
    /*todo Make a station/line viewer that displays info about it
    *todo Accounts, for: home station, DOB, fare payment type, 
     *todo Save data to either a server or a database
     *todo Use DB in future because it be easy 
     */
    public partial class MainWindow : Window
    {
        TflClient _client;
        Dictionary<string, Root> _stations = new(); //key: stationCommonName, value: station
        Dictionary<string, List<Root>> _lines = new(); //key: lineId, value: station

        public MainWindow()
        {
            _client = new TflClient();
            LoadStations();
            InitializeComponent();
        }

        private async void LoadStations()
        {
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

                MainBox.Text = "Stations Loaded";
            }
            catch (System.Net.Http.HttpRequestException) //If user isn't connected to the internet
            {
                MainBox.Text = "Error in loading, Connect to the internet?";
            }
            catch (Exception ex) //Any other error that I wouldn't know
            {
                Console.WriteLine(ex.Message);
                MainBox.Text = "Error in loading stations";
            }
            //Server side error
        }
        private void BtnMap_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("https://tfl.gov.uk/maps/track"); //An attempt at a hyperlink
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://tfl.gov.uk/maps/track") { UseShellExecute = true });
        }

        private async void ExtraRoutingInformation()
        {
            RouteInfo.Clear();
            //todo find toilets

            /*  for each station except the last one in route
            *  Find station departure time
            *  Display: station name - departure time
            *  Find time taken to get to next station
            *  Give user atleast {time} minutes to walk since the calculated time after time taken
            */

            //todo Implement Date in future
            //TestBox.Text = Date.SelectedDate.HasValue ? (Date.SelectedDate.Value.Year.ToString() + ((Convert.ToInt32(Date.SelectedDate.Value.Month) < 10) ? "0" + Date.SelectedDate.Value.Month.ToString() : Date.SelectedDate.Value.Month.ToString()) + ((Convert.ToInt16(Date.SelectedDate.Value.Day) < 10) ? "0" + Date.SelectedDate.Value.Day.ToString() : Date.SelectedDate.Value.Day.ToString())) : (Date.DisplayDate.Year.ToString() + ((Convert.ToInt16(Date.DisplayDate.Month) < 10) ? "0" + Date.DisplayDate.Month.ToString() : Date.DisplayDate.Month.ToString()) + ((Convert.ToInt16(Date.DisplayDate.Day) < 10) ? "0" + Date.SelectedDate.Value.Day.ToString() : Date.SelectedDate.Value.Day.ToString()));


            if (FromComboBox.SelectedItem.ToString() != null && ToComboBox.SelectedItem.ToString() != null)
            {
                if (ResultListBox.Items.Count == 2)
                {
                    Root Journey = await _client.GetJourney(_stations[FromComboBox.SelectedItem.ToString()].Id, _stations[ToComboBox.SelectedItem.ToString()].Id, DateTime.Now.ToString("HHmm"), "departing");
                    RouteInfo.Text += $"Fare - £{Journey.Journeys[0].Fare.TotalCost.ToString().Insert(Journey.Journeys[0].Fare.TotalCost.ToString().Length - 2, ".")}";
                    RouteInfo.Text += $"\nDuration - {Journey.Journeys[0].Duration} minutes";
                }
                else if (ResultListBox.Items.Count == 3)
                {
                    Root Journey = await _client.GetJourney(_stations[FromComboBox.SelectedItem.ToString()].Id, _stations[ToComboBox.SelectedItem.ToString()].Id, DateTime.Now.ToString("HHmm"), "departing");
                    RouteInfo.Text += $"Fare - £{Journey.Journeys[0].Fare.TotalCost.ToString().Insert(Journey.Journeys[0].Fare.TotalCost.ToString().Length - 2, ".")}";
                    RouteInfo.Text += $"\nDuration - {Journey.Journeys[0].Duration} minutes";
                }
                /*
                 * List<Root> Fares = await _client.GetFares(_stations[FromComboBox.SelectedItem.ToString()].Id, _stations[ToComboBox.SelectedItem.ToString()].Id);
                 * 
                else
                {
                    for (int i = 0; i < ResultListBox.Items.Count; i++)
                    {
                        if (i < ResultListBox.Items.Count - 1)
                        {
                            Root Journey = await _client.GetJourney(ResultListBox.Items.GetItemAt(0).ToString(), ResultListBox[i + 1].Id, DateTime.Now.ToString("HHmm"), "departing");

                        }
                    }
                }
                */
            }
        }

        private async System.Threading.Tasks.Task<string> RouteByDjikstra() //unfinished
        {
            //todo Make table of all stations with a distance of 1
            //todo Idea: await FastRouting_Unchecked() - used as check for whether station table has been created
            

            var lines = await _client.GetAllLinesByModeAsync("tube");
            Dictionary<string, List<OrderedLineRoute>> orderedStations = new Dictionary<string, List<OrderedLineRoute>>();

            foreach (Root line in lines)
            {
                var lineRoute = await _client.GetLineRouteByLineAsync(line.GetId(), "inbound");
                orderedStations.Add(line.GetName(), lineRoute.GetOrderedLineRoutes());
                ResultListBox.Items.Add(line.GetName());
            }
            return "";
            // *use djisktra's algorithm to find shortest route
            //* return route
        }

        private void Route()
        {
            List<Root> route = new(); //Contains stations to switch lines at unless the 2 stations chosen are the same line
            RoutingCalculations routing = new RoutingCalculations(_client, _stations, _lines);
            // I currently don't know which one is faster, by the end i'll find out which is faster and set it to that certain routing method

            if ((bool)FastRouting.IsChecked!)//Method 1 - Compare by lines
            {

                bool sameLine = false;
                Root fromStation = _stations[FromComboBox.SelectionBoxItem.ToString()!];
                Root toStation = _stations[ToComboBox.SelectionBoxItem.ToString()!];

                foreach (Line fromLine in fromStation.GetLines()) //All lines related to the from station
                {
                    foreach (Line toLine in toStation.GetLines()) //All lines related to the to station
                    {
                        if (fromLine.GetId() == toLine.GetId()) //Whether stations are on the same line
                        {
                            //TestBox.Text += $"From Route: Matched {fromLine.GetId()}";
                            MainBox.Text += $"Take the {fromLine.Id} line";
                            sameLine = true;
                        }
                    }
                }
                
                route.Concat(routing.RouteByLines(fromStation, toStation, route, sameLine, new List<string>(), 0));
            }
            else //Method 2 - Use Djisktra
            {
               //route.Concat(routing.RouteByDjikstra());
            }

            foreach (Root station in route)
            {
                ResultListBox.Items.Add(station.GetCommonName());
            }
        }

        private void BtnRoute_Click(object sender, RoutedEventArgs e)
        {
            ResultListBox.Items.Clear();
            MainBox.Clear();
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
                    ExtraRoutingInformation();
                    //Display all
                }
            }
        }

        //Testing purposes from here

        private void FromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Might be used to display disruptions
        {
            MainBox.Clear();
            /* Checking lines and stuff
            if (FromComboBox.SelectedItem.ToString() != null)
            {
                TestBox.Text = $"{FromComboBox.SelectedItem.ToString()} StationId - {_stations[FromComboBox.SelectedItem.ToString()].Id} Lon - {_stations[FromComboBox.SelectedItem.ToString()].Lon} Lat - {_stations[FromComboBox.SelectedItem.ToString()].Lat}";
                for (int i = 0; i < _stations[FromComboBox.SelectedItem.ToString()].GetLines().Count; i++)
                {
                    TestBox.Text += $"{Environment.NewLine}{_stations[FromComboBox.SelectedItem.ToString()].GetLines()[i].Name}"; //Finding out what lines are on the chosen station
                }
            }
            */
        }

        private void ToComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Might be used to display disruptions
        {
            MainBox.Clear();
            /* Checking lines and stuff
            if (ToComboBox.SelectedItem.ToString() != null)
            {
                TestBox.Text = $"{ToComboBox.SelectedItem.ToString()} StationId - {_stations[ToComboBox.SelectedItem.ToString()].Id} Lon - {_stations[ToComboBox.SelectedItem.ToString()].Lon} Lat  - {_stations[ToComboBox.SelectedItem.ToString()].Lat}";
                for (int i = 0; i < _stations[ToComboBox.SelectedItem.ToString()].GetLines().Count; i++)
                {
                    TestBox.Text += $"{Environment.NewLine}{_stations[ToComboBox.SelectedItem.ToString()].GetLines()[i].Name}"; //Finding out what lines are on the chosen station
                }
            }
            */
        }

        private void FastRouting_Unchecked(object sender, RoutedEventArgs e) 
        {
            //Create Djisktras table
        }
    }
}