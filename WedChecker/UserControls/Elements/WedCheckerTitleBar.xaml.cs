using WedChecker.Exceptions;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Elements
{
	public sealed partial class WedCheckerTitleBar : UserControl
	{
		private bool isMobile = false;

		public event RoutedEventHandler BackButtonClick;

		private SolidColorBrush _backgroundBrush;
		private SolidColorBrush backgroundBrush
		{
			get
			{
				if (_backgroundBrush == null)
				{


					var backgroundBrushName = "SystemControlBackgroundAccentBrush";
					if (Application.Current.Resources.ContainsKey(backgroundBrushName))
					{
						_backgroundBrush = new SolidColorBrush((Application.Current.Resources[backgroundBrushName] as SolidColorBrush).Color);
					}
				}

				return _backgroundBrush;
			}
		}

		private SolidColorBrush _deactivatedBackgroundBrush;
		private SolidColorBrush deactivatedBackgroundBrush
		{
			get
			{
				if (_deactivatedBackgroundBrush == null)
				{


					var backgroundBrushName = "SystemControlBackgroundBaseHighBrush";
					if (Application.Current.Resources.ContainsKey(backgroundBrushName))
					{
						_deactivatedBackgroundBrush = new SolidColorBrush((Application.Current.Resources[backgroundBrushName] as SolidColorBrush).Color);
					}
				}

				return _deactivatedBackgroundBrush;
			}
		}

		private SolidColorBrush _foregroundBrush;
		private SolidColorBrush foregroundBrush
		{
			get
			{
				if (_backgroundBrush == null)
				{


					var foregroundBrushName = "SystemControlBackgroundAccentBrush";
					if (Application.Current.Resources.ContainsKey(foregroundBrushName))
					{
						_foregroundBrush = new SolidColorBrush((Application.Current.Resources[foregroundBrushName] as SolidColorBrush).Color);
					}
				}

				return _foregroundBrush;
			}
		}

		public bool ProgressActive
		{
			get
			{
				return loadingProgress.IsActive;
			}
			set
			{
				loadingProgress.IsActive = value;
			}
		}

		public WedCheckerTitleBar()
		{
			this.InitializeComponent();

			SystemNavigationManager.GetForCurrentView().BackRequested += WedCheckerTitleBar_BackRequested;

			var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

			coreTitleBar.ExtendViewIntoTitleBar = true;

			//PC customization
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
			{
				isMobile = false;
				var appTitleBar = ApplicationView.GetForCurrentView().TitleBar;
				if (appTitleBar != null)
				{
					appTitleBar.ButtonBackgroundColor = Colors.Transparent;
					appTitleBar.ButtonForegroundColor = Colors.White;
					appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
				}
			}

			//Mobile customization
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				TitleBar.MinHeight = 50;
				isMobile = true;
				var statusBar = StatusBar.GetForCurrentView();
				if (statusBar != null)
				{
					statusBar.BackgroundOpacity = 1;
					statusBar.BackgroundColor = backgroundBrush?.Color;
					statusBar.ForegroundColor = foregroundBrush?.Color;
				}
			}

			Window.Current.SetTitleBar(MainTitleBar);

			Window.Current.Activated += Current_Activated;

			coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
			coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
		}

		private void WedCheckerTitleBar_BackRequested(object sender, BackRequestedEventArgs e)
		{
			if (BackButtonClick != null)
			{
				try
				{
					BackButtonClick(this, new RoutedEventArgs());
					e.Handled = true;
				}
				catch (WedCheckerNavigationException)
				{
				}
			}
		}

		private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
		{
			TitleBar.Height = sender.Height;
		}

		private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
		{
			TitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		private void Current_Activated(object sender, WindowActivatedEventArgs e)
		{
			if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
			{
				backButton.IsEnabled = true;
				MainTitleBar.Background = backgroundBrush;
				MainTitleBar.Opacity = 1;
			}
			else
			{
				backButton.IsEnabled = false;
				MainTitleBar.Background = deactivatedBackgroundBrush;
				MainTitleBar.Opacity = 0.5;
			}
		}

		public void SetSubTitle(string subtitle)
		{
			subtitleBlock.Text = subtitle;
		}

		private void backButton_Click(object sender, RoutedEventArgs e)
		{
			BackButtonClick?.Invoke(this, e);
		}

		public void SetBackButtonVisible(bool visible)
		{
			backButton.Visibility = (visible && !isMobile) ? Visibility.Visible : Visibility.Collapsed;
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visible ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
		}
	}
}
