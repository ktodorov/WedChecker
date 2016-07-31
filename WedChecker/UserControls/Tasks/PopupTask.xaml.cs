using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.UserControls.Elements;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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

        public PopupTask()
        {
            this.InitializeComponent();
        }

        public event RoutedEventHandler SaveClick;
        public event RoutedEventHandler CancelClick;
        public event SizeChangedEventHandler TaskSizeChanged;

        public EventHandler OnEdit;
        public EventHandler OnDelete;
        public EventHandler OnShare;
        public EventHandler OnExport;

        public PopupTask(BaseTaskControl control, bool isNew)
        {
            this.InitializeComponent();

            try
            {
                ConnectedTaskControl = control;
                ConnectedTaskControl.Margin = new Thickness(10);

                var taskName = control.TaskName.ToString();
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

        void editTask_Click(object sender, RoutedEventArgs e)
        {
            taskOptionsFlyout.Hide();
            OnEdit?.Invoke(this, new EventArgs());
        }

        public async Task Edit()
        {
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
            await Save();
        }

        public async Task Save()
        {
            await DisplayConnectedTask();

            SaveClick?.Invoke(this, new RoutedEventArgs());

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

        private void deleteTask_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(this, new EventArgs());
        }

        public void Cancel()
        {
            CancelClick?.Invoke(this, new RoutedEventArgs());
        }


        private void cancelTask_Click(object sender, RoutedEventArgs e)
        {
            CancelClick?.Invoke(this, e);
        }

        public void ResizeContent(double windowWidth, double windowHeight, WedCheckerTitleBar titleBar)
        {
            var popupHeaderHeight = buttonTaskName.ActualHeight;
            var popupFooterHeight = commandGrid.ActualHeight;
            var margins = 10;

            var removedHeight = margins + popupHeaderHeight + popupFooterHeight;

            var maxHeight = (windowHeight - removedHeight);
            var maxWidth = windowWidth - 50;
            SetContentMaxHeightWidth(maxHeight, maxWidth);
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

        private void shareTask_Click(object sender, RoutedEventArgs e)
        {
            OnShare?.Invoke(this, new EventArgs());
        }

        private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                Cancel();
            }
        }

        private void exportTask_Click(object sender, RoutedEventArgs e)
        {
            OnExport?.Invoke(this, new EventArgs());
        }

        private void SetContentMaxHeightWidth(double maxHeight, double maxWidth)
        {
            contentScroll.MaxWidth = maxWidth;

            if (ConnectedTaskControl.HasOwnScrollViewer)
            {
                maxHeight -= tbTaskHeader.ActualHeight + 20; // 20 for the margins
                ConnectedTaskControl.SetOwnScrollViewerHeight(maxHeight, maxWidth);
            }
            else
            {
                contentScroll.MaxHeight = maxHeight;
            }
        }
    }
}
