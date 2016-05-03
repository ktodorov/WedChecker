using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WedChecker.Common;
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
					buttonTaskName.Text = taskName; // TaskName.ToUpper();
					this.Name = taskName;
				}

				var header = control.GetType().GetProperty("DisplayHeader")?.GetValue(null, null).ToString();
				tbTaskHeader.Text = header;

				spConnectedControl.Children.Add(ConnectedTaskControl);

				this.Loaded += PopupTask_Loaded;

				MaxWidth = Window.Current.Bounds.Width - 50;
				MaxHeight = Window.Current.Bounds.Height - 100;

				InEditMode = false;
			}
			catch (Exception ex)
			{
				var a = ex.Message;
			}
		}

		private async void PopupTask_Loaded(object sender, RoutedEventArgs e)
		{
			connectedControlProgress.IsActive = true;
			await ConnectedTaskControl.DeserializeValues();

			FireSizeChangedEvent();

			connectedControlProgress.IsActive = false;
			connectedControlProgress.Visibility = Visibility.Collapsed;
		}

		void editTask_Click(object sender, RoutedEventArgs e)
		{
			taskOptionsFlyout.Hide();
			EditConnectedTask();
			InEditMode = true;
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

		private void CommandHandler(IUICommand command)
		{
			var commandLabel = command.Label;
			switch (commandLabel)
			{
				case "Delete":
					DeleteTask();
					if (CancelClick != null)
					{
						CancelClick(this, new RoutedEventArgs());
					}
					break;
				case "Cancel":
					break;
			}
		}

		private async void DeleteTask()
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
			var layoutRoot = this.FindAncestorByName("LayoutRoot") as Grid;

			var svMain = layoutRoot.Children.OfType<ScrollViewer>().FirstOrDefault(sv => sv.Name == "svMain");
			if (svMain == null)
			{
				return;
			}

			var panel = svMain.Content as StackPanel;
			if (panel == null)
			{
				return;
			}

			var gridViews = panel.Children.OfType<GridView>().ToList();
			foreach (var gridView in gridViews)
			{
				var tiles = gridView.Items.OfType<TaskTileControl>();
				var tile = tiles.FirstOrDefault(t => t.Name == ConnectedTaskControl.TaskCode);

				if (tile != null)
				{
					tile.IsEnabled = true;
					return;
				}
			}
		}

		private void DeletePopulatedTask()
		{
			var layoutRoot = this.FindAncestorByName("LayoutRoot") as Grid;

			if (!DeleteFromElement(layoutRoot, "gvPlanings"))
			{
				if (!DeleteFromElement(layoutRoot, "gvPurchases"))
				{
					DeleteFromElement(layoutRoot, "gvBookings");
				}
			}
		}

		private bool DeleteFromElement(FrameworkElement parent, string name)
		{
			var element = parent.FindName(name) as GridView;
			if (element != null)
			{
				var populatedTask = element.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.Name == this.Name);
				if (populatedTask != null)
				{
					element.Items.Remove(populatedTask);
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

		public void ResizeContent(double windowWidth, double windowHeight)
		{
			contentScroll.MaxHeight = windowHeight - 230;
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
