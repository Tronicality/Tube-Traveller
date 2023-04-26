using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tube_Traveller.Model;
using Tube_Traveller.Accounts;
using System.Diagnostics;

namespace Tube_Traveller
{
    public partial class MainWindow : Window
    {
        //todo Options page where you can change account settings

        Account? userAccount = new();
        TflClient _client = new();
        Dictionary<string, Root> _stations = new(); //key: stationCommonName, value: station
        Dictionary<string, List<Root>> _lines = new(); //key: lineId, value: station

        public MainWindow()
        {
            LoadStations();
            InitializeComponent();
        }

        private async void LoadStations()
        {
            //Getting stations from api             
            try
            {
                List<string> tempStations = new();
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

                for (int i = 0; i < tempStations.Count; i++)
                {
                    FromComboBox.Items.Add(tempStations[i]);
                    ToComboBox.Items.Add(tempStations[i]);
                }

                //FromComboBox.ItemsSource = tempStations;
                //ToComboBox.ItemsSource = tempStations;


                //Should probably check statuses per station only if it's bad

                MainBox.Text = "Stations Loaded";
            }
            catch (System.Net.Http.HttpRequestException) //If user isn't connected to the internet
            {
                MainBox.Text = "Error in loading, Connect to the internet?";
            }
            catch (Exception ex) //Any other error that I wouldn't know
            {
                Debug.WriteLine(ex.Message);
                MainBox.Text = "Error in loading stations";
            }
        }
        private void BtnMap_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://tfl.gov.uk/maps/track") { UseShellExecute = true });
        }

        private void AccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AccountBtn.Content.ToString() == "Sign Out") //Logout
            {
                Logout();
            }
            else
            {
                Login();
            }
        }

        private void Logout()
        {
            userAccount = null;
            AccountBtn.Content = "Sign In";

            _stations.Remove("Home Station");
            FromComboBox.Items.RemoveAt(0);
            ToComboBox.Items.RemoveAt(0);
        }

        private void Login()
        {
            LoginWindow loginWindow = new();
            loginWindow.SetStations(_stations.Keys.ToList());
            bool? result = loginWindow.ShowDialog();

            if (result == true) //New account has been made or account has been logged into
            {
                userAccount = loginWindow.GetAccount();
                AccountBtn.Content = "Sign Out";

                if (userAccount?.GetHomeStation() != "")
                {
                    _stations.Add("Home Station", _stations?[userAccount?.GetHomeStation()]);
                    FromComboBox.Items.Insert(0, "Home Station");
                    ToComboBox.Items.Insert(0, "Home Station");
                }
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
                }
            }
        }

        private async void Route()
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
                            try
                            {
                                if (_lines[fromLine.GetId()] is not null)
                                {
                                    MainBox.Text += $"Take the {fromLine.Id} line\n";
                                    sameLine = true;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
                
                route.Concat(routing.RouteByLines(fromStation, toStation, route, sameLine, new List<string>(), 0));
            }
            else //Method 2 - Use Djisktra
            {
               route.Concat(await routing.RouteByDjikstra());
            }

            foreach (Root station in route)
            {
                ResultListBox.Items.Add(station.GetCommonName());
            }
        }

        private async void ExtraRoutingInformation()
        {
            RouteInfo.Clear();

            if (FromComboBox.SelectedItem.ToString() != null && ToComboBox.SelectedItem.ToString() != null)
            {
                Root Journey = await _client.GetJourney(_stations[FromComboBox.SelectedItem.ToString()].Id, _stations[ToComboBox.SelectedItem.ToString()].Id, DateTime.Now.ToString("HHmm"), "departing");
                RouteInfo.Text += $"Fare - £{Journey.Journeys[0].Fare.TotalCost.ToString().Insert(Journey.Journeys[0].Fare.TotalCost.ToString().Length - 2, ".")}";
                RouteInfo.Text += $"\nDuration - {Journey.Journeys[0].Duration} minutes";
                RouteInfo.Text += $"\n\nShows single fare payment ONLY";

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