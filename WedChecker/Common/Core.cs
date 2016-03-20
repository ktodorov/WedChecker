using System.Collections.Generic;
using WedChecker.UserControls.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace WedChecker.Common
{
    public static class Core
    {
        public static ApplicationDataContainer LocalSettings
        {
            get
            {
                return ApplicationData.Current.LocalSettings;
            }
        }

        public static StorageFolder LocalFolder
        {
            get
            {
                return ApplicationData.Current.LocalFolder;
            }
        }

        public static ApplicationDataContainer RoamingSettings
        {
            get
            {
                return ApplicationData.Current.RoamingSettings;
            }
        }

        public static StorageFolder RoamingFolder
        {
            get
            {
                return ApplicationData.Current.RoamingFolder;
            }
        }

        public static void SetSetting(string key, object value)
        {
            LocalSettings.Values[key] = value;
            RoamingSettings.Values[key] = value;
        }

        public static object GetSetting(string key)
        {
            object result = null;

            if (RoamingSettings.Values.ContainsKey(key))
            {
                result = RoamingSettings.Values[key];
            }

            return result;
        }

        public static bool IsFirstLaunch()
        {
            var firstLaunch = AppData.GetRoamingSetting<bool?>("FirstLaunch");
            return !firstLaunch.HasValue || firstLaunch.Value;
        }

        public static Color GetSystemAccentColor()
        {
            //App.Current.Resources.Values
            var phoneAccentBrush = new SolidColorBrush((App.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color);

            return phoneAccentBrush.Color;
        }

        public static bool UserIsOnWiFi()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

            if (connectionProfile != null && connectionProfile.IsWlanConnectionProfile)
            {
                return true;
            }

            return false;
        }

        public static bool CanRoamData()
        {
            var wiFiOnlyValue = AppData.GetRoamingSetting<bool?>("WiFiOnlySync"); //AppData.GetValue("wifiOnlySync");
            var wiFiOnly = wiFiOnlyValue.HasValue && wiFiOnlyValue.Value;
            if ((wiFiOnly && UserIsOnWiFi()) || !wiFiOnly)
            {
                return true;
            }

            return false;
        }
    }
}
