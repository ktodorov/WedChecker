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

#if WINDOWS_PHONE_APP
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using System;
using System.Threading.Tasks;
#elif WINDOWS_APP
using Bing.Maps;
using System;
using System.Threading.Tasks;
#endif

namespace WedChecker.UserControls
{
    public class MapView : Grid, INotifyPropertyChanged
    {
        private MapControl _map;
//#if WINDOWS_APP
//        private Map _map;
//        private MapShapeLayer _shapeLayer;
//        private MapLayer _pinLayer;
//#elif WINDOWS_PHONE_APP
        //private MapControl _map;
//#endif

        public BasicGeoposition PinnedPlace
        {
            get;
            set;
        }

        public MapView()
        {
////#if WINDOWS_APP
//            _map = new Map();

//            _shapeLayer = new MapShapeLayer();
//            _pinLayer = new MapLayer();
//            _map.ShapeLayers.Add(_shapeLayer);
//            _map.Children.Add(_pinLayer);

			//#elif WINDOWS_PHONE_APP

			_map = new MapControl();
			//#endif
			Loaded += async (sender, args) =>
            {
                try
                {
                    await CenterOnCurrentLocation();
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
//#if WINDOWS_APP
//                return _map.Center.ToGeopoint();
//#elif WINDOWS_PHONE_APP
                return _map.Center;
//#endif
            }
            set
            {
#if WINDOWS_APP
                _map.Center = value.ToLocation();
#elif WINDOWS_PHONE_APP
                _map.Center = value;
#endif

                OnPropertyChanged("Center");
            }
        }

        public string Credentials
        {
            get
            {
//#if WINDOWS_APP
//                return _map.Credentials;
//#elif WINDOWS_PHONE_APP
                return string.Empty;
//#endif
            }
            set
            {
#if WINDOWS_APP
                if (!string.IsNullOrEmpty(value))
                {
                    _map.Credentials = value;
                }
#endif

                OnPropertyChanged("Credentials");
            }
        }

        public string MapServiceToken
        {
            get
            {
//#if WINDOWS_APP
//                return string.Empty;
//#elif WINDOWS_PHONE_APP
                return _map.MapServiceToken;
//#endif
            }
            set
            {
#if WINDOWS_PHONE_APP
                if (!string.IsNullOrEmpty(value))
                {
                    _map.MapServiceToken = value;
                }
#endif

                OnPropertyChanged("MapServiceToken");
            }
        }

        public bool ShowTraffic
        {
            get
            {
//#if WINDOWS_APP
//                return _map.ShowTraffic;
//#elif WINDOWS_PHONE_APP
                return _map.TrafficFlowVisible;
//#endif
            }
            set
            {
#if WINDOWS_APP
                _map.ShowTraffic = value;
#elif WINDOWS_PHONE_APP
                _map.TrafficFlowVisible = value;
#endif

                OnPropertyChanged("ShowTraffic");
            }
        }

        public void SetView(BasicGeoposition center, double zoom)
        {
#if WINDOWS_APP
            _map.SetView(center.ToLocation(), zoom);
            OnPropertyChanged("Center");
            OnPropertyChanged("Zoom");
#elif WINDOWS_PHONE_APP
            _map.Center = new Geopoint(center);
            _map.ZoomLevel = zoom;
#endif
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
#if WINDOWS_APP
            var pin = new Pushpin()
            {
                Text = text
            };
            MapLayer.SetPosition(pin, location.ToLocation());
            _pinLayer.Children.Add(pin);
#elif WINDOWS_PHONE_APP
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
            grid.Children.Add(new TextBlock()
            {
                Text = text,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                Margin = new Thickness(3)
            });

            pin.Children.Add(grid);

            MapControl.SetLocation(pin, new Geopoint(location));
            _map.Children.Add(pin);

            PinnedPlace = location;
#endif
        }

        public void RemovePinnedLocation()
        {
#if WINDOWS_APP
            _pinLayer.Children.Clear();
#elif WINDOWS_PHONE_APP
            _map.Children.Clear();
#endif
        }
        public void ClearMap()
        {
#if WINDOWS_APP
            _shapeLayer.Shapes.Clear();
            _pinLayer.Children.Clear();
#elif WINDOWS_PHONE_APP
            _map.MapElements.Clear();
            _map.Children.Clear();
#endif
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
    }
}