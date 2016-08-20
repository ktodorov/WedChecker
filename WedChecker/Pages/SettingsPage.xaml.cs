using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using WedChecker.Extensions;
using WedChecker.Infrastructure;
using WedChecker.Helpers;
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
    class SortingType
    {
        public TaskSortingType Type;
        public int Value;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
    class SortingOrder
    {
        public TaskSortingOrder Order;
        public int Value;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
	{
		ObservableCollection<SortingType> sortingTypes = new ObservableCollection<SortingType>();
		ObservableCollection<SortingOrder> sortingOrderings = new ObservableCollection<SortingOrder>();
		ObservableCollection<Currency> currencies = new ObservableCollection<Currency>();

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

            LoadSortingData();

            LoadCurrencies();
        }

        private void LoadSortingData()
        {
            // Task sort type
            var typeEnumValues = Enum.GetValues(typeof(TaskSortingType));

            sortingTypes.Clear();
            foreach (var enumValue in typeEnumValues)
            {
                var sortingType = new SortingType() { Type = (TaskSortingType)enumValue, Value = (int)enumValue, Name = ((TaskSortingType)enumValue).ToString().SeparateCamelCase() };

                sortingTypes.Add(sortingType);
            }

            cbTasksSorting.SelectedIndex = AppData.GetRoamingSetting<int>("TaskSortType");

            // Task sort order
            var orderEnumValues = Enum.GetValues(typeof(TaskSortingOrder));

            sortingOrderings.Clear();
            foreach (var enumValue in orderEnumValues)
            {
                var sortingOrder = new SortingOrder() { Order = (TaskSortingOrder)enumValue, Value = (int)enumValue, Name = ((TaskSortingOrder)enumValue).ToString().SeparateCamelCase() };

                sortingOrderings.Add(sortingOrder);
            }

            cbTasksOrdering.SelectedIndex = AppData.GetRoamingSetting<int>("TaskSortOrder");
        }

        private void LoadCurrencies()
        {
            currencies.Clear();

            var currenciesList = CultureInfoHelper.GetAllCurrencies();
            foreach (var currency in currenciesList)
            {
                currencies.Add(currency);
            }

            var selectedCurrency = CultureInfoHelper.GetStoredCurrency();
            
            cbCurrency.SelectedValue = selectedCurrency;
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

            var currentAppTheme = Core.GetAppTheme();
            if (appTheme == AppTheme.SystemDefault && currentAppTheme != appTheme)
            {
                restartRequiredBlock.Visibility = Visibility.Visible;
            }
            else
            {
                restartRequiredBlock.Visibility = Visibility.Collapsed;
            }

            Core.UpdateAppTheme(appTheme);

			this.RequestedTheme = Core.GetElementTheme();
		}

        private void cbTasksSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppData.InsertRoamingSetting("TaskSortType", cbTasksSorting.SelectedIndex);
        }

        private void cbTasksOrdering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppData.InsertRoamingSetting("TaskSortOrder", cbTasksOrdering.SelectedIndex);
        }

        private void cbCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currencySelected = cbCurrency.SelectedValue as Currency;
            if (currencySelected != null)
            {
                AppData.InsertRoamingSetting("CurrencyCulture", currencySelected.CultureString);
            }
        }
    }
}
