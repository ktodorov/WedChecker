using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class RegistryPlace : BaseTaskControl
    {
        public override string TaskName
        {
            get
            {
                return "Registry place";
            }
        }

        public RegistryPlace()
        {
            this.InitializeComponent();
        }

        public RegistryPlace(string value)
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            pinAdressButton.Visibility = Visibility.Collapsed;
            VerticalMapBorder.Visibility = Visibility.Collapsed;
            HorizontalMapBorder.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            pinAdressButton.Visibility = Visibility.Visible;
            VerticalMapBorder.Visibility = Visibility.Visible;
            HorizontalMapBorder.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.RegistryPlace);

            writer.Write(locationMap.pinnedPlace.Latitude);
            writer.Write(locationMap.pinnedPlace.Longitude);
        }

        public override BaseTaskControl Deserialize(BinaryReader reader)
        {
            var registryPlace = new RegistryPlace();

            var latitude = reader.ReadDouble();
            var longitude = reader.ReadDouble();
            var basicGeoposition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };

            registryPlace.locationMap.AddPushpin(basicGeoposition, "1");

            return registryPlace;
        }

        private async void pinAdressButton_Click(object sender, RoutedEventArgs e)
        {
            var location = GetCenteredLocation();

            locationMap.AddPushpin(location, "Registry");
            await AppData.InsertGlobalValue("RegistryLocation", location.Latitude.ToString() + ";" + location.Longitude.ToString());
            DisplayValues();
        }

        private BasicGeoposition GetCenteredLocation()
        {
            var center = locationMap.Center.Position;
            
            var location = new BasicGeoposition() { Latitude = center.Latitude, Longitude = center.Longitude };
            return location;
        }
    }
}
