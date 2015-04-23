using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace WedChecker.Common
{
    public static class Core
    {
        public static ApplicationDataContainer Settings
        {
            get
            {
                return Windows.Storage.ApplicationData.Current.LocalSettings;
            }
        }

        public static bool IsFirstLaunch()
        {
            return !Settings.Values.ContainsKey("first");
        }

        public static void StartUp()
        {
            if (IsFirstLaunch())
            {
                Settings.Values["first"] = true;
            }
        }
    }
}
