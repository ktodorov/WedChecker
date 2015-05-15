﻿using System;
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
using WedChecker.UserControls;
using WedChecker.UserControls.Tasks;
using WedChecker.Common;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;

namespace WedChecker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            tbCountdownTimer.UpdateTimeLeft();
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (LbTasks.Visibility == Visibility.Visible)
            {
                LbTasks.Visibility = Visibility.Collapsed;
                appBar.Visibility = Visibility.Visible;
                mainPivot.Visibility = Visibility.Visible;
                e.Handled = true;
            }
            else if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Core.RoamingSettings.Values["first"] = false; // debug
            Loaded += async (sender, args) =>
            {
                if (Core.IsFirstLaunch())
                {
                    await firstLaunchPopup.GreetUser();
                }
                else
                {
                    await AppData.ReadDataFile();
                    firstLaunchPopup.Visibility = Visibility.Collapsed;
                    appBar.Visibility = Visibility.Visible;
                    tbGreetUser.Text = string.Format("Hello, {0}", Core.RoamingSettings.Values["Name"]);
                    AddPopulatedControls();
                    mainPivot.Visibility = Visibility.Visible;
                }
            };
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            LbTasks.Visibility = Visibility.Visible;
            appBar.Visibility = Visibility.Collapsed;
            mainPivot.Visibility = Visibility.Collapsed;
        }

        private void AddPopulatedControls()
        {
            var populatedControls = Core.GetPopulatedControls();

            foreach (var populatedControl in populatedControls)
            {
                CreateTaskControl(populatedControl);
            }
        }

        private void CreateTaskControl(KeyValuePair<string, object> populatedControl)
        {
            object value = populatedControl.Value;
            switch (populatedControl.Key)
            {
                case "WeddingBudget":
                    CreateWeddingBudgetControl(value);
                    break;

                case "WeddingStyle":

                    break;

                case "RegistryPlace":

                    break;

                case "ReligiousPlace":

                    break;

                case "DocumentsRequired":

                    break;

                case "Restaurant":

                    break;

                case "RestaurantFood":

                    break;

                case "BestMan_MaidOfHonour":

                    break;

                case "BridesmaidsGroomsmen":

                    break;

                case "Decoration":

                    break;

                case "FreshFlowers":

                    break;

                case "MusicLayout":

                    break;

                case "Photographer":

                    break;

                case "BrideAccessories":

                    break;

                case "BrideClothes":

                    break;

                case "GroomAccessories":

                    break;

                case "GroomClothes":

                    break;

                case "BMMOHAccessories":

                    break;

                case "BMMOHClothes":

                    break;

                case "BAGAccessories":

                    break;

                case "BAGClothes":

                    break;

                case "HoneymoonDestination":

                    break;

                case "GuestsList":

                    break;

                case "ForeignGuestsAccomodation":

                    break;

                case "HairdresserMakeupArtist":

                    break;

                case "Invitations":

                    break;

                case "PurchaseBrideAccessories":

                    break;

                case "PurchaseBrideClothes":

                    break;

                case "PurchaseGroomAccessories":

                    break;

                case "PurchaseGroomClothes":

                    break;

                case "PurchaseBMMOHAccessories":

                    break;

                case "PurchaseBMMOHClothes":

                    break;

                case "PurchaseBAGAccessories":

                    break;

                case "PurchaseBAGClothes":

                    break;

                case "PurchaseRestaurantFood":

                    break;

                case "PurchaseFreshFlowers":

                    break;

                case "PurchaseRings":

                    break;

                case "PurchaseCake":

                    break;

                case "BookMusicLayout":

                    break;

                case "BookPhotographer":

                    break;

                case "BookHoneymoonDestination":

                    break;

                case "BookGuestsAccomodation":

                    break;

                case "BookHairdresserMakeupArtistAppointments":

                    break;

                case "SendInvitations":

                    break;

                case "RestaurantAccomodationPlan":

                    break;
                    
                default:
                    break;

            }
        }

        private void CreateWeddingBudgetControl(object value)
        {
            var weddingBudget = new BudgetPicker(Convert.ToInt32(value));

            pivotStackPanel.Children.Add(weddingBudget);
        }

        private void ListBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BudgetPicker weddingBudget = new BudgetPicker();
            pivotStackPanel.Children.Add(weddingBudget);
        }
    }
}
