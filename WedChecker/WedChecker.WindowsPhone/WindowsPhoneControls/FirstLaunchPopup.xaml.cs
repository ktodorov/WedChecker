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

            var layoutRoot = this.Parent as Grid;
            var mainPivot = layoutRoot.FindName("mainPivot") as Pivot;
            mainPivot.Visibility = Visibility.Visible;

            var appBar = layoutRoot.FindName("appBar") as AppBar;
            appBar.Visibility = Visibility.Visible;

            var tbGreetUser = layoutRoot.FindName("tbGreetUser") as TextBlock;
            tbGreetUser.Text = string.Format("Hello, {0}", Core.GetSetting("Name"));

            var tbCountdownTimer = layoutRoot.FindName("tbCountdownTimer") as CountdownTimer;
            tbCountdownTimer.UpdateTimeLeft();

            popup.Visibility = Visibility.Collapsed;

            Core.SetSetting("first", true);
        }
    }
}
