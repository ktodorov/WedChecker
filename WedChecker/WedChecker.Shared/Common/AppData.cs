using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace WedChecker.Common
{
    public static class AppData
    {
        private static Dictionary<string, string> LocalAppData
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
            var keyValuePairs = Regex.Match(dataFile, @"\<([^)]*)\>").Groups;

            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                string key = Regex.Match(keyValuePairs[i].Value, @"\<([^)]*)\>").Groups[1].Value;
                string value = Regex.Match(keyValuePairs[i].Value, @"\<([^)]*)\>").Groups[2].Value;
                LocalAppData[key] = value;
            }
        }

        public static string GetValue(string key)
        {
            if (LocalAppData.ContainsKey(key))
            {
                return LocalAppData[key];
            }

            return null;
        }

        public static async Task WriteDataFile()
        {
            var encodedData = EncodeDataToString();
            var fileName = "dataFile.txt";

            byte[] data = Encoding.UTF8.GetBytes(encodedData);

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            if ((file.DateCreated - DateTime.Now).TotalMinutes > 1)
            {
                return;
            }

            using (Stream s = await file.OpenStreamForWriteAsync())
            {
                await s.WriteAsync(data, 0, data.Length);
            }
        }

        public static async Task<string> ReadDataFile()
        {
            var fileName = "dataFile.txt";

            byte[] data;

            StorageFolder folder = ApplicationData.Current.LocalFolder;

            await WriteDataFile();

            var file = await folder.GetFileAsync(fileName);

            using (Stream s = await file.OpenStreamForReadAsync())
            {
                data = new byte[s.Length];
                await s.ReadAsync(data, 0, (int)s.Length);
            }

            return Encoding.UTF8.GetString(data, 0, data.Length);
        }
    }
}
