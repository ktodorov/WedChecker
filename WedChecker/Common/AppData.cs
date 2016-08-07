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
	public enum AppTheme
	{
		Light = 0,
		Dark = 1,
		SystemDefault = 2
	}

    public static class AppData
    {
        public const string GLOBAL_SEPARATOR = "$WedChecker_GlobalAppDataSeparator$";
        public static string TextForShare;

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

        private static DateTime? _weddingDate;
        public static DateTime? WeddingDate
        {
            get
            {
                if (_weddingDate == null)
                {
                    var weddingDateString = GetRoamingSetting<string>("WeddingDate");
                    if (string.IsNullOrEmpty(weddingDateString))
                    {
                        return null;
                    }

                    try
                    {
                        _weddingDate = Convert.ToDateTime(weddingDateString);
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                }

                return _weddingDate;
            }
            set
            {
                _weddingDate = value;
            }
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
            LocalStorage[key] = value;
        }

        public async static Task<List<BaseTaskControl>> PopulateAddedControls(TaskCategories? category = null)
        {
            var controls = new List<BaseTaskControl>();

            AllTasks = new AddedTasks();

            var priorityControls = AllTasks.GetAllPriorityTasks(category);
            if (priorityControls != null)
            {
                foreach (var priorityControl in priorityControls)
                {
                    var baseTaskControl = AllTasks.GetTaskByTaskCode(priorityControl);
                    await baseTaskControl.DeserializeValues();
                    controls.Add(baseTaskControl);
                }
            }

            var nonPriorityControls = AllTasks.GetAllNonPriorityTasks(category);
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

        public static void InsertLocalSetting(string setting, object value)
        {
            if (value != null)
            {
                Core.LocalSettings.Values[setting] = value;
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

        public static T GetLocalSetting<T>(string setting)
        {
            if (Core.LocalSettings.Values.ContainsKey(setting) && Core.LocalSettings.Values[setting] is T)
            {
                return (T)Core.LocalSettings.Values[setting];
            }

            return default(T);
        }

        public static void RemoveRoamingSetting(string setting)
        {
            if (Core.RoamingSettings.Values.ContainsKey(setting))
            {
                Core.RoamingSettings.Values.Remove(setting);
            }
        }

        public static void RemoveLocalSetting(string setting)
        {
            if (Core.LocalSettings.Values.ContainsKey(setting))
            {
                Core.LocalSettings.Values.Remove(setting);
            }
        }
    }
}
