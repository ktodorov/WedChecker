using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class FreshFlowers : BaseTaskControl
    {
        private string FlowersNotes { get; set; } = string.Empty;

        public override string TaskName
        {
            get
            {
                return "Flowers";
            }
        }

        public FreshFlowers()
        {
            this.InitializeComponent();
        }

        public FreshFlowers(string value)
        {
            this.InitializeComponent();
            FlowersNotes = value;
        }

        public override void DisplayValues()
        {
            pinAdressButton.Visibility = Visibility.Collapsed;
            VerticalMapBorder.Visibility = Visibility.Collapsed;
            HorizontalMapBorder.Visibility = Visibility.Collapsed;

            tbFreshFlowersDisplay.Text = FlowersNotes;
            displayPanel.Visibility = Visibility.Visible;
            tbHeader.Text = "This is the flowers info you have noted";
            tbFreshFlowers.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            pinAdressButton.Visibility = Visibility.Visible;
            VerticalMapBorder.Visibility = Visibility.Visible;
            HorizontalMapBorder.Visibility = Visibility.Visible;

            tbFreshFlowers.Text = tbFreshFlowersDisplay.Text;
            tbFreshFlowers.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
            tbHeader.Text = "Here you can save any info about the flowers, you have planned for the wedding";
        }


        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.FreshFlowers.ToString());

            var objectsCount = GetObjectsCount();
            writer.Write(objectsCount);

            if (objectsCount == 1 || objectsCount == 2)
            {
                writer.Write(FlowersNotes);
            }
            if (objectsCount == -1 || objectsCount == 2)
            {
                writer.Write(locationMap.PinnedPlace.Latitude);
                writer.Write(locationMap.PinnedPlace.Longitude);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            if (objectsCount == 1 || objectsCount == 2)
            {
                FlowersNotes = reader.ReadString();
            }

            if (objectsCount == -1 || objectsCount == 2)
            {
                var latitude = reader.ReadDouble();
                var longitude = reader.ReadDouble();
                var basicGeoposition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };

                locationMap.AddPushpin(basicGeoposition, "1");
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            var decoration = tbFreshFlowers.Text;
            if (string.IsNullOrEmpty(decoration))
            {
                tbErrorMessage.Text = "Please, do not enter an empty flowers information";
                return;
            }

            if (FlowersNotes != decoration)
            {
                FlowersNotes = decoration;
                await AppData.InsertGlobalValue(TaskData.Tasks.FreshFlowers.ToString());
            }
        }

        private void pinAdressButton_Click(object sender, RoutedEventArgs e)
        {
            var location = GetCenteredLocation();

            locationMap.AddPushpin(location, "Flowers");
        }

        private BasicGeoposition GetCenteredLocation()
        {
            var center = locationMap.Center.Position;

            var location = new BasicGeoposition() { Latitude = center.Latitude, Longitude = center.Longitude };
            return location;
        }

        private void tbFreshFlowers_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbFreshFlowersDisplay.Text = tbFreshFlowers.Text;
        }

        private bool PinnedLocation()
        {
            return locationMap.PinnedPlace.Latitude != 0 || locationMap.PinnedPlace.Longitude != 0;
        }

        private async void centerLocationButton_Click(object sender, RoutedEventArgs e)
        {
            await locationMap.CenterOnCurrentLocation();
        }

        private void centerPinButton_Click(object sender, RoutedEventArgs e)
        {
            var basicGeoposition = new BasicGeoposition() { Latitude = locationMap.PinnedPlace.Latitude, Longitude = locationMap.PinnedPlace.Longitude };

            var locationGeopoint = new Geopoint(basicGeoposition);
            locationMap.Center = locationGeopoint;
        }

        int GetObjectsCount()
        {
            // ObjectsCount for serializing:
            // -1 - Only location
            //  1 - Only notes
            //  2 - Both
            var objectsCount = 0;

            if (!string.IsNullOrEmpty(FlowersNotes))
            {
                objectsCount = 1;
            }

            if (PinnedLocation())
            {
                objectsCount = -1;
            }

            if (PinnedLocation() && !string.IsNullOrEmpty(FlowersNotes))
            {
                objectsCount = 2;
            }

            return objectsCount;
        }

        private void showMapGrid_Click(object sender, RoutedEventArgs e)
        {
            if (mapGrid.Visibility == Visibility.Visible)
            {
                showMapGrid.Content = "Show map";
                mapGrid.Visibility = Visibility.Collapsed;
                mapControlsGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                showMapGrid.Content = "Hide map";
                mapGrid.Visibility = Visibility.Visible;
                mapControlsGrid.Visibility = Visibility.Visible;
            }
        }
    }
}
