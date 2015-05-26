using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        }

        public override void EditValues()
        {

        }

        public override void Serialize(BinaryWriter writer)
        {

        }

        public override BaseTaskControl Deserialize(BinaryReader reader)
        {
            return this;
        }

        private void pinAdressButton_Click(object sender, RoutedEventArgs e)
        {
            var location = GetCenteredLocation();

            locationMap.AddPushpin(location, "Registry");
        }

        private BasicGeoposition GetCenteredLocation()
        {
            var center = locationMap.Center.Position;

            var rand = new Random();
            center.Latitude += rand.NextDouble() * 0.05 - 0.025;
            center.Longitude += rand.NextDouble() * 0.05 - 0.025;

            var location = new BasicGeoposition() { Latitude = center.Latitude, Longitude = center.Longitude };
            return location;
        }
    }
}
