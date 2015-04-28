using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            if (Core.LocalSettings.Values.Keys.Contains("WeddingDate") && Core.LocalSettings.Values["WeddingDate"] != null)
            {
                var weddingDate = Convert.ToDateTime(Core.LocalSettings.Values["WeddingDate"].ToString());
                var days = Convert.ToInt32((weddingDate - DateTime.Now).TotalDays);
                var daysString = days.ToString();
                daysString += days != 1 ? " DAYS" : " DAY";
                var hours = Convert.ToInt32((weddingDate - DateTime.Now).Hours);
                var hoursString = hours.ToString();
                hoursString += hours != 1 ? " HOURS" : " HOUR";
                var minutes = Convert.ToInt32((weddingDate - DateTime.Now).Minutes);
                var minutesString = minutes.ToString();
                minutesString += minutes != 1 ? " MINUTES" : " MINUTE";

                tbTimeLeft.Text = string.Format("{0}\n{1}\n{2}", daysString, hoursString, minutesString);
            }
            else if (Core.RoamingSettings.Values.Keys.Contains("WeddingDate") && Core.RoamingSettings.Values["WeddingDate"] != null)
            {
                var weddingDate = Convert.ToDateTime(Core.RoamingSettings.Values["WeddingDate"].ToString());
                var days = Convert.ToInt32((weddingDate - DateTime.Now).TotalDays);
                var daysString = days.ToString();
                daysString += days != 1 ? " DAYS" : " DAY";
                var hours = Convert.ToInt32((weddingDate - DateTime.Now).Hours);
                var hoursString = hours.ToString();
                hoursString += hours != 1 ? " HOURS" : " HOUR";
                var minutes = Convert.ToInt32((weddingDate - DateTime.Now).Minutes);
                var minutesString = minutes.ToString();
                minutesString += minutes != 1 ? " MINUTES" : " MINUTE";

                tbTimeLeft.Text = string.Format("{0}\n{1}\n{2}", daysString, hoursString, minutesString);
            }
        }
    }
}
