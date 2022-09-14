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
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnInput_Click(object sender, RoutedEventArgs e)
        {
            TflData data = new TflData();

            var lineName = await data.GetName(Convert.ToInt32(inputBox.Text));
            var lineMode = await data.GetModeName(Convert.ToInt32(inputBox.Text));
            var currentLocation = await data.GetCurrentLocation(Convert.ToInt32(inputBox.Text));
            var destinationName = await data.GetDestinationName(Convert.ToInt32(inputBox.Text));
            var expectedArrival = await data.GetLineArrivals(Convert.ToInt32(inputBox.Text));

            dataDisplay.Text = ($"Line: {lineName} - Mode: {lineMode}\nCurrent Location: {currentLocation}\nDestination: {destinationName}\nExpected Arrival: {expectedArrival}\n");
        }
    }
}
