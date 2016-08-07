using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using System.Collections.Generic;
using Windows.UI;
using WedChecker.Common;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;

namespace WedChecker.UserControls
{
    public class MapView : Grid, INotifyPropertyChanged
    {
        private MapControl _map;

        public BasicGeoposition PinnedPlace
        {
            get;
            set;
        }

        public MapView()
        {
            _map = new MapControl();

            Loaded += async (sender, args) =>
            {
                try
                {
                    if (HasPinnedLocation())
                    {
                        CenterOnPinnedLocation();
                    }
                    else
                    {
                        await CenterOnCurrentLocation();
                    }
                    this.Children.Add(_map);
                }
                catch
                {
                }
            };
        }


        private void CommandHandler(IUICommand command)
        {
            var commandLabel = command.Label;
            switch (commandLabel)
            {
                case "Delete":
                    break;
                case "Cancel":
                    break;
            }
        }


        public double Zoom
        {
            get
            {
                return _map.ZoomLevel;
            }
            set
            {
                _map.ZoomLevel = value;
                OnPropertyChanged("Zoom");
            }
        }

        public Geopoint Center
        {
            get
            {
                return _map.Center;
            }
            set
            {
                _map.Center = value;

                OnPropertyChanged("Center");
            }
        }

        public string Credentials
        {
            get
            {
                return string.Empty;
            }
            set
            {
                OnPropertyChanged("Credentials");
            }
        }

        public string MapServiceToken
        {
            get
            {
                return _map.MapServiceToken;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _map.MapServiceToken = value;
                }

                OnPropertyChanged("MapServiceToken");
            }
        }

        public bool ShowTraffic
        {
            get
            {
                return _map.TrafficFlowVisible;
            }
            set
            {
                _map.TrafficFlowVisible = value;

                OnPropertyChanged("ShowTraffic");
            }
        }

        public void SetView(BasicGeoposition center, double zoom)
        {
            _map.Center = new Geopoint(center);
            _map.ZoomLevel = zoom;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        public void AddPushpin(BasicGeoposition location, string text)
        {
            RemovePinnedLocation();
            var pin = new StackPanel();
            pin.Orientation = Orientation.Vertical;

            var grid = new Grid()
            {
                Margin = new Thickness(-1, -1, 0, 0)
            };

            var rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.DodgerBlue),
            };

            var points = new PointCollection();
            points.Add(new Windows.Foundation.Point(0, 0));
            points.Add(new Windows.Foundation.Point(10, 10));
            points.Add(new Windows.Foundation.Point(0, 10));

            pin.Children.Add(new Polygon()
            {
                Points = points,
                Fill = new SolidColorBrush(Colors.DodgerBlue),
                Stroke = new SolidColorBrush(Colors.DodgerBlue)
            });

            grid.Children.Add(rectangle);

            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            stackPanel.Children.Add(new TextBlock()
            {
                Text = "\uE707",
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(3)
            });

            stackPanel.Children.Add(new TextBlock()
            {
                Text = text,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(3)
            });

            grid.Children.Add(stackPanel);

            pin.Children.Add(grid);

            MapControl.SetLocation(pin, new Geopoint(location));
            _map.Children.Add(pin);

            PinnedPlace = location;
        }

        public void RemovePinnedLocation()
        {
            _map.Children.Clear();
        }
        public void ClearMap()
        {
            _map.MapElements.Clear();
            _map.Children.Clear();
        }

        public async Task CenterOnCurrentLocation()
        {
            try
            {
                var gl = new Geolocator() { DesiredAccuracy = PositionAccuracy.High };
                Geoposition location = await gl.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));

                var basicGeoposition = new BasicGeoposition() { Latitude = location.Coordinate.Latitude, Longitude = location.Coordinate.Longitude };

                var locationGeopoint = new Geopoint(basicGeoposition);
                Center = locationGeopoint;
            }
            catch (UnauthorizedAccessException)
            {
                var msgDialog = new MessageDialog("Location service is turned off!", "Oops");

                UICommand okBtn = new UICommand("OK");
                msgDialog.Commands.Add(okBtn);

                await msgDialog.ShowAsync();
            }
        }

        public void CenterOnPinnedLocation()
        {
            var basicGeoposition = new BasicGeoposition() { Latitude = PinnedPlace.Latitude, Longitude = PinnedPlace.Longitude };
            var locationGeopoint = new Geopoint(basicGeoposition);
            Center = locationGeopoint;
        }

        public bool HasPinnedLocation()
        {
            return PinnedPlace.Latitude != 0 || PinnedPlace.Longitude != 0;
        }
    }
}