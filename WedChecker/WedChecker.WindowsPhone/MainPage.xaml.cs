using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WedChecker.CustomControls;
using WedChecker.Common;
using WedChecker.Pages;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WedChecker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Loaded += async (sender, args) => 
            {
                if (Core.IsFirstLaunch())
                {
                    await GreetUser();
                    Core.RoamingSettings.Values["first"] = true;
                }
                else
                {
                    Frame.Navigate(typeof(MainScreenPage));
                }
            };
        }

        private async Task GreetUser()
        {
            await Core.StartUp();
            SubmitButton.Click += SubmitButton_Click;
            HeaderDialogTextBlock.Text = AppData.GetValue("firstLaunchFirstHeader");
            TitleDialogTextBlock.Text = AppData.GetValue("firstLaunchFirstTitle");
            DialogTextBlock.Text = AppData.GetValue("firstLaunchFirstDialog");
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
                
                Frame.Navigate(typeof(MainPage));
            }
            else if (timesProcessed == 1)
            {
                Core.RoamingSettings.Values["WeddingDate"] = dpWeddingDate.Date;
            }

            timesProcessed++;
            TimesProcessed.Text = timesProcessed.ToString();

            if (timesProcessed >= 2)
            {
                Frame.Navigate(typeof(MainScreenPage));
            }
        }
    }
}
