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
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            var deferral = taskInstance.GetDeferral();

            // Update the live tile with the feed items.
            UpdateTile();

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        public static void RunTileUpdater()
        {
            UpdateTile();
        }

        private static TimeRemaining GetRemainingTime()
        {
            TimeRemaining remainingTime = new TimeRemaining();
            remainingTime.RemainingDays = 0;
            remainingTime.RemainingHours = 0;

            try
            {
                object weddingDateObject = null;

                if (Windows.Storage.ApplicationData.Current.RoamingSettings.Values["WeddingDate"] != null)
                {
                    weddingDateObject = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["WeddingDate"];
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

        private static void UpdateTile()
        {
            // Get the remaining time.
            var remainingTime = GetRemainingTime();

            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            //// Create a live update for a square tile
            //XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText03);

            //XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            //tileTextAttributes[0].InnerText = $"{remainingTime.RemainingDays} days";
            //tileTextAttributes[1].InnerText = $"{remainingTime.RemainingHours} hours";
            //tileTextAttributes[2].InnerText = "remaining";

            //XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            //((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Square71x71Logo.scale-240.png");
            //((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "Contoso Food & Dining logo");

            //var tileBinding = (XmlElement)tileXml.GetElementsByTagName("binding").Item(0);
            //tileBinding.SetAttribute("branding", "nameAndLogo");

            // Create a tile notification for wide tile.
            XmlDocument wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150PeekImage04);

            var wideTitle = $"{remainingTime.RemainingDays} days and {remainingTime.RemainingHours} hours remaining to the wedding";
            wideTileXml.GetElementsByTagName("text")[0].InnerText = wideTitle;

            XmlNodeList wideTileImageAttributes = wideTileXml.GetElementsByTagName("image");
            ((XmlElement)wideTileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/WideLogo.scale-240.png");
            ((XmlElement)wideTileImageAttributes[0]).SetAttribute("alt", "WedChecker");

            // Create a new tile notification. 
            updater.Update(new TileNotification(wideTileXml));

            //var wideTileBinding = (XmlElement)wideTileXml.GetElementsByTagName("binding").Item(0);
            //wideTileBinding.SetAttribute("branding", "nameAndLogo");

            //// Add the wide tile to the square tile's payload, so they are sibling elements under visual 
            //IXmlNode node = tileXml.ImportNode(wideTileXml.GetElementsByTagName("binding").Item(0), true);
            //tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            //// Create a tile notification that will expire in 1 day and send the live tile update.  
            //TileNotification tileNotification = new TileNotification(tileXml);
            //tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddHours(1);
            //TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }
}
