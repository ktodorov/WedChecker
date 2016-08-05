using System;
using System.Threading.Tasks;

// Added during quickstart
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

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
			var remainingTime = await Task.Run(() => CommonData.GetRemainingTime());

			// Update the live tile with the feed items.
			UpdateTile(remainingTime);

            // Inform the system that the task is finished.
            deferral.Complete();
		}

		private static void UpdateTile(TimeSpan remainingTime)
		{
			// Create a tile update manager for the specified syndication feed.
			var updater = TileUpdateManager.CreateTileUpdaterForApplication();
			updater.EnableNotificationQueue(true);
			updater.Clear();

			//XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);
			var tileText = $"{remainingTime.Days} days and {remainingTime.Hours} hours remaining";

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

			// Create a new tile notification. 
			updater.Update(new TileNotification(tileXml));
		}
    }
}
