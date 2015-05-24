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

        public static bool IsFirstLaunch()
        {
            return !RoamingSettings.Values.ContainsKey("first") || (bool)RoamingSettings.Values["first"] == false;
        }

        public static async Task StartUp()
        {
            await AppData.ReadDataFile();
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
