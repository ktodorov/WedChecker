using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using Windows.ApplicationModel;
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
	public sealed partial class AboutPage : Page
	{
		public AboutPage()
		{
			this.InitializeComponent();

			mainTitleBar.SetSubTitle("ABOUT");
			mainTitleBar.SetBackButtonVisible(true);
			mainTitleBar.BackButtonClick += MainTitleBar_BackButtonClick;

			this.RequestedTheme = Core.GetElementTheme();

            var appVersion = GetAppVersion();
            tbAppVersion.Text = $"v {appVersion}";

            currentYear.Text = DateTime.Now.Year.ToString();

            this.Loaded += AboutPage_Loaded;
        }

        private void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            Core.CurrentTitleBar = mainTitleBar;
        }

        private void MainTitleBar_BackButtonClick(object sender, RoutedEventArgs e)
		{
			if (Frame.CanGoBack)
			{
				Frame.GoBack();
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
		}
        public static string GetAppVersion()
        {

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            var versionString = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

            return versionString;
        }
    }
}
