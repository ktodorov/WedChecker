using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
	public class TileCategory
	{
		public string Title { get; set; }
		public List<TaskTileControl> Tiles { get; set; }
	}

	public sealed partial class TaskDialog : UserControl
	{
		private List<TaskListItem> PlanningTaskItems;
		private List<TaskListItem> PurchasingTaskItems;
		private List<TaskListItem> BookingTaskItems;

		public event TappedEventHandler TappedTile;

		public TaskCategories TappedTaskCategory;
		public string TappedTaskName;

		List<TaskTileControl> tasksList;
		List<TileCategory> taskCategoryList;


		public TaskDialog()
		{
			this.InitializeComponent();

			LoadTasks();
			GroupTasks();

			TaskData.DisableAddedTasks(tasksGridView);
		}

		private void GroupTasks()
		{
			taskCategoryList = new List<TileCategory>();
			var taskGroups = tasksList.OrderBy(e => e.TaskCategory).GroupBy(e => e.TaskCategory);
			foreach (IGrouping<TaskCategories, TaskTileControl> item in taskGroups)
			{
				taskCategoryList.Add(new TileCategory() { Title = item.Key.ToString(), Tiles = item.ToList<TaskTileControl>() });
			}
			groupItemsViewSource.Source = taskCategoryList;
			ZoomedOutGridView.ItemsSource = taskCategoryList;
		}

		private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
		{
			if (ZoomedOutGridView != null && ZoomedOutGridView.Items != null && tasksGridView != null && tasksGridView.Items != null)
			{
				tasksGridView.ScrollIntoView(tasksGridView.Items[0], ScrollIntoViewAlignment.Default);
				if (e.IsSourceZoomedInView == false)
				{
					var index = 0;
					var taskType = ZoomedOutGridView.Items.IndexOf(e.SourceItem.Item);
					if (tasksGridView.Items != null)
					{
						var item = tasksGridView.Items[index] as TaskTileControl;
						while (item == null || ((int)item.TaskCategory) != (taskType + 1))
						{
							index++;
							item = tasksGridView.Items[index] as TaskTileControl;
						}

						e.DestinationItem.Item = item;
					}
				}
			}
		}

		public void LoadTasks()
		{
			tasksList = new List<TaskTileControl>();
			PlanningTaskItems = TaskData.LoadPlanningTaskItems();
			foreach (var planItem in PlanningTaskItems)
			{
				AddTask(planItem.Title, planItem.TaskName, TaskCategories.Planing);
			}

			PurchasingTaskItems = TaskData.LoadPurchasingTaskItems();
			var snd = new List<TaskTileControl>();
			foreach (var purchaseItem in PurchasingTaskItems)
			{
				AddTask(purchaseItem.Title, purchaseItem.TaskName, TaskCategories.Purchase);
			}

			BookingTaskItems = TaskData.LoadBookingTaskItems();
			foreach (var bookItem in BookingTaskItems)
			{
				AddTask(bookItem.Title, bookItem.TaskName, TaskCategories.Booking);
			}
		}

		private void AddTask(string title, string name, TaskCategories category)
		{
			var taskTile = new TaskTileControl();
			taskTile.TaskTitle = title;
			taskTile.TaskName = name;
			taskTile.TileTapped += TaskTile_Tapped;
			taskTile.TaskCategory = category;

			tasksList.Add(taskTile);
		}

		private void TaskTile_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var tile = sender as TaskTileControl;

			if (tile.Category == TaskCategories.Booking)
			{
				TappedTaskCategory = TaskCategories.Booking;
			}
			else if (tile.Category == TaskCategories.Planing)
			{
				TappedTaskCategory = TaskCategories.Planing;
			}
			else if (tile.Category == TaskCategories.Purchase)
			{
				TappedTaskCategory = TaskCategories.Purchase;
			}

			TappedTaskName = tile.TaskName;

			//tile.IsEnabled = false;

			TappedTile?.Invoke(this, e);
		}

		public void IsTileEnabled(string taskName, bool enabled)
		{
			var tile = tasksList.FirstOrDefault(t => t.TaskName == taskName);

			if (tile != null)
			{
				tile.IsEnabled = enabled;
			}
		}

        private void tasksGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemsWrapGrid appItemsPanel = (ItemsWrapGrid)tasksGridView.ItemsPanelRoot;

            double optimizedWidth = 150.0;
            double margin = 0.0;
            var number = (int)e.NewSize.Width / (int)optimizedWidth;
            appItemsPanel.ItemWidth = (e.NewSize.Width - margin) / (double)number;
        }
    }
}
