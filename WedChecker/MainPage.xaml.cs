﻿using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Helpers;
using WedChecker.Infrastructure;
using WedChecker.Interfaces;
using WedChecker.Pages;
using WedChecker.UserControls;
using WedChecker.UserControls.Elements;
using WedChecker.UserControls.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WedChecker
{
    public sealed partial class MainPage : Page
    {
        private bool FirstTimeLaunched = true;
        private string arguments;
        private bool inSelectionMode = false;

        List<ICommandBarElement> secondaryCommands;

        public TaskCategories CurrentPageCategory
        {
            get
            {
                var page = contentFrame?.Content as IUpdateableTasks;
                if (page != null)
                {
                    return page.TasksCategory;
                }

                return TaskCategories.Home;
            }
        }

        public List<Page> FrameContents = new List<Page>();

        public IUpdateableTasks CurrentContentPage
        {
            get
            {
                var page = contentFrame?.Content as IUpdateableTasks;
                if (page != null)
                {
                    return page;
                }

                throw new Exception("Something happened...");
            }
        }

        public WedCheckerTitleBar MainTitleBar
        {
            get
            {
                return mainTitleBar;
            }
        }

        public CommandBar AppBar
        {
            get
            {
                return appBar;
            }
        }

        public SplitView MainSplitView
        {
            get
            {
                return mainSplitView;
            }
        }

        public Popup TaskPopup
        {
            get
            {
                return taskPopup;
            }
        }

        public CanvasControl RectBackgroundHide
        {
            get
            {
                return rectBackgroundHide;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                InitializeFrostedGlass();
            }
            catch
            {
                // If we're here, that probably means user system doesn't support aero glass effects
                // No matter the reason, we continue without glass effects
            }

            Loaded += MainPage_Loaded;

            mainTitleBar.BackButtonClick += MainTitleBar_BackButtonClick;

            this.RequestedTheme = Core.GetElementTheme();

            ApplicationData.Current.DataChanged += RoamingDataChanged;
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
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
                if (!string.IsNullOrEmpty(arguments))
                {
                    SwitchCategoryFromArguments(arguments);
                }
                else
                {
                    var category = AppData.GetLocalSetting<int?>("CurrentCategory");
                    if (category.HasValue)
                    {
                        await ChangeTaskCategory((TaskCategories)category.Value);
                        AppData.RemoveLocalSetting("CurrentCategory");
                    }
                    else
                    {
                        await ChangeTaskCategory(TaskCategories.Home);
                    }
                }

                await UpdateTasks();
            }

            FirstTimeLaunched = false;

            CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
        }

        void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (!string.IsNullOrEmpty(AppData.TextForShare))
            {
                args.Request.Data.SetText(AppData.TextForShare);
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
            if (string.IsNullOrEmpty(arguments))
            {
                var category = AppData.GetLocalSetting<int?>("CurrentCategory");
                if (category.HasValue)
                {
                    taskCategory = (TaskCategories)category.Value;
                    AppData.RemoveLocalSetting("CurrentCategory");
                }
            }
            else
            {
                switch (arguments)
                {
                    case "plannings":
                        taskCategory = TaskCategories.Planning;
                        break;
                    case "purchases":
                        taskCategory = TaskCategories.Purchase;
                        break;
                    case "bookings":
                        taskCategory = TaskCategories.Booking;
                        break;
                }
            }

            await ChangeTaskCategory(taskCategory);
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
            else if (CurrentPageCategory != TaskCategories.Home && taskPopup.Child != null)
            {
                var popupTask = taskPopup.Child as PopupTask;
                if (popupTask != null && !popupTask.InEditMode)
                {
                    HidePopupTask();
                }
            }
            else if (inSelectionMode)
            {
                LeaveSelectionMode();
            }
            else
            {
                throw new WedCheckerNavigationException();
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
            this.RegisterNotificationBackgroundTask();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            LeaveSelectionMode();

            AppData.InsertLocalSetting("CurrentCategory", (int)CurrentPageCategory);
        }


        private const string taskName = "RemainingTimeBackgroundProcess";
        private const string taskEntryPoint = "TileUpdateBackgroundProcess.RemainingTimeBackgroundProcess";

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


        private const string notificationsTaskName = "NotificationBackgroundProcess";
        private const string notificationsTaskEntryPoint = "TileUpdateBackgroundProcess.NotificationBackgroundProcess";

        private async void RegisterNotificationBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == notificationsTaskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = notificationsTaskName;
                taskBuilder.TaskEntryPoint = notificationsTaskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(1500, false));
                var registration = taskBuilder.Register();
            }
        }


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

            addTaskDialog.Visibility = Visibility.Collapsed;
            appBar.Visibility = Visibility.Visible;
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

        private async void TaskTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var taskCategory = addTaskDialog.TappedTaskCategory;
            await ChangeTaskCategory(taskCategory);

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            var page = CurrentContentPage as TasksViewPage;
            var created = await page.CreateTask(addTaskDialog.TappedTaskName);
            if (!created)
            {
                return;
            }

            addTaskDialog.Visibility = Visibility.Collapsed;
            appBar.Visibility = Visibility.Visible;
            HamburgerButton.Visibility = Visibility.Visible;
            mainTitleBar.SetBackButtonVisible(false);

            CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);

            addTaskDialog.IsTileEnabled(addTaskDialog.TappedTaskName, false);
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            addTaskDialog.Visibility = Visibility.Visible;
            mainTitleBar.SetBackButtonVisible(true);
            appBar.Visibility = Visibility.Collapsed;
            HamburgerButton.Visibility = Visibility.Collapsed;
            if (CurrentPageCategory != TaskCategories.Home)
            {
                (CurrentContentPage as TasksViewPage).AttachedTasksViewer.LeaveSelectionMode();
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

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;

            if (mainSplitView.DisplayMode != SplitViewDisplayMode.CompactOverlay)
            {
                CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateTaskSizes(e.NewSize.Width, e.NewSize.Height);
        }

        public async void CalculateTaskSizes(double width, double height)
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

        private async Task ChangeTaskCategory(TaskCategories category)
        {
            if (CurrentPageCategory == category && contentFrame != null && contentFrame.Content != null)
            {
                return;
            }

            if (CurrentPageCategory != TaskCategories.Home)
            {
                LeaveSelectionMode();
            }

            if (Window.Current.Bounds.Width < 1024)
            {
                mainSplitView.IsPaneOpen = false;
            }

            mainTitleBar.ProgressActive = true;
            optionsListView.IsEnabled = false;

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
                {
                    mainTitleBar.SetSubTitle(category.ToString().ToUpper());

                    if (category == TaskCategories.Home)
                    {
                        if (FrameContents.OfType<HomePage>().Any())
                        {
                            var storedPage = FrameContents.OfType<HomePage>().FirstOrDefault();
                            contentFrame.Content = storedPage;
                        }
                        else
                        {
                            contentFrame.Navigate(typeof(HomePage));
                            FrameContents.Add(CurrentContentPage as Page);
                        }
                        (CurrentContentPage as HomePage).PageLoaded += CurrentContentPage_PageLoaded;
                        selectAppBarButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (FrameContents.OfType<TasksViewPage>().Any(p => p.TasksCategory == category))
                        {
                            var storedPage = FrameContents.OfType<TasksViewPage>().FirstOrDefault(p => p.TasksCategory == category);
                            contentFrame.Content = storedPage;
                        }
                        else
                        {
                            contentFrame.Navigate(typeof(TasksViewPage), new Params() { CurrentPage = this, Category = category });
                            FrameContents.Add(CurrentContentPage as Page);
                        }

                        (CurrentContentPage as TasksViewPage).PageLoaded += CurrentContentPage_PageLoaded;
                        selectAppBarButton.Visibility = Visibility.Visible;
                    }
                }
            );

            optionsListView.SelectedIndex = (int)category;
        }

        private void CurrentContentPage_PageLoaded(object sender, EventArgs e)
        {
            optionsListView.IsEnabled = true;
            mainTitleBar.ProgressActive = false;
        }

        private void HamburgerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;

            if (mainSplitView.DisplayMode != SplitViewDisplayMode.CompactOverlay)
            {
                CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
            }
        }

        private async void optionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedButton = e.AddedItems[0] as MetroDesignLanguageTextButton;

            if (selectedButton.Tag == null)
            {
                return;
            }

            var category = (TaskCategories)Enum.Parse(typeof(TaskCategories), selectedButton.Tag.ToString());

            await ChangeTaskCategory(category);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape && CurrentPageCategory != TaskCategories.Home)
            {
                HidePopupTask();
            }
        }

        public void HidePopupTask()
        {
            rectBackgroundHide.Visibility = Visibility.Collapsed;
            taskPopup.IsOpen = false;
            taskPopup.Child = null;

            appBar.Visibility = Visibility.Visible;
            mainSplitView.Pane.Visibility = Visibility.Visible;
        }

        public void ResizePopup()
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

            popupTask.ResizeContent(windowWidth, windowHeight);

            double NewHorizontalOffset = ((windowWidth - popupTask.ActualWidth) / 2);
            double NewVerticalOffset = (windowHeight - popupTask.ActualHeight) / 2;

            if (ActualHorizontalOffset != NewHorizontalOffset || ActualVerticalOffset != NewVerticalOffset)
            {
                taskPopup.HorizontalOffset = NewHorizontalOffset;
                taskPopup.VerticalOffset = NewVerticalOffset;
            }
        }

        private void contentFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePopup();
        }

        private void rectBackgroundHide_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePopup();
        }

        private void ShareAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            TasksOperationsHelper.ShareAllTasks();
        }

        private void ExportAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            TasksOperationsHelper.ExportAllTasks();
        }

        public void EnterSelectionMode()
        {
            inSelectionMode = true;
            shareAppBarButton.Visibility = Visibility.Visible;
            exportAppBarButton.Visibility = Visibility.Visible;
            deleteAppBarButton.Visibility = Visibility.Visible;
            cancelSelectAppBarButton.Visibility = Visibility.Visible;
            cancelSelectSeparator.Visibility = Visibility.Visible;
            selectAppBarButton.Visibility = Visibility.Collapsed;
            addAppBarButton.Visibility = Visibility.Collapsed;
            secondaryCommands = appBar.SecondaryCommands.ToList();
            appBar.SecondaryCommands.Clear();
            appBar.IsSticky = true;
            appBar.IsOpen = true;
            var currentPage = CurrentContentPage as TasksViewPage;
            if (currentPage == null)
            {
                return;
            }

            currentPage.AttachedTasksViewer?.EnterSelectionMode();
        }

        public void LeaveSelectionMode()
        {
            inSelectionMode = false;
            shareAppBarButton.Visibility = Visibility.Collapsed;
            exportAppBarButton.Visibility = Visibility.Collapsed;
            deleteAppBarButton.Visibility = Visibility.Collapsed;
            cancelSelectAppBarButton.Visibility = Visibility.Collapsed;
            cancelSelectSeparator.Visibility = Visibility.Collapsed;
            selectAppBarButton.Visibility = Visibility.Visible;
            addAppBarButton.Visibility = Visibility.Visible;
            appBar.IsSticky = false;
            appBar.IsOpen = false;

            if (!appBar.SecondaryCommands.Any())
            {
                foreach (var command in secondaryCommands)
                {
                    appBar.SecondaryCommands.Add(command);
                }
            }

            var currentPage = CurrentContentPage as TasksViewPage;
            if (currentPage == null)
            {
                return;
            }

            currentPage.AttachedTasksViewer?.LeaveSelectionMode();
        }

        private void ShareBarButton_Click(object sender, RoutedEventArgs e)
        {
            var currentPage = CurrentContentPage as TasksViewPage;
            if (currentPage == null)
            {
                return;
            }

            currentPage.AttachedTasksViewer.ShareSelected();
        }

        private void ExportBarButton_Click(object sender, RoutedEventArgs e)
        {
            var currentPage = CurrentContentPage as TasksViewPage;
            if (currentPage == null)
            {
                return;
            }

            currentPage.AttachedTasksViewer.ExportSelected();
        }
        private void DeleteBarButton_Click(object sender, RoutedEventArgs e)
        {
            var currentPage = CurrentContentPage as TasksViewPage;
            if (currentPage == null)
            {
                return;
            }

            currentPage.AttachedTasksViewer.DeleteSelected();
        }

        private void selectAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            EnterSelectionMode();
        }

        private void cancelSelectAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            LeaveSelectionMode();
        }

        private void InitializeFrostedGlass()
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(rectBackgroundHide);
            Compositor compositor = hostVisual.Compositor;

            // Create a glass effect, requires Win2D NuGet package
            var glassEffect = new GaussianBlurEffect
            {
                BlurAmount = 15.0f,
                BorderMode = EffectBorderMode.Hard,
                Source = new ArithmeticCompositeEffect
                {
                    MultiplyAmount = 0,
                    Source1Amount = 0.5f,
                    Source2Amount = 0.5f,
                    Source1 = new CompositionEffectSourceParameter("backdropBrush"),
                    Source2 = new ColorSourceEffect
                    {
                        //Color = Color.FromArgb(255, 245, 245, 245)
                        Color = Color.FromArgb(255, 25, 25, 25)
                    }
                }
            };

            //  Create an instance of the effect and set its source to a CompositionBackdropBrush
            var effectFactory = compositor.CreateEffectFactory(glassEffect);
            var backdropBrush = compositor.CreateBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();

            effectBrush.SetSourceParameter("backdropBrush", backdropBrush);

            // Create a Visual to contain the frosted glass effect
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = effectBrush;

            // Add the blur as a child of the host in the visual tree
            ElementCompositionPreview.SetElementChildVisual(rectBackgroundHide, glassVisual);

            // Make sure size of glass host and glass visual always stay in sync
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);

            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }

        void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.DrawEllipse(155, 115, 80, 30, Colors.Black, 3);
            args.DrawingSession.DrawText("Hello, world!", 100, 100, Colors.Yellow);
        }

    }
}
