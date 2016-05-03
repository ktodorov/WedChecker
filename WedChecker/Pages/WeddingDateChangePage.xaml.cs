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
			mainTitleBar.BackButtonClick += MainTitleBar_BackButtonClick;

			var weddingDateString = AppData.GetRoamingSetting<string>("WeddingDate");
			if (string.IsNullOrEmpty(weddingDateString))
			{
				return;
			}
			var weddingDate = new DateTime();

			try
			{
				weddingDate = Convert.ToDateTime(weddingDateString);

				currentDateTextBlock.Text = weddingDate.ToString("HH:mm, dd MMMM, yyyy");

				dpWeddingDate.Date = weddingDate.Date;
				tpWeddingDate.Time = weddingDate.TimeOfDay;
			}
			catch (FormatException)
			{
				return;
			}

			this.RequestedTheme = Core.GetElementTheme();
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

		private void submitDateButton_Click(object sender, RoutedEventArgs e)
		{
			mainTitleBar.ProgressActive = true;

			var weddingDate = new DateTime(dpWeddingDate.Date.Year, dpWeddingDate.Date.Month, dpWeddingDate.Date.Day,
											   tpWeddingDate.Time.Hours, tpWeddingDate.Time.Minutes, 0);

			Core.SetSetting("WeddingDate", weddingDate.ToString());

			currentDateTextBlock.Text = weddingDate.ToString("HH:mm, dd MMMM, yyyy");

			mainTitleBar.ProgressActive = false;
		}
	}
}
