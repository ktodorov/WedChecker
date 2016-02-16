using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WedChecker.UserControls;
using WedChecker.UserControls.Tasks;
using WedChecker.Common;
using Windows.Phone.UI.Input;
using System.Threading;
using WedChecker.WindowsPhoneControls;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using Windows.System;
using Windows.ApplicationModel.Store;

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
        private List<TaskListItem> PlanningTaskItems;
        private List<TaskListItem> PurchasingTaskItems;
        private List<TaskListItem> BookingTaskItems;
        private bool FirstTimeLaunched = true;

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

            LoadAdditionalData();
        }

        private void LoadAdditionalData()
        {
            PlanningTaskItems = TaskData.LoadPlanningTaskItems();
            foreach (var planItem in PlanningTaskItems)
            {
                var taskTile = new TaskTileControl();
                taskTile.TaskTitle = planItem.Title;
                taskTile.Name = planItem.TaskName;
                taskTile.Tapped += TaskTile_Tapped;

                gvPlanningTasks.Items.Add(taskTile);
            }

            PurchasingTaskItems = TaskData.LoadPurchasingTaskItems();
            foreach (var purchaseItem in PurchasingTaskItems)
            {
                var taskTile = new TaskTileControl();
                taskTile.TaskTitle = purchaseItem.Title;
                taskTile.Name = purchaseItem.TaskName;
                taskTile.Tapped += TaskTile_Tapped;

                gvPurchasingTasks.Items.Add(taskTile);
            }

            BookingTaskItems = TaskData.LoadBookingTaskItems();
            foreach (var bookItem in BookingTaskItems)
            {
                var taskTile = new TaskTileControl();
                taskTile.TaskTitle = bookItem.Title;
                taskTile.Name = bookItem.TaskName;
                taskTile.Tapped += TaskTile_Tapped;

                gvBookingTasks.Items.Add(taskTile);
            }
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            tbCountdownTimer.UpdateTimeLeft();
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (svMain.Visibility == Visibility.Visible)
            {
                svMain.Visibility = Visibility.Collapsed;
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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Loaded += async (sender, args) =>
            {
                if (!FirstTimeLaunched)
                {
                    return;
                }

                if (Core.IsFirstLaunch())
                {
                    var firstLaunchPopup = new FirstLaunchPopup();
                    firstLaunchPopup.VerticalAlignment = VerticalAlignment.Stretch;
                    LayoutRoot.Children.Add(firstLaunchPopup);
                }
                else
                {
                    loadControlsProgressRing.IsActive = true;
                    var task = AppData.PopulateAddedControls();
                    await Task.WhenAll(task);
                    var controls = task.Result;
                    controls = controls.OrderBy(c => c.TaskName).ToList();
                    AddPopulatedControls(controls);

                    tbGreetUser.Text = string.Format("Hello, {0}", Core.GetSetting("Name"));

                    loadControlsProgressRing.IsActive = false;
                }

                FirstTimeLaunched = false;
            };

            await this.RegisterBackgroundTask();
        }

        private const string taskName = "RemainingTimeBackgroundTask";
        private const string taskEntryPoint = "UniversalBackgroundTasks.RemainingTimeBackgroundTask";

        private async Task RegisterBackgroundTask()
        {
            var result = await BackgroundExecutionManager.RequestAccessAsync();
            if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                        task.Value.Unregister(true);
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            svMain.Visibility = Visibility.Visible;
            appBar.Visibility = Visibility.Collapsed;
            mainPivot.Visibility = Visibility.Collapsed;
        }

        private void AddPopulatedControls()
        {
            var populatedControls = AppData.AllTasks.GetAllTasks();

            foreach (var populatedControl in populatedControls)
            {
                TaskData.CreateTaskControl(this, populatedControl);
            }

            TaskData.DisableAddedTasks(gvPlanningTasks);
            TaskData.DisableAddedTasks(gvPurchasingTasks);
            TaskData.DisableAddedTasks(gvBookingTasks);
        }

        private void AddPopulatedControls(List<BaseTaskControl> populatedControls)
        {
            foreach (var populatedControl in populatedControls)
            {
                var type = populatedControl.GetType();
                TaskData.InsertTaskControl(this, type, populatedControl, false);
            }

            var firstLaunchPopup = LayoutRoot.Children.OfType<FirstLaunchPopup>().FirstOrDefault();
            if (firstLaunchPopup != null)
            {
                LayoutRoot.Children.Remove(firstLaunchPopup);
            }
            svMain.Visibility = Visibility.Collapsed;
            appBar.Visibility = Visibility.Visible;
            mainPivot.Visibility = Visibility.Visible;
            mainPivot.SelectedIndex = 1;

            TaskData.DisableAddedTasks(gvPlanningTasks);
            TaskData.DisableAddedTasks(gvPurchasingTasks);
            TaskData.DisableAddedTasks(gvBookingTasks);
        }

        private void TaskItem_Clicked(object sender, RoutedEventArgs e)
        {

        }
        private void TaskTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var senderElement = sender as TaskTileControl;
            if (senderElement == null || !senderElement.IsEnabled)
            {
                return;
            }

            var created = TaskData.CreateTaskControl(this, senderElement.Name);
            if (created)
            {
                senderElement.IsEnabled = false;
            }
        }

        private void AboutPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

        private void SettingsPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private async void RateReviewButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=8f849272-e314-42ee-81d5-0967f0034456"));
        }
    }
}
