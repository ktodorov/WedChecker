using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WedChecker.UserControls.Tasks;
using Windows.Storage;

namespace WedChecker.Common
{
    public static class Core
    {
        public static ApplicationDataContainer LocalSettings
        {
            get
            {
                return Windows.Storage.ApplicationData.Current.LocalSettings;
            }
        }

        public static Windows.Storage.StorageFolder LocalFolder
        {
            get
            {
                return Windows.Storage.ApplicationData.Current.LocalFolder;
            }
        }

        public static ApplicationDataContainer RoamingSettings
        {
            get
            {
                return Windows.Storage.ApplicationData.Current.RoamingSettings;
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

            if (LocalSettings.Values[key] != null)
            {
                result = LocalSettings.Values[key];
            }
            else if (RoamingSettings.Values[key] != null)
            {
                result = RoamingSettings.Values[key];
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
    }
}
