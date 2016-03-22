using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.UserControls;
using WedChecker.UserControls.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WedChecker
{
	public enum TaskCategories
	{
		Home = 0,
		Planing = 1,
		Purchase = 2,
		Booking = 3
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		//private NavigationHelper navigationHelper;
		//private ObservableDictionary defaultViewModel = new ObservableDictionary();
		private DispatcherTimer dispatcherTimer = new DispatcherTimer();
		//private CancellationTokenSource cts;
		private List<TaskListItem> PlanningTaskItems;
		private List<TaskListItem> PurchasingTaskItems;
		private List<TaskListItem> BookingTaskItems;
		private bool FirstTimeLaunched = true;

		public MainPage()
		{
			this.InitializeComponent();
			//this.navigationHelper = new NavigationHelper(this);
			//this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
			//this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
			//this.NavigationCacheMode = NavigationCacheMode.Required;
			//HardwareButtons.BackPressed += HardwareButtons_BackPressed;
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();

			SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
			{
				if (svMain.Visibility == Visibility.Visible)
				{
					appBar.Visibility = Visibility.Visible;
					//mainPivot.Visibility = Visibility.Visible;
					svMain.Visibility = Visibility.Collapsed;
					SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
				}
				else if (Frame.CanGoBack)
				{
					Frame.GoBack();
					a.Handled = true;
				}
			};

			CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

			var applicationView = ApplicationView.GetForCurrentView();
			var titleBar = applicationView.TitleBar;
			titleBar.ButtonBackgroundColor = Colors.Transparent;
			titleBar.ButtonForegroundColor = Colors.Black;

			//cts = new CancellationTokenSource();
			//App.ctsToUse = cts;
			//AppData.CancelToken = cts.Token;

			LoadAdditionalData();

			Loaded += MainPage_Loaded;
		}

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!FirstTimeLaunched)
			{
				return;
			}

			await this.RegisterBackgroundTask();

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

			CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
		}

		private const string taskName = "RemainingTimeBackgroundTask";
		private const string taskEntryPoint = "UniversalBackgroundTasks.RemainingTimeBackgroundTask";

		private async Task RegisterBackgroundTask()
		{
			//var result = await BackgroundExecutionManager.RequestAccessAsync();
			//if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
			//    result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
			//{
			//    foreach (var task in BackgroundTaskRegistration.AllTasks)
			//    {
			//        if (task.Value.Name == taskName)
			//            task.Value.Unregister(true);
			//    }

			//    BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
			//    taskBuilder.Name = taskName;
			//    taskBuilder.TaskEntryPoint = taskEntryPoint;
			//    taskBuilder.SetTrigger(new TimeTrigger(15, false));
			//    var registration = taskBuilder.Register();
			//}
		}

		private void AddPopulatedControls(List<BaseTaskControl> populatedControls)
		{
			foreach (var populatedControl in populatedControls)
			{
				var type = populatedControl.GetType();
				TaskData.InsertTaskControl(this, type, populatedControl, false, taskTapped);
			}

			//var firstLaunchPopup = LayoutRoot.Children.OfType<FirstLaunchPopup>().FirstOrDefault();
			//if (firstLaunchPopup != null)
			//{
			//    LayoutRoot.Children.Remove(firstLaunchPopup);
			//}
			svMain.Visibility = Visibility.Collapsed;
			appBar.Visibility = Visibility.Visible;
			//mainPivot.Visibility = Visibility.Visible;
			//mainPivot.SelectedIndex = 0;


			TaskData.DisableAddedTasks(gvPlanningTasks);
			TaskData.DisableAddedTasks(gvPurchasingTasks);
			TaskData.DisableAddedTasks(gvBookingTasks);
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

			var created = TaskData.CreateTaskControl(this, senderElement.Name, taskTapped);
			if (created)
			{
				senderElement.IsEnabled = false;

				svMain.Visibility = Visibility.Collapsed;
				appBar.Visibility = Visibility.Visible;

				ChangeTaskCategoryAccordingToTile(senderElement);
				CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
			}
		}

		private void ChangeTaskCategoryAccordingToTile(TaskTileControl tile)
		{
			var taskCategory = TaskCategories.Home;

			if (tile.Parent == gvBookingTasks)
			{
				taskCategory = TaskCategories.Booking;
			}
			else if (tile.Parent == gvPlanningTasks)
			{
				taskCategory = TaskCategories.Planing;
			}
			else if (tile.Parent == gvPurchasingTasks)
			{
				taskCategory = TaskCategories.Purchase;
			}

			ChangeTaskCategory(taskCategory);
		}

		private void AddTaskButton_Click(object sender, RoutedEventArgs e)
		{
			svMain.Visibility = Visibility.Visible;
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
			appBar.Visibility = Visibility.Collapsed;
		}

		private void AboutPageButton_Click(object sender, RoutedEventArgs e)
		{
			// this.Frame.Navigate(typeof(AboutPage));
		}

		private void SettingsPageButton_Click(object sender, RoutedEventArgs e)
		{
			//this.Frame.Navigate(typeof(SettingsPage));
		}

		private async void RateReviewButton_Click(object sender, RoutedEventArgs e)
		{
			await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=8f849272-e314-42ee-81d5-0967f0034456"));
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
		}

		private void taskTapped(object sender, TappedRoutedEventArgs e)
		{
			var populatedTask = sender as PopulatedTask;

			if (populatedTask != null)
			{
				var baseTaskType = populatedTask.ConnectedTaskControl.GetType();
				var baseTaskControl = Activator.CreateInstance(baseTaskType) as BaseTaskControl;

				var popupTask = new PopupTask(baseTaskControl, false);
				popupTask.SaveClick += PopupTask_SaveClick;
				popupTask.CancelClick += PopupTask_CancelClick;

				appBar.Visibility = Visibility.Collapsed;
				MySplitView.Pane.Visibility = Visibility.Collapsed;

				var windowWidth = Window.Current.Bounds.Width;
				var windowHeight = Window.Current.Bounds.Height;

				//Set our background rectangle to fill the entire window
				rectBackgroundHide.Height = windowHeight;
				rectBackgroundHide.Width = windowWidth;
				rectBackgroundHide.Margin = new Thickness(0, 0, 0, 0);
				rectBackgroundHide.Visibility = Visibility.Visible;

				taskPopup.Child = popupTask;
				taskPopup.IsOpen = true;
			}
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
			HidePopupTask();
		}

		private void HidePopupTask()
		{
			rectBackgroundHide.Visibility = Visibility.Collapsed;
			taskPopup.IsOpen = false;
			appBar.Visibility = Visibility.Visible;
			MySplitView.Pane.Visibility = Visibility.Visible;
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

		private void CalculateTaskSizes(double width, double height)
		{
			var columnsCount = 0;

			columnsCount = (int)Math.Round(width / 300.0);

			rectBackgroundHide.Height = Window.Current.Bounds.Width;
			rectBackgroundHide.Width = Window.Current.Bounds.Height;

			RepopulateGridChildren(svPlanings, columnsCount);
			RepopulateGridChildren(svPurchases, columnsCount);
			RepopulateGridChildren(svBookings, columnsCount);

			if (taskPopup.IsOpen)
			{
				var popupTask = taskPopup.Child as PopupTask;
				popupTask.MaxWidth = Window.Current.Bounds.Width - 50;
				popupTask.MaxHeight = Window.Current.Bounds.Height;
			}
		}

		private void RepopulateGridChildren(ScrollViewer scrollViewer, int numberOfColumns)
		{
			var rootWidth = LayoutRoot.ActualWidth;

			var taskWidth = rootWidth - 6;

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

		private void purchasesMenu_Tapped(object sender, TappedRoutedEventArgs e)
		{
			ChangeTaskCategory(TaskCategories.Purchase);
		}

		private void planningsMenu_Tapped(object sender, TappedRoutedEventArgs e)
		{
			ChangeTaskCategory(TaskCategories.Planing);
		}

		private void bookingsMenu_Tapped(object sender, TappedRoutedEventArgs e)
		{
			ChangeTaskCategory(TaskCategories.Booking);
		}

		private void homeMenu_Tapped(object sender, TappedRoutedEventArgs e)
		{
			ChangeTaskCategory(TaskCategories.Home);
		}

		private void ChangeTaskCategory(TaskCategories category)
		{
			spHome.Visibility = IsVisible(category == TaskCategories.Home);
			planingsGrid.Visibility = IsVisible(category == TaskCategories.Planing);
			bookingsGrid.Visibility = IsVisible(category == TaskCategories.Booking);
			purchasesGrid.Visibility = IsVisible(category == TaskCategories.Purchase);
		}

		private Visibility IsVisible(bool value)
		{
			if (value)
			{
				return Visibility.Visible;
			}

			return Visibility.Collapsed;
		}
	}
}
