using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.UserControls.Elements;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
	public sealed partial class PopupTask : UserControl
	{
		public BaseTaskControl ConnectedTaskControl
		{
			get;
			private set;
		}

		public bool ConnectedControlVisible
		{
			get
			{
				return spConnectedControl.Visibility == Visibility.Visible;
			}
			set
			{
				if (value)
				{
					spConnectedControl.Visibility = Visibility.Visible;
				}
				else
				{
					spConnectedControl.Visibility = Visibility.Collapsed;
				}
			}
		}

		public bool InEditMode = false;
		private bool TaskOptionsOpened = false;

		public PopupTask()
		{
			this.InitializeComponent();
		}

		public event RoutedEventHandler SaveClick;
		public event RoutedEventHandler CancelClick;
		public event SizeChangedEventHandler TaskSizeChanged;

		public PopupTask(BaseTaskControl control, bool isNew)
		{
			this.InitializeComponent();

			try
			{
				ConnectedTaskControl = control;
				ConnectedTaskControl.Margin = new Thickness(10);

				var taskName = control.GetType().GetProperty("TaskName")?.GetValue(null, null).ToString();
				if (taskName != null)
				{
					buttonTaskName.Text = taskName;
					this.Name = taskName;
				}

				var header = control.GetType().GetProperty("DisplayHeader")?.GetValue(null, null).ToString();
				tbTaskHeader.Text = header;


				this.Loaded += PopupTask_Loaded;

				InEditMode = false;

			}
			catch (Exception ex)
			{
				var a = ex.Message;
			}
		}

		private async void PopupTask_Loaded(object sender, RoutedEventArgs e)
		{
			ProgressRingActive(true);
			await ConnectedTaskControl.DeserializeValues();

			FireSizeChangedEvent();


			spConnectedControl.Children.Add(ConnectedTaskControl);

			ProgressRingActive(false);
		}

		async void editTask_Click(object sender, RoutedEventArgs e)
		{
			taskOptionsFlyout.Hide();
			ProgressRingActive(true);

			await Task.Delay(TimeSpan.FromMilliseconds(1));
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => EditConnectedTask());

			InEditMode = true;

			ProgressRingActive(false);
		}

		private void ProgressRingActive(bool active)
		{
			if (active)
			{
					progressBackground.Height = contentScroll.ActualHeight;
				spConnectedControl.Visibility = Visibility.Collapsed;
			}
			else
			{
				spConnectedControl.Visibility = Visibility.Visible;
			}

			progressBackground.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
			connectedControlProgress.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
			connectedControlProgress.IsActive = active;
		}

		private void EditConnectedTask()
		{

			InEditMode = true;
			tbTaskHeader.Text = ConnectedTaskControl.EditHeader ?? string.Empty;
			editTask.Visibility = Visibility.Collapsed;
			if (ConnectedTaskControl != null)
			{
				if (ConnectedTaskControl.Visibility == Visibility.Collapsed)
				{
					ConnectedTaskControl.Visibility = Visibility.Visible;
				}

				ConnectedTaskControl.EditValues();
			}

			FireSizeChangedEvent();

		}

		private async void saveTask_Click(object sender, RoutedEventArgs e)
		{
			await DisplayConnectedTask();

			if (SaveClick != null)
			{
				SaveClick(this, e);
			}
			InEditMode = false;
		}

		private async Task DisplayConnectedTask(bool shouldSave = true)
		{
			InEditMode = false;
			editTask.Visibility = Visibility.Visible;
			if (ConnectedTaskControl != null)
			{
				if (shouldSave)
				{
					await ConnectedTaskControl.SubmitValues();
				}
				ConnectedTaskControl.DisplayValues();
			}

			FireSizeChangedEvent();
		}

		private void tryAgainButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private async void deleteTask_Click(object sender, RoutedEventArgs e)
		{
			var msgDialog = new MessageDialog("Are you sure you want to delete this task?", "Please confirm");

			msgDialog.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(CommandHandler)));
			msgDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(CommandHandler)));

			await msgDialog.ShowAsync();
		}

		private async void CommandHandler(IUICommand command)
		{
			var commandLabel = command.Label;
			switch (commandLabel)
			{
				case "Delete":
					await DeleteTask();
					CancelClick?.Invoke(this, new RoutedEventArgs());
					break;
				case "Cancel":
					break;
			}
		}

		private async Task DeleteTask()
		{
			if (ConnectedTaskControl != null)
			{
				await ConnectedTaskControl.DeleteValues();

				EnableTaskTile();

				DeletePopulatedTask();
			}
		}

		private void EnableTaskTile()
		{
			var mainGrid = this.FindAncestorByName("mainGrid") as Grid;
			if (mainGrid == null)
			{
				return;
			}

			var taskDialog = mainGrid.Children.OfType<TaskDialog>().FirstOrDefault();
			if (taskDialog != null)
			{
				taskDialog.IsTileEnabled(ConnectedTaskControl.TaskCode, true);
			}
		}

		private void DeletePopulatedTask()
		{
			var mainGrid = this.FindAncestorByName("mainGrid") as Grid;
			if (mainGrid == null)
			{
				return;
			}

			var mainSplitView = mainGrid.Children.OfType<SplitView>().FirstOrDefault(sv => sv.Name == "mainSplitView");
			if (mainSplitView == null)
			{
				return;
			}

			var layoutRoot = mainSplitView.Content as Grid;
			if (layoutRoot == null)
			{
				return;
			}

			if (!DeleteFromElement(layoutRoot, "svPlanings"))
			{
				if (!DeleteFromElement(layoutRoot, "svPurchases"))
				{
					DeleteFromElement(layoutRoot, "svBookings");
				}
			}
		}

		private bool DeleteFromElement(FrameworkElement parent, string name)
		{
			var scrollViewer = parent.FindName(name) as ScrollViewer;
			if (scrollViewer == null)
			{
				return false;
			}

			var gridView = scrollViewer.Content as GridView;
			if (gridView != null)
			{
				var populatedTask = gridView.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.Name == this.Name);
				if (populatedTask != null)
				{
					gridView.Items.Remove(populatedTask);
					return true;
				}
			}

			return false;
		}

		private void cancelTask_Click(object sender, RoutedEventArgs e)
		{
			if (CancelClick != null)
			{
				CancelClick(this, e);
			}
		}

		public void ResizeContent(double windowWidth, double windowHeight, WedCheckerTitleBar titleBar)
		{
			var popupHeaderHeight = buttonTaskName.ActualHeight;
			var popupFooterHeight = commandGrid.ActualHeight; 
			var margins = 10;

			var removedHeight = margins + popupHeaderHeight + popupFooterHeight;

			contentScroll.MaxHeight = windowHeight - (removedHeight);
			contentScroll.MaxWidth = windowWidth - 50;
		}

		private void FireSizeChangedEvent()
		{
			if (TaskSizeChanged != null)
			{
				var t = new RoutedEventArgs();
				TaskSizeChanged(this, t as SizeChangedEventArgs);
			}
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			FireSizeChangedEvent();
		}
	}
}
