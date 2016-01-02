using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

        private static Dictionary<string, object> _localAppData;
        public static Dictionary<string, object> LocalAppData
        {
            get
            {
                if (_localAppData == null)
                {
                    _localAppData = new Dictionary<string, object>();
                }
                return _localAppData;
            }
        }

        private static Dictionary<string, string> GlobalAppData { get; set; } = new Dictionary<string, string>();

        public static CancellationToken CancelToken
        {
            get;
            set;
        }

        public static void PopulateAppData()
        {
        }

        private static List<BaseTaskControl> SerializableTasks
        {
            get;
            set;
        }

        public static string EncodeDataToString()
        {
            if (GlobalAppData == null)
            {
                GlobalAppData = new Dictionary<string, string>();
            }

            if (GlobalAppData.Count == 0)
            {
                PopulateAppData();
            }
            var result = string.Empty;

            foreach (var key in GlobalAppData.Keys)
            {
                result += string.Format("<<{0}><{1}>>", key, GlobalAppData[key]);
            }

            return result;
        }

        public static void DecodeDataFromString(string dataFile)
        {
            if (GlobalAppData == null)
            {
                GlobalAppData = new Dictionary<string, string>();
            }

            var index = 0;
            string key = "", value = "";
            for (int i = 0; i < dataFile.Length; i++)
            {
                if (dataFile[i] == '<' || dataFile[i] == '>')
                {
                    index++;
                }
                else if (index < 3) // then it's key
                {
                    key += dataFile[i];
                }
                else if (index < 6) // then it's value
                {
                    value += dataFile[i];
                }

                if (index == 6)
                {
                    GlobalAppData[key] = value;
                    index = 0;
                    key = "";
                    value = "";
                }
            }
        }

        public static string GetValue(string key)
        {
            if (GlobalAppData == null || GlobalAppData.Count == 0)
            {
                GlobalAppData = new Dictionary<string, string>();
                PopulateAppData();
            }

            if (GlobalAppData.ContainsKey(key))
            {
                return GlobalAppData[key];
            }

            return null;
        }

        public static async Task InsertGlobalValue(string name, string value = "serialized", bool serialize = true)
        {
            GlobalAppData[name] = value;

            if (serialize)
            {
                await SerializeData();
            }
        }

        public static async Task RemoveGlobalValue(string name, bool serialize = true)
        {
            if (GlobalAppData.ContainsKey(name))
            {
                GlobalAppData.Remove(name);

                if (serialize)
                {
                    await SerializeData();
                }
            }
        }

        public static async Task InsertGlobalValues(string name, List<string> values, bool serialize = true)
        {
            var key = string.Empty;
            foreach (var value in values)
            {
                key += $"{value}{EOS_SEPARATOR}";
            }

            GlobalAppData[name] = key;

            if (serialize)
            {
                await SerializeData();
            }
        }

        public static List<string> GetGlobalValues(string name)
        {
            if (!GlobalAppData.ContainsKey(name))
            {
                GlobalAppData[name] = string.Empty;
            }

            var key = GlobalAppData[name];

            var values = key.Split(new string[] { EOS_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return values;
        }

        public static async Task SerializeData(CancellationTokenSource ctsToUse = null, bool serializeToRoaming = false)
        {
            var wiFiOnlyValue = GetValue("wifiOnlySync");
            var wiFiOnly = wiFiOnlyValue != null && bool.Parse(wiFiOnlyValue);
            if (wiFiOnly && !Core.UserIsOnWiFi())
            {
                serializeToRoaming = false;
            }

            CancellationToken ctToUse;
            if (ctsToUse == null)
            {
                ctToUse = CancelToken;
            }
            else
            {
                ctToUse = ctsToUse.Token;
            }

            var fileName = "dataFileSerialized.dat";

            StorageFolder folder = Core.LocalFolder;

            if (serializeToRoaming)
            {
                folder = Core.RoamingFolder;
            }

            StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask(ctToUse);

            if ((file.DateCreated - DateTime.Now).TotalMinutes > 1)
            {
                return;
            }

            if (serializeToRoaming)
            {
                Core.RoamingSettings.Values["SerializedOn"] = DateTime.Now.ToUniversalTime().ToString();
                if (SerializableTasks != null)
                {
                    Core.RoamingSettings.Values["TasksCount"] = SerializableTasks.Count;
                }
            }
            else
            {
                Core.LocalSettings.Values["SerializedOn"] = DateTime.Now.ToUniversalTime().ToString();
                if (SerializableTasks != null)
                {
                    Core.LocalSettings.Values["TasksCount"] = SerializableTasks.Count;
                }
            }

            var populatedControls = Core.GetPopulatedTaskControls();

            try
            {
                using (Stream stream = await file.OpenStreamForWriteAsync())
                using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Compress, true))
                using (BinaryWriter writer = new BinaryWriter(compressionStream, Encoding.Unicode, true))
                {
                    ctToUse.ThrowIfCancellationRequested();

                    SerializeAppData(writer);

                    if (SerializableTasks == null)
                    {
                        SerializableTasks = new List<BaseTaskControl>();
                    }

                    foreach (var serializableTask in SerializableTasks)
                    {
                        serializableTask.Serialize(writer);
                    }

                    writer.Write(EOS_CONST);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public static async Task<List<BaseTaskControl>> DeserializeData(bool deserializeFromRoaming = false)
        {
            var wiFiOnlyValue = GetValue("wifiOnlySync");
            var wiFiOnly = wiFiOnlyValue != null && bool.Parse(wiFiOnlyValue);
            if (wiFiOnly && !Core.UserIsOnWiFi())
            {
                deserializeFromRoaming = false;
            }
            else
            {
                if (!deserializeFromRoaming &&
                    Core.RoamingSettings.Values.ContainsKey("SerializedOn") &&
                    Core.RoamingSettings.Values.ContainsKey("TasksCount") &&
                    Core.LocalSettings.Values.ContainsKey("SerializedOn") &&
                    Core.LocalSettings.Values.ContainsKey("TasksCount"))
                {
                    var roamingSerializedOn = Core.RoamingSettings.Values["SerializedOn"] as string;
                    var roamingTasksCount = Core.RoamingSettings.Values["TasksCount"] as int?;
                    var localSerializedOn = Core.LocalSettings.Values["SerializedOn"] as string;
                    var localTasksCount = Core.LocalSettings.Values["TasksCount"] as int?;

                    if (roamingTasksCount != null && roamingSerializedOn != null &&
                        localTasksCount != null && localSerializedOn != null)
                    {
                        var roamingSerializedOnDate = DateTime.Parse(roamingSerializedOn);
                        var localSerializedOnDate = DateTime.Parse(localSerializedOn);

                        if ((roamingSerializedOnDate - localSerializedOnDate).Milliseconds > 0 ||
                            ((roamingSerializedOnDate - localSerializedOnDate).Minutes < 0 &&
                              roamingTasksCount.Value - localTasksCount.Value > 3))
                        {
                            deserializeFromRoaming = true;
                        }
                    }
                }
                else if (!deserializeFromRoaming &&
                        Core.RoamingSettings.Values.ContainsKey("SerializedOn") &&
                        Core.RoamingSettings.Values.ContainsKey("TasksCount"))
                {
                    deserializeFromRoaming = true;
                }
            }

            var fileName = "dataFileSerialized.dat";

            StorageFolder folder = Core.LocalFolder;

            if (deserializeFromRoaming)
            {
                folder = Core.RoamingFolder;
            }

            var addedControls = new List<BaseTaskControl>();

            bool fileAvailable = true;

            try
            {
                StorageFile file = await folder.GetFileAsync(fileName).AsTask(CancelToken);

                using (Stream stream = await file.OpenStreamForReadAsync())
                using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                using (BinaryReader reader = new BinaryReader(decompressionStream, Encoding.Unicode, true))
                {
                    try
                    {
                        while (true)
                        {
                            CancelToken.ThrowIfCancellationRequested();

                            var control = reader.ReadString();

                            if (control == null || control == EOS_CONST)
                            {
                                return addedControls;
                            }

                            if (control == "AppData")
                            {
                                DeserializeAppData(reader);
                                continue;
                            }

                            foreach (var taskControl in TaskData.TaskControls)
                            {
                                if (taskControl == control)
                                {
                                    var baseTaskControl = TaskData.GetTaskControlFromString(taskControl);
                                    await InsertGlobalValue(baseTaskControl.GetType().ToString(), "true", false);
                                    baseTaskControl.Deserialize(reader);
                                    addedControls.Add(baseTaskControl);
                                    break;
                                }
                            }
                        }
                    }
                    // Then we have reached end of stream and failed to return before that
                    catch (EndOfStreamException)
                    {
                        return addedControls;
                    }
                }
            }
            catch (FileNotFoundException) // The file is not created
            {
                fileAvailable = false;
            }
            catch (EndOfStreamException)
            {
                return addedControls;
            }
            catch (OperationCanceledException)
            {
                return null;
            }

            if (!fileAvailable)
            {
                await SerializeData(serializeToRoaming: deserializeFromRoaming);
                await DeserializeData(deserializeFromRoaming);
            }

            return addedControls;
        }

        private static void SerializeAppData(BinaryWriter writer)
        {
            writer.Write("AppData");
            writer.Write(GlobalAppData.Keys.Count);

            foreach (var key in GlobalAppData.Keys)
            {
                writer.Write(key);
                writer.Write(GlobalAppData[key]);
            }
        }

        private static void DeserializeAppData(BinaryReader reader)
        {
            var appDataSize = reader.ReadInt32();
            var pos = 0;
            while (pos < appDataSize)
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                InsertGlobalValue(key, value, false);
                pos++;
            }
        }

        public static void InsertSerializableTask(BaseTaskControl baseTaskControl)
        {
            if (SerializableTasks == null)
            {
                SerializableTasks = new List<BaseTaskControl>();
            }

            SerializableTasks.Add(baseTaskControl);
        }

        public async static Task DeleteSerializableTask(BaseTaskControl baseTaskControl, bool serialize = true)
        {
            if (SerializableTasks != null && SerializableTasks.Contains(baseTaskControl))
            {
                SerializableTasks.Remove(baseTaskControl);
            }

            if (serialize)
            {
                await SerializeData();
            }
        }
    }
}
