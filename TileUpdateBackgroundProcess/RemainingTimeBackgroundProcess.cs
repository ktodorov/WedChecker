using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Added during quickstart
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
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

		// Although most HTTP servers do not require User-Agent header, others will reject the request or return 
		// a different response if this header is missing. Use SetRequestHeader() to add custom headers. 
		static string customHeaderName = "User-Agent";
		static string customHeaderValue = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

		static string textElementName = "text";
		static string feedUrl = @"http://blogs.msdn.com/b/MainFeed.aspx?Type=BlogsOnly";
	}
}
