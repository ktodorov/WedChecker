using System;
using System.Collections.Generic;
using WedChecker.UserControls.Elements;
using WedChecker.UserControls.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
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

        private static WedCheckerTitleBar _currentTitleBar;
        public static WedCheckerTitleBar CurrentTitleBar
        {
            get
            {
                return _currentTitleBar;
            }
            set
            {
                _currentTitleBar = value;
            }
        }

        public static void SetSetting(string key, object value)
        {
            //LocalSettings.Values[key] = value;
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
            if (!firstLaunch.HasValue)
            {
                firstLaunch = AppData.GetRoamingSetting<bool?>("HighPriority");
                return !firstLaunch.HasValue || firstLaunch.Value;
            }

            return firstLaunch.Value;
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

        public static void UpdateAppTheme(AppTheme theme)
        {
            Core.SetSetting("AppTheme", (int)theme);
        }

        public static AppTheme GetAppTheme()
        {
            var themeNumber = Core.GetSetting("AppTheme") as int?;

            if (themeNumber.HasValue)
            {
                return (AppTheme)themeNumber;
            }
            else
            {
                return AppTheme.SystemDefault;
            }
        }

        public static ElementTheme GetElementTheme()
        {
            var theme = GetAppTheme();

            switch (theme)
            {
                case AppTheme.Dark:
                    return ElementTheme.Dark;
                case AppTheme.Light:
                    return ElementTheme.Light;
                default:
                    return ElementTheme.Default;
            }
        }

        public static TaskCategories GetTaskCategory(Type taskType)
        {
            var result = TaskCategories.Home;

            if (taskType.FullName.StartsWith("WedChecker.UserControls.Tasks.Bookings"))
            {
                result = TaskCategories.Booking;
            }
            else if (taskType.FullName.StartsWith("WedChecker.UserControls.Tasks.Planings"))
            {
                result = TaskCategories.Planing;
            }
            else if (taskType.FullName.StartsWith("WedChecker.UserControls.Tasks.Purchases"))
            {
                result = TaskCategories.Purchase;
            }
            else
            {
                throw new Exception("Could not recognize the task type");
            }

            return result;
        }

        public static Hyperlink CreateNewHyperlink(string navigateUri, string text)
        {
            var currentAccentColorHex = Core.GetSystemAccentColor();

            // Navigate URI
            var hyperlinkText = new Hyperlink();
            hyperlinkText.NavigateUri = new Uri(navigateUri);

            // Inline text
            var line = new Run();
            line.Text = text;
            hyperlinkText.Inlines.Add(line);

            // Foreground
            SolidColorBrush backColor = new SolidColorBrush(currentAccentColorHex);
            hyperlinkText.Foreground = backColor;

            return hyperlinkText;
        }
    }
}
