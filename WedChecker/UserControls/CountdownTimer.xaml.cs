using System;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WedChecker.UserControls
{
    public sealed partial class CountdownTimer : UserControl
    {
        public CountdownTimer()
        {
            this.InitializeComponent();
        }

        private void tbTimeLeft_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTimeLeft();
        }

        public void UpdateTimeLeft()
        {
            var weddingDateString = AppData.GetRoamingSetting<string>("WeddingDate");
            if (string.IsNullOrEmpty(weddingDateString))
            {
                return;
            }
            var weddingDate = new DateTime();
            
            try
            {
                weddingDate = Convert.ToDateTime(weddingDateString);
            }
            catch (FormatException)
            {
                return;
            }

            var days = Convert.ToInt32((weddingDate - DateTime.Now).TotalDays);
            var hours = Convert.ToInt32((weddingDate - DateTime.Now).Hours);
            var minutes = Convert.ToInt32((weddingDate - DateTime.Now).Minutes);
            var seconds = Convert.ToInt32((weddingDate - DateTime.Now).Seconds);

            tbDaysLeft.Text = days.ToString("00");
            tbHoursLeft.Text = hours.ToString("00");
            tbMinutesLeft.Text = minutes.ToString("00");
            tbSecondsLeft.Text = seconds.ToString("00");
        }
    }
}
