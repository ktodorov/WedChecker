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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WedChecker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeddingDateChangePage : Page
    {
        public WeddingDateChangePage()
        {
            this.InitializeComponent();

            mainTitleBar.SetSubTitle("SETTINGS");
            mainTitleBar.SetBackButtonVisible(true);

            var weddingDate = GetSavedWeddingDate();

            if (weddingDate != null && weddingDate.HasValue)
            {
                currentDateTextBlock.Text = weddingDate.Value.ToString("dd MMMM, yyyy, HH:mm");

                dpWeddingDate.Date = weddingDate.Value.Date;
                tpWeddingDate.Time = weddingDate.Value.TimeOfDay;
            }

            this.RequestedTheme = Core.GetElementTheme();

            this.Loaded += WeddingDateChangePage_Loaded;
        }

        private void WeddingDateChangePage_Loaded(object sender, RoutedEventArgs e)
        {
            Core.CurrentTitleBar = mainTitleBar;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private DateTime? GetSavedWeddingDate()
        {
            var weddingDateString = AppData.GetRoamingSetting<string>("WeddingDate");
            if (string.IsNullOrEmpty(weddingDateString))
            {
                return null;
            }
            var weddingDate = new DateTime();

            try
            {
                weddingDate = Convert.ToDateTime(weddingDateString);

                return weddingDate;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private void submitDateButton_Click(object sender, RoutedEventArgs e)
        {
            tbError.Visibility = Visibility.Collapsed;
            mainTitleBar.ProgressActive = true;

            var weddingDate = new DateTime(dpWeddingDate.Date.Year, dpWeddingDate.Date.Month, dpWeddingDate.Date.Day,
                                               tpWeddingDate.Time.Hours, tpWeddingDate.Time.Minutes, 0);

            if (weddingDate < DateTime.Now)
            {
                tbError.Text = "Sorry, you can't set date which is already passed";
                tbError.Visibility = Visibility.Visible;
                mainTitleBar.ProgressActive = false;
                return;
            }

            var savedWeddingDate = GetSavedWeddingDate();
            if (savedWeddingDate.HasValue && savedWeddingDate == weddingDate)
            {
                tbError.Text = "This is already the saved date";
                tbError.Visibility = Visibility.Visible;
                mainTitleBar.ProgressActive = false;
                return;
            }

            Core.SetSetting("WeddingDate", weddingDate.ToString());

            currentDateTextBlock.Text = weddingDate.ToString("dd MMMM, yyyy, HH:mm");

            // We can now notify the user again
            AppData.InsertRoamingSetting("Days100Notified", false);
            AppData.InsertRoamingSetting("Days50Notified", false);
            AppData.InsertRoamingSetting("Days10Notified", false);
            AppData.InsertRoamingSetting("Days1Notified", false);
            AppData.InsertRoamingSetting("WeddingPassedNotified", false);

            mainTitleBar.ProgressActive = false;

            tbInfo.Visibility = Visibility.Visible;
        }
    }
}
