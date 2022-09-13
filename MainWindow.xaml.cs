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

            var line = await data.GetLineId(Convert.ToInt32(inputBox.Text));
            var destinationName = await data.GetDestinationName(Convert.ToInt32(inputBox.Text));
            var expectedArrival = await data.GetLineArrivals(Convert.ToInt32(inputBox.Text));
            dataDisplay.AppendText($"Line: {line}\nDestination: {destinationName}\nExpected Arrival: {expectedArrival}\n");
        }
    }
}
