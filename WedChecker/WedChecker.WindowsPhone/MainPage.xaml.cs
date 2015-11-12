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
using System.Threading;

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
        private CancellationTokenSource cts;

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

            cts = new CancellationTokenSource();
            App.ctsToUse = cts;
            AppData.CancelToken = cts.Token;
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
                    var controls = await AppData.DeserializeData();
                    AddPopulatedControls(controls);

                    tbGreetUser.Text = string.Format("Hello, {0}", Core.GetSetting("Name"));
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
                TaskData.CreateTaskControl(this, populatedControl);
            }

            TaskData.DisableAddedTasks(LbTasks);
        }

        private void AddPopulatedControls(List<BaseTaskControl> populatedControls)
        {
            foreach (var populatedControl in populatedControls)
            {
                var type = populatedControl.GetType();
                TaskData.InsertTaskControl(this, type, populatedControl);
                //spPlanings.Children.Add(new PopulatedTask(populatedControl, false));
                AppData.InsertSerializableTask(populatedControl);
            }
            firstLaunchPopup.Visibility = Visibility.Collapsed;
            LbTasks.Visibility = Visibility.Collapsed;
            appBar.Visibility = Visibility.Visible;
            mainPivot.Visibility = Visibility.Visible;
            mainPivot.SelectedIndex = 1;

            TaskData.DisableAddedTasks(LbTasks);
        }

        private void TaskItem_Clicked(object sender, RoutedEventArgs e)
        {
            var senderElement = sender as Button;
            if (senderElement == null)
            {
                return;
            }

            var taskClicked = new KeyValuePair<string, object>(senderElement.Name, null);
            TaskData.CreateTaskControl(this, taskClicked);
            senderElement.IsEnabled = false;
        }
    }
}
