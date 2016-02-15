using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WedChecker.Helpers;
using WedChecker.UserControls.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace WedChecker.Common
{
    public static class AppData
    {
        private const string EOS_CONST = "$WedChecker_EndOfStream$";
        private const string EOS_SEPARATOR = "$WedChecker_Separator$";
        public const string GLOBAL_SEPARATOR = "$WedChecker_GlobalAppDataSeparator$";

        private static Dictionary<string, object> _localStorage;
        public static Dictionary<string, object> LocalStorage
        {
            get
            {
                if (_localStorage == null)
                {
                    _localStorage = new Dictionary<string, object>();
                }
                return _localStorage;
            }
        }

        public static AddedTasks AllTasks = new AddedTasks();

        public static CancellationToken CancelToken
        {
            get;
            set;
        }

        public static bool ControlIsAdded(string control)
        {
            var populatedControls = AllTasks.GetAllTasks();

            return populatedControls.Contains(control);
        }

        public static object GetStorage(string key)
        {
            if (LocalStorage.ContainsKey(key))
            {
                return LocalStorage[key];
            }

            return null;
        }

        public static void SetStorage(string key, object value)
        {
            //if (LocalStorage.ContainsKey(key) && LocalStorage[key] == value)
            //{
            //    return;
            //}

            LocalStorage[key] = value;
        }

        public async static Task<List<BaseTaskControl>> PopulateAddedControls()
        {
            var controls = new List<BaseTaskControl>();

            var priorityControls = AllTasks.GetAllPriorityTasks();
            if (priorityControls != null)
            {
                foreach (var priorityControl in priorityControls)
                {
                    var baseTaskControl = AllTasks.GetTaskByTaskCode(priorityControl);
                    await baseTaskControl.DeserializeValues();
                    controls.Add(baseTaskControl);
                }
            }

            var nonPriorityControls = AllTasks.GetAllNonPriorityTasks();
            if (nonPriorityControls != null)
            {
                foreach (var nonPriorityControl in nonPriorityControls)
                {
                    var baseTaskControl = AllTasks.GetTaskByTaskCode(nonPriorityControl);
                    await baseTaskControl.DeserializeValues();
                    controls.Add(baseTaskControl);
                }
            }

            return controls;
        }

        public static void InsertRoamingSetting(string setting, object value)
        {
            if (value != null)
            {
                Core.RoamingSettings.Values[setting] = value;
            }
        }

        public static T GetRoamingSetting<T>(string setting)
        {
            if (Core.RoamingSettings.Values.ContainsKey(setting) && Core.RoamingSettings.Values[setting] is T)
            {
                return (T)Core.RoamingSettings.Values[setting];
            }

            return default(T);
        }
    }
}
