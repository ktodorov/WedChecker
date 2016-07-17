using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Added during quickstart
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace TileUpdateBackgroundProcess
{
	public sealed class RemainingTimeBackgroundProcess : IBackgroundTask
	{
		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			// Get a deferral, to prevent the task from closing prematurely 
			// while asynchronous code is still running.
			BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

			// Download the feed.
			var remainingTime = await Task.Run(() => GetRemainingTime());

			// Update the live tile with the feed items.
			UpdateTile(remainingTime);


            ShowNotificationIfNeeded(remainingTime);

            // Inform the system that the task is finished.
            deferral.Complete();
		}

		private static TimeSpan GetRemainingTime()
		{
			var remainingTime = new TimeSpan();

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

					remainingTime = new TimeSpan(days, hours, 0, 0);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}

			return remainingTime;
		}
		private static void UpdateTile(TimeSpan remainingTime)
		{
			// Create a tile update manager for the specified syndication feed.
			var updater = TileUpdateManager.CreateTileUpdaterForApplication();
			updater.EnableNotificationQueue(true);
			updater.Clear();

			//XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);
			var tileText = $"{remainingTime.Days} days and {remainingTime.Hours} hours remaining";

	//		<binding template=""TileSmall"">
	//	<image src=""Assets/Square71x71Logo.scale-100.png"" placement=""peek""/>
	//	<text hint-style=""captionSubtle"" hint-align=""center"">{tileText}</text>
	//</binding>


			var tileXmlString = $@"<tile>
  <visual>
    <binding template=""TileMedium"" hint-textStacking=""center"">
		<image src=""Assets/Square150x150Logo.scale-100.png"" placement=""peek""/>
		<text hint-align=""center"" hint-wrap=""true"">{tileText}</text>
    </binding>

    <binding template=""TileWide"" hint-textStacking=""center"">
		<image src=""Assets/Wide310x150Logo.scale-100.png"" placement=""peek""/>
		<text hint-align=""center"" hint-wrap=""true"">{tileText}</text>
    </binding>

    <binding template=""TileLarge"" hint-textStacking=""center"">
		<image src=""Assets/Square310x310Logo.scale-100.png"" placement=""peek""/>
		<text hint-align=""center"" hint-wrap=""true"">{tileText}</text>
    </binding>
  </visual>
</tile>
";

			var tileXml = new XmlDocument();
			tileXml.LoadXml(tileXmlString);
			//tileXml.GetElementsByTagName(textElementName)[0].InnerText = tileText;

			// Create a new tile notification. 
			updater.Update(new TileNotification(tileXml));
		}

        private static void ShowNotificationIfNeeded(TimeSpan remainingTime)
        {
            var tileXml = GetTileXmlForRemainingTime(remainingTime);
            if (tileXml == null)
            {
                return;
            }


            PopCustomToast(tileXml);
        }

        private static string GetTileXmlForRemainingTime(TimeSpan remainingTime)
        {
            if (remainingTime.TotalDays > 100)
            {
                return null;
            }

            var daysLeftText = string.Empty;
            if (remainingTime.TotalDays == 100 && Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days100Notified"] == null)
            {
                var days100Notified = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days100Notified"] as bool?;
                if (!days100Notified.HasValue || !days100Notified.Value)
                {
                    daysLeftText = @"
<text>Your wedding is almost here</text>
<text>Only 100 days left</text>";

                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days100Notified"] = true;
                }
            }
            else if (remainingTime.TotalDays == 50 && Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days50Notified"] == null)
            {
                var days50Notified = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days50Notified"] as bool?;
                if (!days50Notified.HasValue || !days50Notified.Value)
                {
                    daysLeftText = @"
<text>Your wedding is almost here</text>
<text>Only 50 days left</text>";

                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days50Notified"] = true;
                }
            }
            else if (remainingTime.TotalDays == 10 && Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days10Notified"] == null)
            {
                var days10Notified = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days10Notified"] as bool?;
                if (!days10Notified.HasValue || !days10Notified.Value)
                {
                    daysLeftText = @"
<text>Your wedding is almost here</text>
<text>Only 10 days left</text>";

                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days10Notified"] = true;
                }
            }
            else if (remainingTime.TotalDays == 1 && Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days1Notified"] == null)
            {
                var days1Notified = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days1Notified"] as bool?;
                if (!days1Notified.HasValue || !days1Notified.Value)
                {
                    daysLeftText = @"
<text>Your wedding is almost here</text>
<text>Are you ready for tomorrow?</text>";

                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Days1Notified"] = true;
                }
            }
            else if (remainingTime.TotalDays < 0 && Windows.Storage.ApplicationData.Current.RoamingSettings.Values["WeddingPassedNotified"] == null)
            {
                var weddingPassedNotified = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["WeddingPassedNotified"] as bool?;
                if (!weddingPassedNotified.HasValue || !weddingPassedNotified.Value)
                {
                    daysLeftText = @"
<text>Your perfect wedding just passed</text>
<text>WedChecker wishes you a happy married life like the ones in the Hollywood movies :)</text>";

                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values["WeddingPassedNotified"] = true;
                }
            }
            else
            {
                return null;
            }

            if (string.IsNullOrEmpty(daysLeftText))
            {
                return null;
            }

            var xml = $@"
<toast>
    <visual>
        <binding template='ToastGeneric'>
            {daysLeftText}
        </binding>
    </visual>
</toast>";

            return xml;
        }

        public static ToastNotification PopCustomToast(string xml)
        {
            return PopCustomToast(xml, null, null);
        }

        public static ToastNotification PopCustomToast(string xml, string tag, string group)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);


            return PopCustomToast(doc, tag, group);
        }

        [DefaultOverloadAttribute]
        public static ToastNotification PopCustomToast(XmlDocument doc, string tag, string group)
        {
            var toast = new ToastNotification(doc);

            if (tag != null)
                toast.Tag = tag;

            if (group != null)
                toast.Group = group;

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            return toast;
        }
    }
}
