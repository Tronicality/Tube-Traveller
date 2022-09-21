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
using Tube_Traveller;

namespace Tube_Traveller
{
    public partial class MainWindow : Window
    {
        TflClient _client;

        public MainWindow()
        {
            InitializeComponent();
            _client = new TflClient();
            LoadTrains();

        }

        private void LoadTrains()
        {

        }

        private async void btnInput_Click(object sender, RoutedEventArgs e)
        {
            //dataDisplay.Items[0] = "";
            dataDisplay.Items.Clear();
            try
            {
                var datas = await _client.GetArrivalsAsync(inputBox.Text);
                if (datas.Count == 0)
                {
                    dataDisplay.Items[0] = "Nothing was found";
                }
                else
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        dataDisplay.Items.Add($"Line: {datas[i].LineName} - Mode: {datas[i].ModeName}\nStation Name: {datas[i].StationName} - Current Location: {datas[i].CurrentLocation}\nDestination: {datas[i].DestinationName}\nExpected Arrival: {datas[i].ExpectedArrival.AddHours(1)}\n\n\n");
                    }   
                }
            }
            catch (Exception)
            {
                dataDisplay.Items[0] = "Error";
                //dataDisplay.Text = "Error";
            }
        }

        private async void btnGetTubeModes_Click(object sender, RoutedEventArgs e)
        {
            dataDisplay.Items.Clear();
            var modes = await _client.GetAllLinesByModeAsync("tube");
            var tubeModes = modes.Where(x => x.ModeName == "tube").Select(x => x.LineId);

            dataDisplay.Items.Add(tubeModes);

            //dataDisplay.Text = string.Join(" ", tubeModes);
        }
    }
}