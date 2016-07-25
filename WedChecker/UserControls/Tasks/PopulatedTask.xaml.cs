using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class PopulatedTask : UserControl
    {
        public BaseTaskControl ConnectedTaskControl
        {
            get;
            private set;
        }

        public EventHandler OnEdit;
        public EventHandler OnDelete;
        public EventHandler OnShare;
        public EventHandler OnExport;

        public string DisplayHeader
        {
            get
            {
                return tbTaskHeader.Text;
            }
        }

        public PopulatedTask()
        {
            this.InitializeComponent();
        }

        public PopulatedTask(BaseTaskControl control, bool isNew, bool setVisible = false)
        {
            this.InitializeComponent();

            ConnectedTaskControl = control;
            var controlType = control.GetType();
            var taskName = control.TaskName.ToString();
            if (taskName != null)
            {
                buttonTaskName.Text = taskName.ToUpper();
                this.Name = taskName;
            }

            var displayHeader = controlType.GetProperty("DisplayHeader")?.GetValue(null, null).ToString();
            tbTaskHeader.Text = displayHeader;

            if (isNew)
            {
                newTilePanel.Visibility = Visibility.Visible;
            }

            taskSummary.LoadTaskData(ConnectedTaskControl);
        }

        private void editTask_Click(object sender, RoutedEventArgs e)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(mainPanel);
            flyout.Hide();
            OnEdit?.Invoke(this, new EventArgs());
        }

        private void deleteTask_Click(object sender, RoutedEventArgs e)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(mainPanel);
            flyout.Hide();
            OnDelete?.Invoke(this, new EventArgs());
        }

        private void mainPanel_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        public void RefreshTaskSummary(BaseTaskControl taskControl = null)
        {
            if (taskControl == null)
            {
                taskControl = ConnectedTaskControl;
            }

            taskSummary.RefreshTaskData(taskControl);
        }

        private void shareTask_Click(object sender, RoutedEventArgs e)
        {
            OnShare?.Invoke(this, new EventArgs());
        }

        private void exportTask_Click(object sender, RoutedEventArgs e)
        {
            OnExport?.Invoke(this, new EventArgs());
        }
    }
}
