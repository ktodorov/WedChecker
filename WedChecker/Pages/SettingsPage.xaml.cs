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
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			this.InitializeComponent();

			mainTitleBar.SetSubTitle("SETTINGS");
			mainTitleBar.SetBackButtonVisible(true);

			LoadActiveTheme();

			this.RequestedTheme = Core.GetElementTheme();

			if (!mainTitleBar.IsMobile)
			{
				rbDefaultTheme.Visibility = Visibility.Collapsed;
			}

            this.Loaded += SettingsPage_Loaded;
		}

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
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

		private void changeWeddingDateButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(WeddingDateChangePage));
		}

		private void LoadActiveTheme()
		{
			var theme = Core.GetAppTheme();

			if (mainTitleBar.IsMobile)
			{
				switch (theme)
				{
					case AppTheme.Dark:
						rbDarkTheme.IsChecked = true;
						break;
					case AppTheme.Light:
						rbLightTheme.IsChecked = true;
						break;
					case AppTheme.SystemDefault:
						rbDefaultTheme.IsChecked = true;
						break;
				}
			}
			else
			{
				switch (theme)
				{
					case AppTheme.Dark:
						rbDarkTheme.IsChecked = true;
						break;
					default:
						rbLightTheme.IsChecked = true;
						break;
				}
			}
		}

		private void appThemeSelection_Checked(object sender, RoutedEventArgs e)
		{
			var button = sender as RadioButton;

			var themeNumber = Convert.ToInt32(button.Tag);
			var appTheme = (AppTheme)themeNumber;

			Core.UpdateAppTheme(appTheme);

			this.RequestedTheme = Core.GetElementTheme();
		}
	}
}
