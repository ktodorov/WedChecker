using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
            return !RoamingSettings.Values.ContainsKey("first");
        }

        public static async Task StartUp()
        {
            await AppData.ReadDataFile();
        }

    }
}
