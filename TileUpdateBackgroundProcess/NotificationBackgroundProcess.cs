using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;

namespace TileUpdateBackgroundProcess
{
    public sealed class NotificationBackgroundProcess : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // Download the feed.
            var remainingTime = await Task.Run(() => CommonData.GetRemainingTime());

            ShowNotificationIfNeeded(remainingTime);

            // Inform the system that the task is finished.
            deferral.Complete();
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
