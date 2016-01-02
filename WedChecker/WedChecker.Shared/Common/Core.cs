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

            if (LocalSettings.Values.ContainsKey(key))
            {
                result = LocalSettings.Values[key];
            }
            else if (RoamingSettings.Values[key] != null)
            {
                result = RoamingSettings.Values[key];
                LocalSettings.Values.Add(key, result);
            }

            return result;
        }

        public static bool IsFirstLaunch()
        {
            var firstLaunch = GetSetting("first");
            return firstLaunch == null || (bool)firstLaunch == false;
        }

        public static Dictionary<string, object> GetPopulatedControls()
        {
            var populatedObjects = new Dictionary<string, object>();
            
            foreach(var taskControl in TaskData.TaskControls)
            {
                if (AppData.GetValue(taskControl) != null)
                {
                    // Then this task was populated
                    populatedObjects.Add(taskControl, AppData.GetValue(taskControl));
                }
            }
            
            return populatedObjects;
        }

        public static List<BaseTaskControl> GetPopulatedTaskControls()
        {
            var populatedObjects = new List<BaseTaskControl>();

            foreach (var taskControl in TaskData.TaskControls)
            {
                if (AppData.GetValue(taskControl) != null)
                {
                    // Then this task was populated
                    populatedObjects.Add(TaskData.GetTaskControlFromString(taskControl));
                }
            }

            return populatedObjects;
        }

        public static Color GetPhoneAccentBrush()
        {
            var phoneAccentBrush = new SolidColorBrush((App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color);

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
    }
}
