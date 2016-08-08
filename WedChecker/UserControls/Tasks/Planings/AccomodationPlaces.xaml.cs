using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
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

namespace WedChecker.UserControls.Tasks.Plannings
{
    public sealed partial class AccomodationPlaces : BaseTaskControl
    {
        private List<string> Places { get; set; } = new List<string>();

        private bool PlacesChanged = false;

        public override string TaskName
        {
            get
            {
                return "Accomodation places";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "You can add your accomodation places for the guests here";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "These are your accomodation places so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.AccomodationPlaces.ToString();
            }
        }

        public AccomodationPlaces()
        {
            this.InitializeComponent();
        }

        public AccomodationPlaces(List<string> values)
        {
            this.InitializeComponent();
            Places = values;
            PlacesChanged = false;
        }
        public override void DisplayValues()
        {
            foreach (var place in spPlaces.Children.OfType<ElementControl>())
            {
                place.DisplayValues();
            }
            addPlaceButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            foreach (var place in spPlaces.Children.OfType<ElementControl>())
            {
                place.EditValues();
            }
            addPlaceButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            foreach (var place in spPlaces.Children.OfType<ElementControl>())
            {
                SavePlace(place.Title);
            }

            Places.RemoveAll(p => string.IsNullOrEmpty(p));

            writer.Write(Places.Count);
            foreach (var place in Places)
            {
                writer.Write(place);
            }

            PlacesChanged = false;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Places = new List<string>();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                var place = reader.ReadString();
                AddPlace(place);
            }
        }

        protected override void SetLocalStorage()
        {
            AppData.SetStorage("AccomodationPlaces", Places);
        }

        private void SavePlace(string place)
        {
            if (!Places.Contains(place))
            {
                Places.Add(place);
                PlacesChanged = true;
            }
        }

        private void addPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            AddPlace();
            PlacesChanged = true;
        }

        private void AddPlace(string title = "")
        {
            if (string.IsNullOrEmpty(title))
            {
                title = string.Empty;
            }
            else
            {
                Places.Add(title);
            }

            var newPlace = new ElementControl(title);

            newPlace.removeElementButton.Click += removePlaceButton_Click;
            spPlaces.Children.Insert(newPlace.Number, newPlace);
            PlacesChanged = true;
        }

        private void savePlaceButton_Click(object sender, RoutedEventArgs e)
        {
            var place = ((sender as Button).Parent as Grid).Parent as ElementControl;
            SavePlace(place.Title);

            place.DisplayValues();
        }

        private void removePlaceButton_Click(object sender, RoutedEventArgs e)
        {
            var place = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (Places != null)
            {
                Places.Remove(place.Title);
            }

            spPlaces.Children.Remove(place);

            UpdatePlacesNumbers();
        }

        private void UpdatePlacesNumbers()
        {
            var placeControls = spPlaces.Children.OfType<ElementControl>();
            var i = -1;
            foreach (var placeControl in placeControls)
            {
                i++;
                if (placeControl.Number != i)
                {
                    spPlaces.Children.RemoveAt(i);
                    placeControl.Number = i;
                    spPlaces.Children.Insert(i, placeControl);
                }
            }
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            foreach (var place in Places)
            {
                sb.AppendLine(place);
            }
        }
    }
}
