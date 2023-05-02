using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Tube_Traveller.Model;
using Tube_Traveller.Accounts;
using System.Threading.Tasks;

namespace Tube_Traveller
{
    public partial class MainWindow : Window
    {
        //todo Save stations to a text file
        private Account? _userAccount = new();
        private TflClient _client = new();
        private Dictionary<string, Root> _stations = new(); //key: stationCommonName, value: station
        private Dictionary<string, List<Root>> _lines = new(); //key: lineId, value: station
        private bool _hasStationsLoaded = false;

        public MainWindow()
        {
            LoadStations(0);
            InitializeComponent();
        }

        private async void LoadStations(int iteration)
        {
            //Getting stations from api             
            try
            {
                List<string> tempStations = await GetAllStations();
                tempStations.Sort();

                for (int i = 0; i < tempStations.Count; i++)
                {
                    FromComboBox.Items.Add(tempStations[i]);
                    ToComboBox.Items.Add(tempStations[i]);
                }

                if (_userAccount != null)
                {
                    AddHomeStation();
                }

                _hasStationsLoaded = true;
                MainBox.Text = "Loaded Stations";
            }
            catch (System.Net.Http.HttpRequestException) //If user isn't connected to the internet
            {
                MainBox.Text = "Error in loading, Connect to the internet?";
                if (iteration < 4)
                {
                    LoadStations(iteration);
                }
            }
            catch (Exception) //Any other error that I wouldn't know
            {
                MainBox.Text = "Error in loading stations";
            }
        }

        private async Task<List<string>> GetAllStations()
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

            return tempStations;
        }

        private void ReloadStationsBtn_Click(object sender, RoutedEventArgs e)
        {
            _stations.Clear();
            _lines.Clear();
            FromComboBox.Items.Clear();
            ToComboBox.Items.Clear();
            MainBox.Text = "Reloading Stations";
            LoadStations(0);
        }

        private void BtnRoute_Click(object sender, RoutedEventArgs e)
        {
            ResultListBox.Items.Clear();
            MainBox.Clear();
            if (FromComboBox.SelectionBoxItem.ToString() == string.Empty | ToComboBox.SelectionBoxItem.ToString() == string.Empty)
            {
                MainBox.Text = "You might be missing some value(s)... Choose station(s) (from and to)";
            }
            else
            {
                if (FromComboBox.SelectionBoxItem == ToComboBox.SelectionBoxItem)
                {
                    MainBox.Text = "Nice, you've entered the same stations";
                }
                else
                {
                    Route();
                    ExtraRoutingInformation();
                }
            }
        }

        private void Route()
        {
            List<Root> route = new(); //Contains stations to switch lines at unless the 2 stations chosen are the same line
            RoutingCalculations routing = new RoutingCalculations(_client, _stations, _lines);

            try //Deciding method on finding the route
            {
                if (_userAccount!.GetRouteMethod() == Text.GetRouteMethod_A())
                {
                    MainBox.Text = "Routing Algorithimcally currently is a work in progress... please change the Route Method back to regular";
                   // route.Concat(await routing.RouteByDjikstra());
                }
                else 
                {
                    RouteByCompareLines(ref route, routing);
                }
            }
            catch (System.NullReferenceException)
            {
                RouteByCompareLines(ref route, routing);
            }

            foreach (Root station in route)
            {
                ResultListBox.Items.Add(station.GetCommonName());
            }
        }

        private void RouteByCompareLines(ref List<Root> route, RoutingCalculations routing)
        {
            bool sameLine = false;
            Root fromStation = _stations[FromComboBox.SelectionBoxItem.ToString()!];
            Root toStation = _stations[ToComboBox.SelectionBoxItem.ToString()!];

            foreach (Line fromLine in fromStation.GetLines())
            {
                foreach (Line toLine in toStation.GetLines())
                {
                    if (fromLine.GetId() == toLine.GetId()) //Whether stations are on the same line
                    {
                        if (_lines.ContainsKey(fromLine.GetId()))
                        {
                            MainBox.Text += $"Take the {fromLine.GetId()} line\n";
                            sameLine = true;
                        }
                    }
                }
            }
            route.Concat(routing.RouteByLines(fromStation, toStation, route, sameLine, new List<string>(), 0));
        }

        private async void ExtraRoutingInformation()
        {
            RouteInfo.Clear();

            if (FromComboBox.SelectedItem.ToString() != null && ToComboBox.SelectedItem.ToString() != null)
            {
                try
                {
                    Root Journey = await _client.GetJourney(_stations[FromComboBox.SelectedItem.ToString()!].GetId(), _stations[ToComboBox.SelectedItem.ToString()!].GetId(), DateTime.Now.ToString("HHmm"), "departing");
                    RouteInfo.Text += $"Fare - £{Journey.GetJourneys()![0].GetFare()!.GetTotalCost().ToString()!.Insert(Journey.GetJourneys()![0].GetFare()!.GetTotalCost().ToString()!.Length - 2, ".")}";
                    RouteInfo.Text += $"\nDuration - {Journey.GetJourneys()![0].GetDuration()} minutes";
                    RouteInfo.Text += $"\nDeparture Time - {Journey.GetJourneys()![0].GetLegs()![0].GetDepartureTime().TimeOfDay}";
                    RouteInfo.Text += $"\n\nShows single fare payment ONLY";
                }
                catch
                {
                    RouteInfo.Text += "Sorry an error has occured... try again later";
                }
            }
        }

        private void BtnMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://tfl.gov.uk/maps/track") { UseShellExecute = true });
            }
            catch
            {
                MainBox.Text = "Connect to the internet";
            }
        }

        private void AccountBtn_Click(object sender, RoutedEventArgs e)
        {

            if (_hasStationsLoaded == true)
            {
                if (AccountBtn.Content.ToString() == "Options")
                {
                    AccountOptions();
                }
                else
                {
                    Login();
                }
            }
            else
            {
                MainBox.Text = "Wait for stations to be loaded"; // As all stations wouldn't be able to be chosen for homeStation
            }
        }

        private void Login()
        {
            LoginWindow loginWindow = new();
            loginWindow.SetStations(_stations.Keys.ToList());
            bool? result = loginWindow.ShowDialog(); //Gets data before closing window

            if (result == true) //New account has been made or account has been logged into
            {
                _userAccount = loginWindow.GetAccount();
                AccountBtn.Content = "Options";
                AddHomeStation();
            }
        }

        /// <summary>
        /// Adds homestation to lists of stations
        /// </summary>
        private void AddHomeStation()
        {
            if (!string.IsNullOrEmpty(_userAccount!.GetHomeStation()))
            {
                _stations.Add("Home Station", _stations[_userAccount.GetHomeStation()!]);
                FromComboBox.Items.Insert(0, "Home Station");
                ToComboBox.Items.Insert(0, "Home Station");
            }
        }

        private void AccountOptions()
        {
            AccountSettingsWindow accountSettingsWindow = new();
            accountSettingsWindow.SetAccount(_userAccount!);
            accountSettingsWindow.SetStations(_stations.Keys.ToList());
            bool? result = accountSettingsWindow.ShowDialog();

            if (result == true) //Logged out
            {
                AccountBtn.Content = "Sign In";
                RemoveHomeStations();
            }
        }

        /// <summary>
        /// Removes Home Station from lists of stations
        /// </summary>
        private void RemoveHomeStations()
        {
            _stations.Remove("Home Station");
            FromComboBox.Items.RemoveAt(0);
            ToComboBox.Items.RemoveAt(0);
        }
    }
}