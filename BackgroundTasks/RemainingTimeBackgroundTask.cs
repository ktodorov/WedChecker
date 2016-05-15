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
			var deferral = taskInstance.GetDeferral();

			// Update the live tile with the feed items.
			await UpdateTile();

			// Inform the system that the task is finished.
			deferral.Complete();
		}

		private static async Task UpdateTile()
		{
			// Get the remaining time.
			var remainingTime = await Task.Run(() => GetRemainingTime());

			// Create a tile update manager for the specified syndication feed.
			var updater = TileUpdateManager.CreateTileUpdaterForApplication();
			updater.EnableNotificationQueue(true);
			updater.Clear();

			// Create a tile notification for wide tile.
			//XmlDocument wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText01);

			var wideTitle = $"{remainingTime.RemainingDays} days and {remainingTime.RemainingHours} hours remaining to the wedding";

			var xml = @"<tile>
  <visual>

    <binding template=""TileWide"">
      <image src=""Assets/Wide310x150Logo.scale-200"" placement=""peek"" hint-overlay=""20""/>
	  <text>
	test test
		</text>
    </binding>
  </visual>
</tile>
";
			var wideTileXml = new XmlDocument();
			wideTileXml.InnerText = xml;


			//wideTileXml.GetElementsByTagName("text")[0].InnerText = wideTitle;

			//XmlNodeList wideTileImageAttributes = wideTileXml.GetElementsByTagName("image");
			//((XmlElement)wideTileImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Wide310x150Logo.scale-200.png");
			//((XmlElement)wideTileImageAttributes[0]).SetAttribute("alt", "WedChecker");

			// Create a new tile notification. 
			updater.Update(new TileNotification(wideTileXml));
		}

	}
}
