using System;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WedChecker.UserControls
{
    public sealed partial class FirstLaunchPopup : UserControl
    {
        public FirstLaunchPopup()
        {
            this.InitializeComponent();
        }

		public event EventHandler FinishedSubmitting;

		async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var writtenName = NameTextBox.Text;
            if (string.IsNullOrEmpty(writtenName))
            {
                tbError.Text = "Please, enter a valid name.";
                return;
            }
            else
            {
                tbError.Text = string.Empty;
            }

            Core.SetSetting("Name", NameTextBox.Text);
            mainPivot.IsLocked = false;
            mainPivot.SelectedIndex = 1;

            await Task.Delay(1000);
            mainPivot.Items.Remove(firstPivotItem);
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            var weddingDate = new DateTime(dpWeddingDate.Date.Year, dpWeddingDate.Date.Month, dpWeddingDate.Date.Day,
                                               tpWeddingDate.Time.Hours, tpWeddingDate.Time.Minutes, 0);

            Core.SetSetting("WeddingDate", weddingDate.ToString());

            AppData.InsertRoamingSetting("FirstLaunch", false);
            AppData.InsertRoamingSetting("HighPriority", true);

            FinishedSubmitting?.Invoke(this, new EventArgs());
		}

		private void NameTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				SubmitButton_Click(SubmitButton, new RoutedEventArgs());
			}
		}
	}
}
