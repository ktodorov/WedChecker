#if WINDOWS_APP
using Bing.Maps;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
#endif


using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WedChecker.Common
{
    public class TaskListItem
    {
        public string Title;
        public string TaskName;
    }

    public static class Extensions
    {
#if WINDOWS_APP

        public static LocationCollection ToLocationCollection(this IList<BasicGeoposition> pointList)
        {
            var locs = new LocationCollection();

            foreach (var p in pointList)
            {
                locs.Add(p.ToLocation());
            }

            return locs;
        }

        public static Geopoint ToGeopoint(this Location location)
        {
            return new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude });
        }

        public static Location ToLocation(this Geopoint location)
        {
            return new Location(location.Position.Latitude, location.Position.Longitude);
        }

        public static Location ToLocation(this BasicGeoposition location)
        {
            return new Location(location.Latitude, location.Longitude);
        }

#elif WINDOWS_PHONE_APP

        //Add any required Windows Phone Extensions

#endif

        public static FrameworkElement FindAncestorByType(this FrameworkElement element, Type elementType)
        {
            var parent = element.Parent as FrameworkElement;

            while (parent != null && parent.GetType() != elementType)
            {
                parent = parent.Parent as FrameworkElement;
            }

            return parent;
        }

		public static FrameworkElement FindAncestorByName(this FrameworkElement element, string elementName)
		{
			var parent = element.Parent as FrameworkElement;

			while (parent != null && parent.Name != elementName)
			{
				parent = parent.Parent as FrameworkElement;
			}

			return parent;
		}
	}
}
