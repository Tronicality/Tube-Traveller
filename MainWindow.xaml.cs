using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Compression;
using System.IO;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    public partial class MainWindow : Window
    {
        TflClient _client;

        public MainWindow()
        {
            InitializeComponent();
            _client = new TflClient();
            LoadStations();
        }

        private async void LoadStations()
        {
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
            }
        }

        private async void btnInput_Click(object sender, RoutedEventArgs e)
        {
            ResultListBox.Items.Clear();
            try
            {
                var datas = await _client.GetArrivalsAsync(inputBox.Text);  // Gives a list of train routes 
                if (datas.Count == 0)
                {
                    ResultListBox.Items.Add("Nothing was found");
                }
                else
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        ResultListBox.Items.Add($"Line: {datas[i].LineName} - Mode: {datas[i].ModeName}\nStation Name: {datas[i].StationName} - Current Location: {datas[i].CurrentLocation}\nDestination: {datas[i].DestinationName}\nExpected Arrival: {datas[i].ExpectedArrival.ToLocalTime()}\n\n\n");
                    }
                }
            }
            catch (Exception)
            {
                ResultListBox.Items.Add("Error, Connect to Internet?");
                //dataDisplay.Text = "Error";
            }
        }

        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //var stopPoints = await _client.GetTest(inputBox.Text);


                //ResultListBox.Items.Clear();
                //for (int i = 0; i < stopPoints.Count; i++) 
                //{
                //    string lines = "";
                //    string children = "";

                //    for (int j = 0; j < stopPoints[i].Lines.Count; j++)
                //    {
                //        lines += $" {stopPoints[i].Lines[j].Name},";
                //    }

                //    for (int k = 0; k < stopPoints[i].Children.Count; k++)
                //    {
                //        children += $" {stopPoints[i].Children[k].CommonName},";
                //    }
                //    TestBox.Text += $"{i}: {stopPoints[i].CommonName} \n";
                //    ResultListBox.Items.Add($"Station Name: {stopPoints[i].CommonName}\nLines:{lines}\nChildren:{children}\n\n\n");

                var modes = await _client.GetModeNameAsync();

                for (var i = 0; i < modes.Count; i++)
                {
                    var routeSections = "";
                    for (var j = 0; j < modes[i].RouteSections.Count; j++)
                    {
                        routeSections += $"{modes[i].RouteSections[j].Name}";
                    }
                    ResultListBox.Items.Add(routeSections);
                }
            }
            catch (Exception ex)
            {
                TestBox.Text = ex.Message;
            }
        }

        private async void BtnRoute_Click(object sender, RoutedEventArgs e)
        {
            ResultListBox.Items.Clear();

            if (FromComboBox.SelectedItem == ToComboBox.SelectedItem)
            {
                ResultListBox.Items.Add("Nice");
            }
            else
            {
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
                }
            }
            //Get all lines and stations corresponding to lines
            //Method 1
            //Lon and lat find out what stations are closests, find out if they correspond to the lines that the closest stations are on then work form there
            //Mehtod 2
            //Use api that has the lines set in order All inbound. Deems distances from stations 

        }
    }
}