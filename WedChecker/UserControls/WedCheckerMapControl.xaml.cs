using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Interfaces;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class WedCheckerMapControl : UserControl, IStorableTask
    {
        private string _pinName;
        public string PinName
        {
            get
            {
                if (_pinName == null)
                {
                    _pinName = "1";
                }

                return _pinName;
            }
            set
            {
                _pinName = value;
            }
        }

        public WedCheckerMapControl()
        {
            this.InitializeComponent();
        }

        public void DisplayValues()
        {
            pinAdressButton.Visibility = Visibility.Collapsed;
            VerticalMapBorder.Visibility = Visibility.Collapsed;
            HorizontalMapBorder.Visibility = Visibility.Collapsed;
        }

        public void EditValues()
        {
            pinAdressButton.Visibility = Visibility.Visible;
            VerticalMapBorder.Visibility = Visibility.Visible;
            HorizontalMapBorder.Visibility = Visibility.Visible;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(locationMap.PinnedPlace.Latitude);
            writer.Write(locationMap.PinnedPlace.Longitude);
        }

        public async Task Deserialize(BinaryReader reader)
        {
            var latitude = reader.ReadDouble();
            var longitude = reader.ReadDouble();
            var basicGeoposition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };

            locationMap.AddPushpin(basicGeoposition, PinName);
        }

        private BasicGeoposition GetCenteredLocation()
        {
            var center = locationMap.Center.Position;

            var location = new BasicGeoposition() { Latitude = center.Latitude, Longitude = center.Longitude };
            return location;
        }

        public bool HasPinnedLocation()
        {
            return locationMap.HasPinnedLocation();
        }

        private void pinAdressButton_Click(object sender, RoutedEventArgs e)
        {
            var location = GetCenteredLocation();

            locationMap.AddPushpin(location, PinName);
        }

        private async void centerLocationButton_Click(object sender, RoutedEventArgs e)
        {
            centerMapProgressRing.IsActive = true;
            centerLocationButton.IsEnabled = false;
            centerPinButton.IsEnabled = false;
            pinAdressButton.IsEnabled = false;

            await locationMap.CenterOnCurrentLocation();

            centerLocationButton.IsEnabled = true;
            centerPinButton.IsEnabled = true;
            pinAdressButton.IsEnabled = true;
            centerMapProgressRing.IsActive = false;
        }

        private void centerPinButton_Click(object sender, RoutedEventArgs e)
        {
            locationMap.CenterOnPinnedLocation();
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

		private void zoomOutButton_Click(object sender, RoutedEventArgs e)
		{
			locationMap.Zoom -= 0.3;
		}

		private void zoomInButton_Click(object sender, RoutedEventArgs e)
		{
			locationMap.Zoom += 0.3;
		}

        public string GetDataAsText()
        {
            var sb = new StringBuilder();

            if (HasPinnedLocation())
            {
                sb.AppendLine($" - Latitude: {locationMap.PinnedPlace.Latitude}");
                sb.AppendLine($" - Longitude: {locationMap.PinnedPlace.Longitude}");
            }

            return sb.ToString();
        }
    }
}
