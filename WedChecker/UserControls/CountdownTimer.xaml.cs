using System;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WedChecker.UserControls
{
    public sealed partial class CountdownTimer : UserControl
    {
        public event EventHandler WeddingPassed;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public bool WeddingHasPassed
        {
            get
            {
                if (!AppData.WeddingDate.HasValue)
                {
                    return false;
                }

                var weddingDate = AppData.WeddingDate.Value;
                var dateNow = DateTime.Now;

                var days = Convert.ToInt32((weddingDate - dateNow).TotalDays);
                var hours = Convert.ToInt32((weddingDate - dateNow).Hours);
                var minutes = Convert.ToInt32((weddingDate - dateNow).Minutes);
                var seconds = Convert.ToInt32((weddingDate - dateNow).Seconds);

                var weddingHasPassed = (days < 0 || hours < 0 || minutes < 0 || seconds < 0);
                return weddingHasPassed;
            }
        }

        public CountdownTimer()
        {
            this.InitializeComponent();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            UpdateTimeLeft();
        }

        public void UpdateTimeLeft()
        {
            if (!AppData.WeddingDate.HasValue)
            {
                return;
            }

            var weddingDate = AppData.WeddingDate.Value;
            var dateNow = DateTime.Now;

            var days = Convert.ToInt32((weddingDate - dateNow).TotalDays);
            var hours = Convert.ToInt32((weddingDate - dateNow).Hours);
            var minutes = Convert.ToInt32((weddingDate - dateNow).Minutes);
            var seconds = Convert.ToInt32((weddingDate - dateNow).Seconds);

            if (days < 0 || hours < 0 || minutes < 0 || seconds < 0)
            {
                days = 0;
                hours = 0;
                minutes = 0;
                seconds = 0;

                WeddingPassed?.Invoke(this, new EventArgs());
            }

            tbDaysLeft.Text = days.ToString("00");
            tbHoursLeft.Text = hours.ToString("00");
            tbMinutesLeft.Text = minutes.ToString("00");
            tbSecondsLeft.Text = seconds.ToString("00");
        }
    }
}
