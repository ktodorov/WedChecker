﻿using System;
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

        private string RegistryNotes
        {
            get;
            set;
        }

        public RegistryPlace()
        {
            this.InitializeComponent();
        }

        public RegistryPlace(string value)
        {
            this.InitializeComponent();
            RegistryNotes = value;
            DisplayValues();
        }

        public override void DisplayValues()
        {
            pinAdressButton.Visibility = Visibility.Collapsed;
            VerticalMapBorder.Visibility = Visibility.Collapsed;
            HorizontalMapBorder.Visibility = Visibility.Collapsed;

            tbRegistryNotesDisplay.Text = RegistryNotes ?? string.Empty;
            displayPanel.Visibility = Visibility.Visible;
            tbHeader.Text = "These are your notes";
            registryPickerButton.Visibility = Visibility.Collapsed;
            tbRegistryNotes.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            pinAdressButton.Visibility = Visibility.Visible;
            VerticalMapBorder.Visibility = Visibility.Visible;
            HorizontalMapBorder.Visibility = Visibility.Visible;

            tbRegistryNotes.Text = tbRegistryNotesDisplay.Text;
            tbRegistryNotes.Visibility = Visibility.Visible;
            tbHeader.Text = "Here you can add address or notes\nor whatever you like for your registry place";
            registryPickerButton.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.RegistryPlace.ToString());

            var objectsCount = GetObjectsCount();
            writer.Write(objectsCount);

            if (objectsCount == 1 || objectsCount == 2)
            {
                writer.Write(RegistryNotes);
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
                RegistryNotes = reader.ReadString();
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

            if (!string.IsNullOrEmpty(RegistryNotes))
            {
                objectsCount = 1;
            }

            if (PinnedLocation())
            {
                objectsCount = -1;
            }

            if (PinnedLocation() && !string.IsNullOrEmpty(RegistryNotes))
            {
                objectsCount = 2;
            }

            return objectsCount;
        }

        private async void pinAdressButton_Click(object sender, RoutedEventArgs e)
        {
            var location = GetCenteredLocation();

            locationMap.AddPushpin(location, "Registry");
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

        private void centerLocationButton_Click(object sender, RoutedEventArgs e)
        {
            locationMap.CenterOnCurrentLocation();
        }

        private void centerPinButton_Click(object sender, RoutedEventArgs e)
        {
            var basicGeoposition = new BasicGeoposition() { Latitude = locationMap.PinnedPlace.Latitude, Longitude = locationMap.PinnedPlace.Longitude };

            var locationGeopoint = new Geopoint(basicGeoposition);
            locationMap.Center = locationGeopoint;
        }

        private async void registryPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var registryNotes = tbRegistryNotes.Text;

            if (RegistryNotes != registryNotes)
            {
                RegistryNotes = registryNotes;
                await AppData.InsertGlobalValue(TaskData.Tasks.RegistryPlace.ToString(), registryNotes);
            }
            DisplayValues();
        }

        private void tbRegistryNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbRegistryNotesDisplay.Text = tbRegistryNotes.Text;
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
