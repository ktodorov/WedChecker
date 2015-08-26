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
    public sealed partial class HoneymoonDestination : BaseTaskControl
    {
        private string HoneymoonNotes
        {
            get;
            set;
        }

        public override string TaskName
        {
            get
            {
                return "Honeymoon destination";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can add address or some notes about your planned honeymoon destination";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "Here is what you have about your honeymoon";
            }
        }

        public HoneymoonDestination()
        {
            this.InitializeComponent();
        }

        public HoneymoonDestination(string value)
        {
            this.InitializeComponent();
            HoneymoonNotes = value;
        }

        public override void DisplayValues()
        {
            pinAdressButton.Visibility = Visibility.Collapsed;
            VerticalMapBorder.Visibility = Visibility.Collapsed;
            HorizontalMapBorder.Visibility = Visibility.Collapsed;

            tbHoneymoonNotesDisplay.Text = HoneymoonNotes ?? string.Empty;
            displayPanel.Visibility = Visibility.Visible;
            tbHoneymoonNotes.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            pinAdressButton.Visibility = Visibility.Visible;
            VerticalMapBorder.Visibility = Visibility.Visible;
            HorizontalMapBorder.Visibility = Visibility.Visible;

            tbHoneymoonNotes.Text = tbHoneymoonNotesDisplay.Text;
            tbHoneymoonNotes.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.HoneymoonDestination.ToString());

            var objectsCount = GetObjectsCount();
            writer.Write(objectsCount);

            if (objectsCount == 1 || objectsCount == 2)
            {
                writer.Write(HoneymoonNotes);
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
                HoneymoonNotes = reader.ReadString();
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

        int GetObjectsCount()
        {
            // ObjectsCount for serializing:
            // -1 - Only location
            //  1 - Only notes
            //  2 - Both
            var objectsCount = 0;

            if (!string.IsNullOrEmpty(HoneymoonNotes))
            {
                objectsCount = 1;
            }

            if (PinnedLocation())
            {
                objectsCount = -1;
            }

            if (PinnedLocation() && !string.IsNullOrEmpty(HoneymoonNotes))
            {
                objectsCount = 2;
            }

            return objectsCount;
        }

        private void pinAdressButton_Click(object sender, RoutedEventArgs e)
        {
            var location = GetCenteredLocation();

            locationMap.AddPushpin(location, "Honeymoon");
        }

        private BasicGeoposition GetCenteredLocation()
        {
            var center = locationMap.Center.Position;

            var location = new BasicGeoposition() { Latitude = center.Latitude, Longitude = center.Longitude };
            return location;
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

        public override async Task SubmitValues()
        {
            var registryNotes = tbHoneymoonNotes.Text;

            if (HoneymoonNotes != registryNotes)
            {
                HoneymoonNotes = registryNotes;
                await AppData.InsertGlobalValue(TaskData.Tasks.HoneymoonDestination.ToString());
            }
        }

        private void tbHoneymoonNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbHoneymoonNotesDisplay.Text = tbHoneymoonNotes.Text;
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
