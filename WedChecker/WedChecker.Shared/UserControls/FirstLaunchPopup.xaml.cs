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

namespace WedChecker.CustomControls
{
    public sealed partial class FirstLaunchPopup : UserControl
    {
        public FirstLaunchPopup()
        {
            this.InitializeComponent();
        }

        public async Task GreetUser()
        {
            await Core.StartUp();
            SubmitButton.Click += SubmitButton_Click;
            HeaderDialogTextBlock.Text = AppData.GetValue("firstLaunchFirstHeader");
            TitleDialogTextBlock.Text = AppData.GetValue("firstLaunchFirstTitle");
            DialogTextBlock.Text = AppData.GetValue("firstLaunchFirstDialog");
            //popup.IsOpen = true;
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
                Core.RoamingSettings.Values["Name"] = NameTextBox.Text;
                NameTextBox.Visibility = Visibility.Collapsed;
                dpWeddingDate.Visibility = Visibility.Visible;

                HeaderDialogTextBlock.Visibility = Visibility.Collapsed;
                TitleDialogTextBlock.Visibility = Visibility.Collapsed;
                DialogTextBlock.Text = AppData.GetValue("firstLaunchSecondDialog");
            }
            else if (timesProcessed == 1)
            {
                Core.RoamingSettings.Values["WeddingDate"] = dpWeddingDate.Date;
            }

            timesProcessed++;
            TimesProcessed.Text = timesProcessed.ToString();

            if (timesProcessed >= 2)
            {
                //popup.IsOpen = false;
                popup.Visibility = Visibility.Collapsed;
                Core.RoamingSettings.Values["first"] = true;
            }
        }
    }
}
