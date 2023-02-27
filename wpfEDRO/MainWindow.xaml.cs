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
using EDRouteOptimizer;

namespace wpfEDRO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string filename;
        private static EDRoute inputRoute;
        public MainWindow()
        {
            InitializeComponent();
            btnParseRoute.IsEnabled = false;
            dropdownSelectWaypoint.IsEnabled = false;
        }

        private void btnOpenRoute_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.InitialDirectory = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents");
            dialog.DefaultExt = ".route";
            dialog.Filter = ".route files (.route)|*.route";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                filename = dialog.FileName;
                btnParseRoute.IsEnabled = true;
            }


        }

        private void btnParseRoute_Click(object sender, RoutedEventArgs e)
        {

            inputRoute = EDRoute.ParseJson(filename);
            int currentDestination = inputRoute.CurrentDestination;
            string currentWaypoint = inputRoute.RouteWaypoints[currentDestination].SystemName;
            labelCurrentWaypointOutput.Text = currentWaypoint;
            setupDropDown();
        }

        private void setupDropDown()
        {
            dropdownSelectWaypoint.IsEnabled = true;
            dropdownSelectWaypoint.ItemsSource = inputRoute.RouteWaypoints;

        }

        private void dropdownSelectWaypoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EDSystem? coord = dropdownSelectWaypoint.SelectedItem as EDSystem;

            string coordString = coord != null ? coord.Coords.ToString() : "null";

            labelCoordinates.Text = coordString;
        }
    }
}
