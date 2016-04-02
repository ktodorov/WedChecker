﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Pages;
using WedChecker.UserControls;
using WedChecker.UserControls.Tasks;
using WedChecker.UserControls.Tasks.Planings;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
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
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
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
		}

		private void MainTitleBar_BackButtonClick(object sender, RoutedEventArgs e)
		{
			if (addTaskDialog.Visibility == Visibility.Visible)
			{
				appBar.Visibility = Visibility.Visible;
				addTaskDialog.Visibility = Visibility.Collapsed;
				mainTitleBar.SetBackButtonVisible(false);
			}
			else if (Frame.CanGoBack)
			{
				Frame.GoBack();
			}
		}

		private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
		{
			if (addTaskDialog.Visibility == Visibility.Visible)
			{
				appBar.Visibility = Visibility.Visible;
				addTaskDialog.Visibility = Visibility.Collapsed;
				mainTitleBar.SetBackButtonVisible(false);
				e.Handled = true;
			}
			else if (Frame.CanGoBack)
			{
				Frame.GoBack();
				e.Handled = true;
			}
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
				mainSplitView.Visibility = Visibility.Collapsed;
				var firstLaunchPopup = new FirstLaunchPopup();
				firstLaunchPopup.VerticalAlignment = VerticalAlignment.Stretch;
				Grid.SetRowSpan(firstLaunchPopup, 2);
				mainGrid.Children.Add(firstLaunchPopup);
			}
			else
			{
				loadControlsProgressRing.IsActive = true;
				var task = AppData.PopulateAddedControls();
				await Task.WhenAll(task);
				var controls = task.Result;
				controls = controls.OrderBy(c => c.GetType().GetProperty("TaskName").GetValue(null, null).ToString()).ToList();
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
				TaskData.InsertTaskControl(this, type, false, taskTapped);
			}

			//var firstLaunchPopup = LayoutRoot.Children.OfType<FirstLaunchPopup>().FirstOrDefault();
			//if (firstLaunchPopup != null)
			//{
			//    LayoutRoot.Children.Remove(firstLaunchPopup);
			//}
			addTaskDialog.Visibility = Visibility.Collapsed;
			appBar.Visibility = Visibility.Visible;
			//mainPivot.Visibility = Visibility.Visible;
			//mainPivot.SelectedIndex = 0;

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

				var taskCategory = addTaskDialog.TappedTaskCategory;
				ChangeTaskCategory(taskCategory);

				CalculateTaskSizes(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
			}
		}

		private void AddTaskButton_Click(object sender, RoutedEventArgs e)
		{
			addTaskDialog.Visibility = Visibility.Visible;
			mainTitleBar.SetBackButtonVisible(true);
			appBar.Visibility = Visibility.Collapsed;
		}

		private void AboutPageButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(AboutPage));
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
			mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
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

				//Set our background rectangle to fill the entire window
				rectBackgroundHide.Height = windowHeight;
				rectBackgroundHide.Width = windowWidth;
				rectBackgroundHide.Margin = new Thickness(0, 0, 0, 0);
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
			HidePopupTask();
		}

		private void HidePopupTask()
		{
			rectBackgroundHide.Visibility = Visibility.Collapsed;
			taskPopup.IsOpen = false;
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

		private void CalculateTaskSizes(double width, double height)
		{
			var columnsCount = 0;

			columnsCount = (int)Math.Round(width / 300.0);

			rectBackgroundHide.Height = height;
			rectBackgroundHide.Width = width;

			RepopulateGridChildren(svPlanings, columnsCount);
			RepopulateGridChildren(svPurchases, columnsCount);
			RepopulateGridChildren(svBookings, columnsCount);
		}

		private void RepopulateGridChildren(ScrollViewer scrollViewer, int numberOfColumns)
		{
			var rootWidth = LayoutRoot.ActualWidth;

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
			svPlanings.Visibility = IsVisible(category == TaskCategories.Planing);
			svBookings.Visibility = IsVisible(category == TaskCategories.Booking);
			svPurchases.Visibility = IsVisible(category == TaskCategories.Purchase);

			mainTitleBar.SetSubTitle(category.ToString().ToUpper());
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

			var windowWidth = Window.Current.Bounds.Width;
			var windowHeight = Window.Current.Bounds.Height;

			double NewHorizontalOffset = ((windowWidth - popupTask.ActualWidth) / 2);
			double NewVerticalOffset = (windowHeight - popupTask.ActualHeight) / 2;

			if (ActualHorizontalOffset != NewHorizontalOffset || ActualVerticalOffset != NewVerticalOffset)
			{
				taskPopup.HorizontalOffset = NewHorizontalOffset;
				taskPopup.VerticalOffset = NewVerticalOffset;
			}

			popupTask.ResizeContent(windowWidth, windowHeight);
		}
	}
}
