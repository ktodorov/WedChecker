﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        private static Dictionary<string, string> LocalAppData
        {
            get;
            set;
        }

        public static CancellationToken CancelToken
        {
            get;
            set;
        }

        public static void PopulateAppData()
        {
            LocalAppData["firstLaunchFirstHeader"] = "Hello and welcome to";
            LocalAppData["firstLaunchFirstTitle"] = "WedChecker";
            LocalAppData["firstLaunchFirstDialog"] = "No doubt we will make a wonderful wedding.\nCan I know your name first?\nIt will help me to know you better.";
            LocalAppData["firstLaunchSecondDialog"] = "Great!\n Okay, when is your wedding?";
        }

        private static List<BaseTaskControl> SerializableTasks
        {
            get;
            set;
        }

        public static string EncodeDataToString()
        {
            if (LocalAppData == null)
            {
                LocalAppData = new Dictionary<string, string>();
            }

            if (LocalAppData.Count == 0)
            {
                PopulateAppData();
            }
            var result = string.Empty;

            foreach (var key in LocalAppData.Keys)
            {
                result += string.Format("<<{0}><{1}>>", key, LocalAppData[key]);
            }

            return result;
        }

        public static void DecodeDataFromString(string dataFile)
        {
            if (LocalAppData == null)
            {
                LocalAppData = new Dictionary<string, string>();
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
                    LocalAppData[key] = value;
                    index = 0;
                    key = "";
                    value = "";
                }
            }
        }

        public static string GetValue(string key)
        {
            if (LocalAppData == null || LocalAppData.Count == 0)
            {
                LocalAppData = new Dictionary<string, string>();
                PopulateAppData();
            }

            if (LocalAppData.ContainsKey(key))
            {
                return LocalAppData[key];
            }

            return null;
        }

        public static async Task InsertGlobalValue(string name, string value, bool serialize = true)
        {
            if (LocalAppData == null)
            {
                LocalAppData = new Dictionary<string, string>();
            }

            LocalAppData[name] = value;

            if (serialize)
            {
                SerializeData();
            }
        }

        public static async Task SerializeData(CancellationTokenSource ctsToUse = null)
        {
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

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask(ctToUse);

            if ((file.DateCreated - DateTime.Now).TotalMinutes > 1)
            {
                return;
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
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public static async Task<List<BaseTaskControl>> DeserializeData()
        {
            var fileName = "dataFileSerialized.dat";

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var addedControls = new List<BaseTaskControl>();

            bool fileAvailable = true;

            try
            {
                StorageFile file = await folder.GetFileAsync(fileName).AsTask(CancelToken);

                using (Stream stream = await file.OpenStreamForReadAsync())
                using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                using (BinaryReader reader = new BinaryReader(decompressionStream, Encoding.Unicode, true))
                {
                    while (true)
                    {
                        CancelToken.ThrowIfCancellationRequested();

                        var control = reader.ReadString();
                         

                        if (control == null)
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
                                InsertGlobalValue(baseTaskControl.GetType().ToString(), "true", false);
                                baseTaskControl.Deserialize(reader);
                                addedControls.Add(baseTaskControl);
                                break;
                            }
                        }
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
                await SerializeData();
                await DeserializeData();
            }

            return addedControls;
        }

        private static void SerializeAppData(BinaryWriter writer)
        {
            writer.Write("AppData");
            writer.Write(LocalAppData.Keys.Count);

            foreach(var key in LocalAppData.Keys)
            {
                writer.Write(key);
                writer.Write(LocalAppData[key]);
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
    }
}
