using System;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WedChecker.WindowsPhoneControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            DisplayLastBackedUpDate();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void wiFiOnlyToggle_Toggled(object sender, RoutedEventArgs e)
        {
            await AppData.InsertGlobalValue("wifiOnlySync", wiFiOnlyToggle.IsOn.ToString());
        }

        private async void backupButton_Click(object sender, RoutedEventArgs e)
        {
            backupButton.IsEnabled = false;
            backupButton.Content = "Please wait...";

            if (Core.CanRoamData())
            {
                await AppData.SerializeData(null, true);
            }
            else
            {
                tbBackupErrorMessage.Visibility = Visibility.Visible;
            }

            backupButton.IsEnabled = true;
            backupButton.Content = "Backup now";

            DisplayLastBackedUpDate();
        }

        private void DisplayLastBackedUpDate()
        {
            var saved = true;
            if (Core.CanRoamData())
            {
                tbBackupErrorMessage.Visibility = Visibility.Collapsed;
                if (Core.RoamingSettings.Values.ContainsKey("SerializedOn"))
                {
                    var serializedOn = Core.RoamingSettings.Values["SerializedOn"] as string;

                    if (serializedOn != null)
                    {
                        var serializedOnDate = DateTime.Parse(serializedOn);

                        tbBackupDate.Text = serializedOnDate.ToLocalTime().ToString();
                    }
                    else
                    {
                        saved = false;
                    }
                }
                else
                {
                    saved = false;
                }
            }
            else
            {
                saved = false;
            }

            if (!saved)
            {
                tbBackupDate.Text = "None";
            }
        }
    }
}
