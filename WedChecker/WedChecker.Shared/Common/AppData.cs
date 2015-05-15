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

        public static string[] TaskControls =
        {
            // Plan
            "WeddingBudget", "WeddingStyle", "RegistryPlace", "ReligiousPlace",
            "DocumentsRequired", "Restaurant", "RestaurantFood", "BestMan_MaidOfHonour",
            "BridesmaidsGroomsmen", "Decoration", "FreshFlowers", "MusicLayout",
            "Photographer", "BrideAccessories", "BrideClothes", "GroomAccessories",
            "GroomClothes", "BMMOHAccessories", "BMMOHClothes", "BAGAccessories",
            "BAGClothes", "HoneymoonDestination", "GuestsList", "ForeignGuestsAccomodation",
            "HairdresserMakeupArtist", "Invitations",
            // Purchase
            "PurchaseBrideAccessories", "PurchaseBrideClothes", "PurchaseGroomAccessories", "PurchaseGroomClothes", 
            "PurchaseBMMOHAccessories", "PurchaseBMMOHClothes", "PurchaseBAGAccessories", "PurchaseBAGClothes",
            "PurchaseRestaurantFood", "PurchaseFreshFlowers", "PurchaseRings", "PurchaseCake",
            // Bookings
            "BookMusicLayout", "BookPhotographer", "BookHoneymoonDestination", "BookGuestsAccomodation",
            "BookHairdresserMakeupArtistAppointments",
            "SendInvitations", "RestaurantAccomodationPlan"
        };

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
            if (LocalAppData.ContainsKey(key))
            {
                return LocalAppData[key];
            }
            else
            {
                if (LocalAppData.ContainsKey(key))
                {
                    return LocalAppData[key];
                }
            }

            return null;
        }

        public static async Task DecodeFileData(StorageFile file)
        {
            byte[] data;

            using (Stream s = await file.OpenStreamForReadAsync())
            {
                data = new byte[s.Length];
                await s.ReadAsync(data, 0, (int)s.Length);
            }

            DecodeDataFromString(Encoding.UTF8.GetString(data, 0, data.Length));
        }

        public static async Task WriteDataFile()
        {
            var encodedData = EncodeDataToString();
            var fileName = "dataFile.txt";

            byte[] data = Encoding.UTF8.GetBytes(encodedData);

            try
            {
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
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task ReadDataFile()
        {
            var fileName = "dataFile.txt";


            StorageFolder folder = ApplicationData.Current.LocalFolder;

            StorageFile file;
            bool fileAvailable = true;

            try
            {
                file = await folder.GetFileAsync(fileName);
                await DecodeFileData(file);
            }
            catch (FileNotFoundException)
            {
                // There is no such file..
                fileAvailable = false;
            }

            if (!fileAvailable)
            {
                await WriteDataFile();
                file = await folder.GetFileAsync(fileName);
                await DecodeFileData(file);
            }
        }

        public static async Task WriteRoamingDataFile()
        {
            var encodedData = EncodeDataToString();
            var fileName = "dataFile.txt";

            byte[] data = Encoding.UTF8.GetBytes(encodedData);

            StorageFolder folder = ApplicationData.Current.RoamingFolder;
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

        public static async void ReadRoamingDataFile()
        {
            var fileName = "dataFile.txt";

            StorageFolder folder = ApplicationData.Current.RoamingFolder;

            StorageFile file;
            bool fileAvailable = true;

            try
            {
                file = await folder.GetFileAsync(fileName);
                await DecodeFileData(file);
            }
            catch (FileNotFoundException)
            {
                // There is no such file.. So create one
                fileAvailable = false;
            }

            if (!fileAvailable)
            {
                await WriteRoamingDataFile();
                file = await folder.GetFileAsync(fileName);
                await DecodeFileData(file);
            }
        }


        public static async Task InsertGlobalValue(string name, string value)
        {
            LocalAppData[name] = value;
            await WriteDataFile();
            await WriteRoamingDataFile();
        }
    }
}
