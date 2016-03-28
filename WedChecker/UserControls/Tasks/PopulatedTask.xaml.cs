using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
	public sealed partial class PopulatedTask : UserControl
	{
		public Type ConnectedTaskControlType
		{
			get;
			private set;
		}

		private bool TaskOptionsOpened = false;

		public PopulatedTask()
		{
			this.InitializeComponent();
		}

		public PopulatedTask(Type controlType, bool isNew, bool setVisible = false)
		{
			this.InitializeComponent();

			ConnectedTaskControlType = controlType;
			var taskName = controlType.GetProperty("TaskName")?.GetValue(null, null).ToString();
			if (taskName != null)
			{
				buttonTaskName.Text = taskName.ToUpper();
				this.Name = taskName;
			}

			var displayHeader = controlType.GetProperty("DisplayHeader")?.GetValue(null, null).ToString();
			tbTaskHeader.Text = displayHeader;
		}
	}
}
