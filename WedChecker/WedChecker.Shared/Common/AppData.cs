using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace WedChecker.Common
{
    public static class AppData
    {
        private static Dictionary<string, string> appData;
        private static Dictionary<string, string> localAppData;

        public static void PopulateAppData()
        {
            appData["firstLaunchFirstHeader"] = "Hello,\nwelcome to";
            appData["firstLaunchFirstTitle"] = "WedChecker";
            appData["firstLaunchFirstDialog"] = "No doubt we will make a wonderful wedding.\nCan I know your name first?\nIt will help me to know you better.";

            appData["firstLaunchSecondHeader"] = "Great!\n Let's start now...";
        }

        public static string EncodeDataToString()
        {
            var result = string.Empty;

            foreach (var key in appData.Keys)
            {
                result += string.Format("<<{0}><{1}>>", key, appData[key]);
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
                localAppData[key] = value;
            }
        }

        public static string GetValue(string key)
        {
            if (localAppData.ContainsKey(key))
            {
                return localAppData[key];
            }

            return null;
        }

        public static async Task SetDataFile()
        {

            StorageFile sampleFile = await Core.LocalFolder.CreateFileAsync("dataFile.txt", CreationCollisionOption.ReplaceExisting);

            if ((sampleFile.DateCreated - DateTime.Now).TotalMinutes < 1)
            {
                if (appData.Count == 0)
                {
                    PopulateAppData();
                }

                var encodedData = EncodeDataToString();

                await FileIO.WriteTextAsync(sampleFile, encodedData);
            }
            else
            {
                string dataFile = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                DecodeDataFromString(dataFile);
            }
        }
    }
}
