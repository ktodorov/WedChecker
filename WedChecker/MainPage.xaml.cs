using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Helpers;
using WedChecker.Pages;
using WedChecker.UserControls;
using WedChecker.UserControls.Elements;
using WedChecker.UserControls.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WedChecker
{
    public sealed partial class MainPage : Page
    {
        private bool FirstTimeLaunched = true;
        private string arguments;
        private string textForShare;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;

            mainTitleBar.BackButtonClick += MainTitleBar_BackButtonClick;

            this.RequestedTheme = Core.GetElementTheme();

            ApplicationData.Current.DataChanged += RoamingDataChanged;
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
        }

        void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (!string.IsNullOrEmpty(textForShare))
            {
                args.Request.Data.SetText(textForShare);
                args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
            }
            else
            {
                args.Request.FailWithDisplayText("Nothing to share");
            }
        }

        public async void SwitchCategoryFromArguments(string givenArguments)
        {
            var taskCategory = TaskCategories.Home;
            switch (arguments)
            {
                case "plannings":
                    taskCategory = TaskCategories.Planing;
                    break;
                case "purchases":
                    taskCategory = TaskCategories.Purchase;
                    break;
                case "bookings":
                    taskCategory = TaskCategories.Booking;
                    break;
            }

            ChangeTaskCategory(taskCategory);
        }

        private async void RoamingDataChanged(ApplicationData sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    var firstLaunchPopup = this.FindName("firstLaunchPopup") as FirstLaunchPopup;
                    if (firstLaunchPopup != null && firstLaunchPopup.Visibility == Visibility.Visible)
                    {
                        var highPriority = AppData.GetRoamingSetting<bool?>("HighPriority");
                        if (highPriority.HasValue && highPriority.Value)
                        {
                            DoFirstLaunchPopupAfterFinishingLogic(firstLaunchPopup);
                        }
                    }

                    await UpdateTasks(true);
                }
            );
        }

        private void MainTitleBar_BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (addTaskDialog.Visibility == Visibility.Visible)
            {
                appBar.Visibility = Visibility.Visible;
                addTaskDialog.Visibility = Visibility.Collapsed;
                HamburgerButton.Visibility = Visibility.Visible;
                mainTitleBar.SetBackButtonVisible(false);
            }
            else if (taskPopup.Child != null)
            {
                var popupTask = taskPopup.Child as PopupTask;
                if (popupTask != null && !popupTask.InEditMode)
                {
                    HidePopupTask();
                }
            }
            else
            {
                throw new WedCheckerNavigationException();
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!FirstTimeLaunched)
            {
                return;
            }

            if (Core.IsFirstLaunch())
            {
                OpenFirstLaunchPopup();
            }
            else
            {
                await UpdateTasks();
            }

            FirstTimeLaunched = false;

            CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);

            if (!string.IsNullOrEmpty(arguments))
            {
                SwitchCategoryFromArguments(arguments);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                arguments = e.Parameter.ToString();
            }

            this.RegisterBackgroundTask();
        }

        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }

        private const string taskName = "RemainingTimeBackgroundProcess";
        private const string taskEntryPoint = "TileUpdateBackgroundProcess.RemainingTimeBackgroundProcess";

        private void OpenFirstLaunchPopup()
        {
            mainSplitView.Visibility = Visibility.Collapsed;
            var firstLaunchPopup = new FirstLaunchPopup();
            firstLaunchPopup.Name = "firstLaunchPopup";
            firstLaunchPopup.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetRow(firstLaunchPopup, 1);
            Grid.SetColumnSpan(firstLaunchPopup, 2);
            firstLaunchPopup.FinishedSubmitting += FirstLaunchPopup_FinishedSubmitting;
            hamburgerDesktopPanel.Visibility = Visibility.Collapsed;
            mainGrid.Children.Add(firstLaunchPopup);
            mainTitleBar.RemoveSubTitle();
        }

        private async Task UpdateTasks(bool alreadyCreated = false)
        {
            mainTitleBar.ProgressActive = true;

            var controls = await AppData.PopulateAddedControls();
            controls = controls.OrderBy(c => c.TaskName.ToString()).ToList();


            if (alreadyCreated)
            {
                // Delete tasks which were deleted on other machine
                var bookings = gvBookings.Items.OfType<PopulatedTask>().Where(p => !controls.Any(c => c.GetType() == p.ConnectedTaskControl.GetType())).ToList();
                foreach (var booking in bookings)
                {
                    gvBookings.Items.Remove(booking);
                }

                var planings = gvPlanings.Items.OfType<PopulatedTask>().Where(p => !controls.Any(c => c.GetType() == p.ConnectedTaskControl.GetType())).ToList();
                foreach (var planing in planings)
                {
                    gvPlanings.Items.Remove(planing);
                }

                var purchases = gvPurchases.Items.OfType<PopulatedTask>().Where(p => !controls.Any(c => c.GetType() == p.ConnectedTaskControl.GetType())).ToList();
                foreach (var purchase in purchases)
                {
                    gvPurchases.Items.Remove(purchase);
                }

                // Refresh the summary data for each remaining task
                var remainingBookings = gvBookings.Items.OfType<PopulatedTask>().Except(bookings);
                foreach (var remainingBooking in remainingBookings)
                {
                    remainingBooking.RefreshTaskSummary();
                }

                var remainingPlanings = gvPlanings.Items.OfType<PopulatedTask>().Except(planings);
                foreach (var remainingPlaning in remainingPlanings)
                {
                    remainingPlaning.RefreshTaskSummary();
                }

                var remainingPurchases = gvPurchases.Items.OfType<PopulatedTask>().Except(purchases);
                foreach (var remainingPurchase in remainingPurchases)
                {
                    remainingPurchase.RefreshTaskSummary();
                }
            }

            AddPopulatedControls(controls);

            tbGreetUser.Text = $"Hello, {Core.GetSetting("Name")}";
            tbGreetUser.Visibility = Visibility.Visible;

            tasksSummary.LoadTasksData(controls);

            mainTitleBar.ProgressActive = false;
        }

        private void DoFirstLaunchPopupAfterFinishingLogic(FirstLaunchPopup firstLaunchPopup)
        {
            mainSplitView.Visibility = Visibility.Visible;
            mainTitleBar.SetSubTitle("HOME");
            appBar.Visibility = Visibility.Visible;
            hamburgerDesktopPanel.Visibility = Visibility.Visible;

            mainGrid.Children.Remove(firstLaunchPopup);
        }

        private async void FirstLaunchPopup_FinishedSubmitting(object sender, EventArgs e)
        {
            var firstLaunchPopup = sender as FirstLaunchPopup;
            DoFirstLaunchPopupAfterFinishingLogic(firstLaunchPopup);

            await UpdateTasks();
        }

        private void AddPopulatedControls(List<BaseTaskControl> populatedControls)
        {
            foreach (var populatedControl in populatedControls)
            {
                var type = populatedControl.GetType();
                if (!TaskAlreadyAdded(type))
                {
                    TaskData.InsertTaskControl(this, populatedControl, false, taskTapped, onTaskEdit, onTaskDelete, onTaskShare);
                }
            }

            tasksSummary.LoadTasksData(populatedControls);

            addTaskDialog.Visibility = Visibility.Collapsed;
            appBar.Visibility = Visibility.Visible;
        }

        private bool TaskAlreadyAdded(Type taskType)
        {
            var bookings = gvBookings.Items.OfType<PopulatedTask>().Where(p => p.ConnectedTaskControl.GetType() == taskType).ToList();
            if (bookings.Any())
            {
                return true;
            }

            var planings = gvPlanings.Items.OfType<PopulatedTask>().Where(p => p.ConnectedTaskControl.GetType() == taskType).ToList();
            if (planings.Any())
            {
                return true;
            }

            var purchases = gvPurchases.Items.OfType<PopulatedTask>().Where(p => p.ConnectedTaskControl.GetType() == taskType).ToList();
            if (purchases.Any())
            {
                return true;
            }

            return false;
        }

        private PopulatedTask GetPopulatedTaskByType(Type taskType)
        {
            var bookingTask = gvBookings.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType() == taskType);
            if (bookingTask != null)
            {
                return bookingTask;
            }

            var planingTask = gvPlanings.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType() == taskType);
            if (planingTask != null)
            {
                return planingTask;
            }

            var purchaseTask = gvPurchases.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType() == taskType);
            if (purchaseTask != null)
            {
                return purchaseTask;
            }

            return null;
        }

        private async void TaskTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var taskType = TaskData.GetTaskType(addTaskDialog.TappedTaskName);
            var taskControl = await TaskData.CreateTaskControlByType(taskType);
            if (taskControl == null)
            {
                return;
            }

            var created = TaskData.CreateTaskControl(this, taskControl, taskTapped, onTaskEdit, onTaskDelete, onTaskShare);
            if (created)
            {
                addTaskDialog.Visibility = Visibility.Collapsed;
                appBar.Visibility = Visibility.Visible;
                HamburgerButton.Visibility = Visibility.Visible;
                mainTitleBar.SetBackButtonVisible(false);

                var taskCategory = addTaskDialog.TappedTaskCategory;
                ChangeTaskCategory(taskCategory);

                CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);

                addTaskDialog.IsTileEnabled(addTaskDialog.TappedTaskName, false);

                PopulatedTask newPopulatedTask = null;
                if (taskCategory == TaskCategories.Booking)
                {
                    newPopulatedTask = gvBookings.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType().Name == addTaskDialog.TappedTaskName);
                }
                else if (taskCategory == TaskCategories.Planing)
                {
                    newPopulatedTask = gvPlanings.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType().Name == addTaskDialog.TappedTaskName);
                }
                else if (taskCategory == TaskCategories.Purchase)
                {
                    newPopulatedTask = gvPurchases.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType().Name == addTaskDialog.TappedTaskName);
                }
                taskTapped(newPopulatedTask, new TappedRoutedEventArgs());

                tasksSummary.AddNewTaskInfo(taskControl);
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            addTaskDialog.Visibility = Visibility.Visible;
            mainTitleBar.SetBackButtonVisible(true);
            appBar.Visibility = Visibility.Collapsed;
            HamburgerButton.Visibility = Visibility.Collapsed;
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

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;

            if (mainSplitView.DisplayMode != SplitViewDisplayMode.CompactOverlay)
            {
                CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
            }
        }

        private void taskTapped(object sender, TappedRoutedEventArgs e)
        {
            var populatedTask = sender as PopulatedTask;

            if (populatedTask != null)
            {
                var popupTask = CreatePopupTaskFromPopulatedTask(populatedTask);

                taskPopup.Child = popupTask;
                taskPopup.IsOpen = true;
            }
        }

        private PopupTask CreatePopupTaskFromPopulatedTask(PopulatedTask populatedTask)
        {
            var baseTaskType = populatedTask.ConnectedTaskControl.GetType();
            var baseTaskControl = Activator.CreateInstance(baseTaskType) as BaseTaskControl;

            var popupTask = new PopupTask(baseTaskControl, false);
            popupTask.SaveClick += PopupTask_SaveClick;
            popupTask.CancelClick += PopupTask_CancelClick;
            popupTask.OnDelete += onTaskDelete;
            popupTask.OnEdit += onTaskEdit;
            popupTask.OnShare += onTaskShare;
            popupTask.TaskSizeChanged += PopupTask_TaskSizeChanged;

            appBar.Visibility = Visibility.Collapsed;
            mainSplitView.Pane.Visibility = Visibility.Collapsed;

            var windowWidth = Window.Current.Bounds.Width;
            var windowHeight = Window.Current.Bounds.Height;

            rectBackgroundHide.Visibility = Visibility.Visible;

            CalculateTaskSizes(windowWidth, windowHeight);

            taskPopup.Child = popupTask;
            taskPopup.IsOpen = true;

            return popupTask;
        }

        private async void onTaskEdit(object sender, EventArgs e)
        {
            var populatedTask = sender as PopulatedTask;
            PopupTask popupTask = null;
            if (sender is PopulatedTask)
            {
                popupTask = CreatePopupTaskFromPopulatedTask(populatedTask);
            }
            else if (sender is PopupTask)
            {
                popupTask = sender as PopupTask;
            }

            if (popupTask == null)
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100));
            await popupTask.Edit();
        }

        private async void onTaskDelete(object sender, EventArgs e)
        {
            BaseTaskControl connectedControl;
            if (sender is PopulatedTask)
            {
                var populatedTask = sender as PopulatedTask;
                connectedControl = populatedTask.ConnectedTaskControl;
                if (connectedControl == null)
                {
                    return;
                }

                populatedTask.RefreshTaskSummary(connectedControl);
            }
            else if (sender is PopupTask)
            {
                var popupTask = sender as PopupTask;
                if (popupTask == null || popupTask.ConnectedTaskControl == null)
                {
                    return;
                }

                connectedControl = popupTask.ConnectedTaskControl;

                var populatedTask = GetPopulatedTaskByType(connectedControl.GetType());
                if (populatedTask != null)
                {
                    populatedTask.RefreshTaskSummary(connectedControl);
                }
            }
            else
            {
                return;
            }

            var frameworkSender = sender as FrameworkElement;

            var deleted = await TasksOperationsHelper.DeleteTaskAsync(this, connectedControl);
            if (sender is PopupTask && deleted)
            {
                var popupTask = sender as PopupTask;
                popupTask.Cancel();
            }

            tasksSummary.RemoveTaskInfo(connectedControl);
        }

        private void onTaskShare(object sender, EventArgs e)
        {
            BaseTaskControl connectedControl = null;
            if (sender is PopulatedTask)
            {
                var populatedTask = sender as PopulatedTask;
                connectedControl = populatedTask.ConnectedTaskControl;
            }
            else if (sender is PopupTask)
            {
                var popupTask = sender as PopupTask;
                connectedControl = popupTask.ConnectedTaskControl;
            }

            if (connectedControl == null)
            {
                return;
            }

            textForShare = connectedControl.GetDataAsText();

            DataTransferManager.ShowShareUI();
        }

        private void PopupTask_TaskSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePopup();
        }

        private void PopupTask_CancelClick(object sender, RoutedEventArgs e)
        {
            HidePopupTask();
        }

        private void PopupTask_SaveClick(object sender, RoutedEventArgs e)
        {
            HidePopupTask();

            var popupTask = sender as PopupTask;
            var updatedTask = popupTask.ConnectedTaskControl;
            tasksSummary.UpdateTaskInfo(updatedTask);

            var populatedTask = GetPopulatedTaskByType(updatedTask.GetType());
            if (populatedTask != null)
            {
                populatedTask.RefreshTaskSummary(updatedTask);
            }
        }

        private void rectBackgroundHide_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (taskPopup.Child != null)
            {
                var popupTask = taskPopup.Child as PopupTask;
                if (popupTask != null && !popupTask.InEditMode)
                {
                    HidePopupTask();
                }
            }
        }

        private void HidePopupTask()
        {
            rectBackgroundHide.Visibility = Visibility.Collapsed;
            taskPopup.IsOpen = false;
            taskPopup.Child = null;
            appBar.Visibility = Visibility.Visible;
            mainSplitView.Pane.Visibility = Visibility.Visible;
        }

        private void taskPopup_LayoutUpdated(object sender, object e)
        {
            if (taskPopup.Child == null || !(taskPopup.Child is PopupTask))
            {
                return;
            }

            var popupTask = taskPopup.Child as PopupTask;

            double ActualHorizontalOffset = taskPopup.HorizontalOffset;
            double ActualVerticalOffset = taskPopup.VerticalOffset;

            double NewHorizontalOffset = ((LayoutRoot.ActualWidth - popupTask.ActualWidth) / 2);
            double NewVerticalOffset = (LayoutRoot.ActualHeight - popupTask.ActualHeight) / 2;

            if (ActualHorizontalOffset != NewHorizontalOffset || ActualVerticalOffset != NewVerticalOffset)
            {
                taskPopup.HorizontalOffset = NewHorizontalOffset;
                taskPopup.VerticalOffset = NewVerticalOffset;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateTaskSizes(e.NewSize.Width, e.NewSize.Height);
        }

        private async void CalculateTaskSizes(double width, double height)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
                {
                    rectBackgroundHide.Height = height;
                    rectBackgroundHide.Width = width;

                    if (Window.Current.Bounds.Width < 720 && stackPane.Children.Contains(HamburgerButton))
                    {
                        stackPane.Children.Remove(HamburgerButton);
                        hamburgerDesktopPanel.Children.Add(HamburgerButton);
                    }
                    else if (Window.Current.Bounds.Width > 720 && !stackPane.Children.Contains(HamburgerButton))
                    {
                        hamburgerDesktopPanel.Children.Remove(HamburgerButton);
                        stackPane.Children.Insert(0, HamburgerButton);
                    }
                }
            );
        }

        private async void ChangeTaskCategory(TaskCategories category)
        {
            if (Window.Current.Bounds.Width < 1024)
            {
                mainSplitView.IsPaneOpen = false;
            }

            mainTitleBar.ProgressActive = true;

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
                {
                    mainTitleBar.SetSubTitle(category.ToString().ToUpper());

                    spHome.Visibility = IsVisible(category == TaskCategories.Home);
                    svPlanings.Visibility = IsVisible(category == TaskCategories.Planing);
                    svBookings.Visibility = IsVisible(category == TaskCategories.Booking);
                    svPurchases.Visibility = IsVisible(category == TaskCategories.Purchase);

                    optionsListView.SelectedIndex = (int)category;
                }
            );

            mainTitleBar.ProgressActive = false;
        }

        private Visibility IsVisible(bool value)
        {
            if (value)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePopup();
        }

        private void ResizePopup()
        {
            if (taskPopup.Child == null || !(taskPopup.Child is PopupTask))
            {
                return;
            }

            var popupTask = taskPopup.Child as PopupTask;
            if (popupTask == null)
            {
                return;
            }

            double ActualHorizontalOffset = taskPopup.HorizontalOffset;
            double ActualVerticalOffset = taskPopup.VerticalOffset;

            var windowHeight = mainSplitView.ActualHeight;
            var windowWidth = mainSplitView.ActualWidth;

            if (windowWidth < 720)
            {
                windowHeight = windowHeight + mainTitleBar.ActualHeight;
            }

            popupTask.ResizeContent(windowWidth, windowHeight, mainTitleBar);

            double NewHorizontalOffset = ((windowWidth - popupTask.ActualWidth) / 2);
            double NewVerticalOffset = (windowHeight - popupTask.ActualHeight) / 2;

            if (ActualHorizontalOffset != NewHorizontalOffset || ActualVerticalOffset != NewVerticalOffset)
            {
                taskPopup.HorizontalOffset = NewHorizontalOffset;
                taskPopup.VerticalOffset = NewVerticalOffset;
            }
        }

        private void gridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = Window.Current.Bounds.Width;

            var columns = (int)Math.Round(width / 400.0);
            if (width < 720)
            {
                columns = 1;
            }

            var gridView = sender as GridView;
            if (gridView == null)
            {
                return;
            }

            var panel = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            var itemWidth = e.NewSize.Width / columns;
            panel.ItemWidth = itemWidth;
            var newHeight = panel.ItemWidth / 2;
            if (newHeight < 200)
            {
                newHeight = 200;
            }
            panel.ItemHeight = newHeight;
        }

        private void HamburgerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;

            if (mainSplitView.DisplayMode != SplitViewDisplayMode.CompactOverlay)
            {
                CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
            }
        }

        private void optionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedButton = e.AddedItems[0] as MetroDesignLanguageTextButton;

            if (selectedButton.Tag == null)
            {
                return;
            }

            var category = (TaskCategories)Enum.Parse(typeof(TaskCategories), selectedButton.Tag.ToString());

            ChangeTaskCategory(category);
        }

        private void tbCountdownTimer_WeddingPassed(object sender, EventArgs e)
        {
            tbWeddingPassed.Visibility = Visibility.Visible;
            tbCountdownTimer.Visibility = Visibility.Collapsed;
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                PopupTask_CancelClick(null, new RoutedEventArgs());
            }
        }
    }
}
