using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace UniversalBackgroundTasks
{
    public struct TimeRemaining
    {
        public int RemainingDays;
        public int RemainingHours;
    }

    public sealed class RemainingTimeBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Get the remaining time.
            var remainingTime = GetRemainingTime();

            // Update the live tile with the feed items.
            UpdateTile(remainingTime);

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private static TimeRemaining GetRemainingTime()
        {
            TimeRemaining remainingTime = new TimeRemaining();
            remainingTime.RemainingDays = 0;
            remainingTime.RemainingHours = 0;

            try
            {
                object weddingDateObject = null;

                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["WeddingDate"] != null)
                {
                    weddingDateObject = Windows.Storage.ApplicationData.Current.LocalSettings.Values["WeddingDate"];
                }

                var weddingDate = Convert.ToDateTime(weddingDateObject);

                if (weddingDate != null)
                {
                    var days = Convert.ToInt32((weddingDate - DateTime.Now).TotalDays);
                    var hours = Convert.ToInt32((weddingDate - DateTime.Now).Hours);

                    remainingTime.RemainingDays = days;
                    remainingTime.RemainingHours = hours;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return remainingTime;
        }

        private static void UpdateTile(TimeRemaining remainingTime)
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Create a tile notification for each feed item.
            {
                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text04);

                var title = $"{remainingTime.RemainingDays} days and {remainingTime.RemainingHours} hours remaining to the wedding!";
                tileXml.GetElementsByTagName(textElementName)[0].InnerText = title;

                // Create a new tile notification. 
                updater.Update(new TileNotification(tileXml));
            }
        }
        static string textElementName = "text";

    }
}
