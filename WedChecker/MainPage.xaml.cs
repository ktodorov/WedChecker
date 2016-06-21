using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Pages;
using WedChecker.UserControls;
using WedChecker.UserControls.Elements;
using WedChecker.UserControls.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WedChecker
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private bool FirstTimeLaunched = true;

        public MainPage()
        {
            this.InitializeComponent();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            Loaded += MainPage_Loaded;

            mainTitleBar.BackButtonClick += MainTitleBar_BackButtonClick;

            this.RequestedTheme = Core.GetElementTheme();

            ApplicationData.Current.DataChanged += RoamingDataChanged;
        }

        private async void RoamingDataChanged(ApplicationData sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    await UpdateTasks();
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
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
            firstLaunchPopup.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetRow(firstLaunchPopup, 1);
            Grid.SetColumnSpan(firstLaunchPopup, 2);
            firstLaunchPopup.FinishedSubmitting += FirstLaunchPopup_FinishedSubmitting;
            mainGrid.Children.Add(firstLaunchPopup);
            mainTitleBar.RemoveSubTitle();
        }

        private async Task UpdateTasks()
        {
            mainTitleBar.ProgressActive = true;

            var task = AppData.PopulateAddedControls();
            await Task.WhenAll(task);
            var controls = task.Result;
            controls = controls.OrderBy(c => c.GetType().GetProperty("TaskName").GetValue(null, null).ToString()).ToList();

            var bookings = gvBookings.Items.OfType<PopulatedTask>().Where(p => !controls.Any(c => c.GetType() == p.ConnectedTaskControlType)).ToList();
            foreach (var booking in bookings)
            {
                gvBookings.Items.Remove(booking);
            }

            var planings = gvPlanings.Items.OfType<PopulatedTask>().Where(p => !controls.Any(c => c.GetType() == p.ConnectedTaskControlType)).ToList();
            foreach (var planing in planings)
            {
                gvPlanings.Items.Remove(planing);
            }

            var purchases = gvPurchases.Items.OfType<PopulatedTask>().Where(p => !controls.Any(c => c.GetType() == p.ConnectedTaskControlType)).ToList();
            foreach (var purchase in purchases)
            {
                gvPurchases.Items.Remove(purchase);
            }

            AddPopulatedControls(controls);

            tbGreetUser.Text = string.Format("Hello, {0}", Core.GetSetting("Name"));

            mainTitleBar.ProgressActive = false;
        }

        private async void FirstLaunchPopup_FinishedSubmitting(object sender, EventArgs e)
        {
            mainSplitView.Visibility = Visibility.Visible;
            mainTitleBar.SetSubTitle("HOME");
            tbCountdownTimer.UpdateTimeLeft();
            appBar.Visibility = Visibility.Visible;

            var firstLaunchPopup = sender as FirstLaunchPopup;
            mainGrid.Children.Remove(firstLaunchPopup);

            await UpdateTasks();
        }

        private void AddPopulatedControls(List<BaseTaskControl> populatedControls)
        {
            foreach (var populatedControl in populatedControls)
            {
                var type = populatedControl.GetType();
                if (!TaskAlreadyAdded(type))
                {
                    TaskData.InsertTaskControl(this, type, false, taskTapped);
                }
            }

            addTaskDialog.Visibility = Visibility.Collapsed;
            appBar.Visibility = Visibility.Visible;
        }

        private bool TaskAlreadyAdded(Type taskType)
        {
            var bookings = gvBookings.Items.OfType<PopulatedTask>().Where(p => p.ConnectedTaskControlType == taskType).ToList();
            if (bookings.Any())
            {
                return true;
            }

            var planings = gvPlanings.Items.OfType<PopulatedTask>().Where(p => p.ConnectedTaskControlType == taskType).ToList();
            if (planings.Any())
            {
                return true;
            }

            var purchases = gvPurchases.Items.OfType<PopulatedTask>().Where(p => p.ConnectedTaskControlType == taskType).ToList();
            if (purchases.Any())
            {
                return true;
            }

            return false;
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            tbCountdownTimer.UpdateTimeLeft();
        }

        private void TaskTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var created = TaskData.CreateTaskControl(this, addTaskDialog.TappedTaskName, taskTapped);
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
                    newPopulatedTask = gvBookings.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControlType.Name == addTaskDialog.TappedTaskName);
                }
                else if (taskCategory == TaskCategories.Planing)
                {
                    newPopulatedTask = gvPlanings.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControlType.Name == addTaskDialog.TappedTaskName);
                }
                else if (taskCategory == TaskCategories.Purchase)
                {
                    newPopulatedTask = gvPurchases.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControlType.Name == addTaskDialog.TappedTaskName);
                }
                taskTapped(newPopulatedTask, new TappedRoutedEventArgs());
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
                var baseTaskType = populatedTask.ConnectedTaskControlType;
                var baseTaskControl = Activator.CreateInstance(baseTaskType) as BaseTaskControl;

                var popupTask = new PopupTask(baseTaskControl, false);
                popupTask.SaveClick += PopupTask_SaveClick;
                popupTask.CancelClick += PopupTask_CancelClick;
                popupTask.TaskSizeChanged += PopupTask_TaskSizeChanged;

                appBar.Visibility = Visibility.Collapsed;
                mainSplitView.Pane.Visibility = Visibility.Collapsed;

                var windowWidth = Window.Current.Bounds.Width;
                var windowHeight = Window.Current.Bounds.Height;

                rectBackgroundHide.Visibility = Visibility.Visible;

                CalculateTaskSizes(windowWidth, windowHeight);

                taskPopup.Child = popupTask;
                taskPopup.IsOpen = true;
            }
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

        private void RepopulateGridChildren(ScrollViewer scrollViewer, int numberOfColumns)
        {
            var rootWidth = Window.Current.Bounds.Width - mainSplitView.CompactPaneLength;

            if (mainSplitView.IsPaneOpen)
            {
                rootWidth = Window.Current.Bounds.Width - mainSplitView.OpenPaneLength;
            }

            var taskWidth = rootWidth - 30;

            if (rootWidth > 720)
            {
                taskWidth = ((rootWidth) / numberOfColumns) - (numberOfColumns * 4);
            }

            var taskHeight = taskWidth / 1.5;
            if (rootWidth < 720)
            {
                taskHeight /= 2;
            }

            var grid = scrollViewer.Content as GridView;
            if (grid == null)
            {
                return;
            }

            var gridName = grid.Name;
            var tasks = grid.Items.OfType<PopulatedTask>();

            foreach (var task in tasks)
            {
                task.Width = taskWidth;
                task.Height = taskHeight;
            }
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
            panel.ItemHeight = panel.ItemWidth / 2;
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
    }
}
