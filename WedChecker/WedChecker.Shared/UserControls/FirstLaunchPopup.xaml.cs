using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class FirstLaunchPopup : UserControl
    {
        public FirstLaunchPopup()
        {
            this.InitializeComponent();
        }

        public void GreetUser()
        {
            SubmitButton.Click += SubmitButton_Click;
            popup.Visibility = Visibility.Visible;
        }

        void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            CheckForAdvancement();
        }

        private void CheckForAdvancement()
        {
            var timesProcessed = Convert.ToInt32(TimesProcessed.Text);

            if (timesProcessed == 0)
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
                firstStepPanel.Visibility = Visibility.Collapsed;
                secondStepPanel.Visibility = Visibility.Visible;

                HeaderDialogTextBlock.Visibility = Visibility.Collapsed;
                TitleDialogTextBlock.Visibility = Visibility.Collapsed;
            }
            else if (timesProcessed == 1)
            {
                var weddingDate = new DateTime(dpWeddingDate.Date.Year, dpWeddingDate.Date.Month, dpWeddingDate.Date.Day, 
                                               tpWeddingDate.Time.Hours, tpWeddingDate.Time.Minutes, 0);
                Core.SetSetting("WeddingDate", weddingDate.ToString());
            }

            timesProcessed++;
            TimesProcessed.Text = timesProcessed.ToString();

            if (timesProcessed >= 2)
            {
                var layoutRoot = this.Parent as Grid;
#if WINDOWS_PHONE_APP
                var mainPivot = layoutRoot.FindName("mainPivot") as Pivot;
                mainPivot.Visibility = Visibility.Visible;
#else
#endif
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
}
