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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Core.IsFirstLaunch())
            {
                GreetUser();
            }

            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async Task GreetUser()
        {
            await Core.StartUp();
            LaunchControl ucFirstLaunch = new LaunchControl();
            ucFirstLaunch.SubmitButton.Click += SubmitButton_Click;
            ucFirstLaunch.HeaderDialogTextBlock.Text = AppData.GetValue("firstLaunchFirstHeader");
            ucFirstLaunch.TitleDialogTextBlock.Text = AppData.GetValue("firstLaunchFirstTitle");
            ucFirstLaunch.DialogTextBlock.Text = AppData.GetValue("firstLaunchFirstDialog");
            MainGrid.Visibility = Visibility.Collapsed;
            this.Content = ucFirstLaunch;
        }

        void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // sender
            int a = 0;
        }
    }
}
