using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
    public sealed class RemainingTimeBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Download the feed.
            var remainingTime = await GetRemainingTime();

            // Update the live tile with the feed items.
            UpdateTile(remainingTime);

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private static async Task<string> GetRemainingTime()
        {
            string remainingTime = null;

            try
            {
                //// Create a syndication client that downloads the feed.  
                //SyndicationClient client = new SyndicationClient();
                //client.BypassCacheOnRetrieve = true;
                //client.SetRequestHeader(customHeaderName, customHeaderValue);

                //// Download the feed. 
                //feed = await client.RetrieveFeedAsync(new Uri(feedUrl));

                remainingTime = "test";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return remainingTime;
        }

        private static void UpdateTile(string remainingTime)
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Create a tile notification for each feed item.
            //foreach (var item in feed.Items)
            {
                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);

                var title = remainingTime;
                //string titleText = title.Text == null ? String.Empty : title.Text;
                tileXml.GetElementsByTagName(textElementName)[0].InnerText = title;

                // Create a new tile notification. 
                updater.Update(new TileNotification(tileXml));

                // Don't create more than 5 notifications.
            }
        }
        static string textElementName = "text";
    }
}
