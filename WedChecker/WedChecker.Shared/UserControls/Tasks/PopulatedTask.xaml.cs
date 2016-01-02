using System;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class PopulatedTask : UserControl
    {
        private BaseTaskControl ConnectedTaskControl
        {
            get;
            set;
        }

        private bool InEditMode = false;
        private bool TaskOptionsOpened = false;

        public PopulatedTask()
        {
            this.InitializeComponent();
            SetBackgroundColor();
        }


        public PopulatedTask(BaseTaskControl control, bool isNew)
        {
            this.InitializeComponent();

            try
            {
                ConnectedTaskControl = control;
                ConnectedTaskControl.Margin = new Thickness(10);
                spConnectedControl.Children.Add(ConnectedTaskControl);
                buttonTaskName.Content = control.TaskName.ToUpper();
                SetBackgroundColor();

                if (!isNew)
                {
                    ConnectedTaskControl.DisplayValues();
                    Task.WaitAll(DisplayConnectedTask(false));
                }
                else
                {
                    EditConnectedTask();
                }

            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }

        private void SetBackgroundColor()
        {
            var phoneAccentColor = Core.GetPhoneAccentBrush();

            phoneAccentColor.A = 10;
            var colorBrush = new SolidColorBrush(phoneAccentColor);
            mainPanel.Background = colorBrush;

            //phoneAccentColor.A = 45;
            //colorBrush = new SolidColorBrush(phoneAccentColor);
            //taskOptionsPanel.Background = colorBrush;
            //showTaskOptionsPanel.Background = colorBrush;
        }

        private void buttonTaskName_Click(object sender, RoutedEventArgs e)
        {
            ShowCollapseTask();
        }

        private void ChangeMainContentVisibility(Visibility visibility)
        {
            childPanel.Visibility = visibility;

            ConnectedTaskControl.Visibility = visibility;
            tbTaskHeader.Visibility = visibility;

            if (TaskOptionsOpened)
            {
                showTaskOptions.Content = "\uE09A";
                taskOptionsPanel.Visibility = Visibility.Collapsed;
                TaskOptionsOpened = false;
            }

            if (visibility == Visibility.Collapsed || InEditMode)
            {
                displayTask.Visibility = visibility;
            }
        }


        void editTask_Click(object sender, RoutedEventArgs e)
        {
            EditConnectedTask();
        }

        private void EditConnectedTask()
        {
            InEditMode = true;
            tbTaskHeader.Text = ConnectedTaskControl.EditHeader ?? string.Empty;
            displayTask.Visibility = Visibility.Visible;
            editTask.Visibility = Visibility.Collapsed;
            if (ConnectedTaskControl != null)
            {
                if (ConnectedTaskControl.Visibility == Visibility.Collapsed)
                {
                    ConnectedTaskControl.Visibility = Visibility.Visible;
                }
                ConnectedTaskControl.EditValues();
            }
        }

        private async void displayTask_Click(object sender, RoutedEventArgs e)
        {
            await DisplayConnectedTask();
        }

        private async Task DisplayConnectedTask(bool shouldSave = true)
        {
            InEditMode = false;
            tbTaskHeader.Text = ConnectedTaskControl.DisplayHeader ?? string.Empty;
            displayTask.Visibility = Visibility.Collapsed;
            editTask.Visibility = Visibility.Visible;
            if (ConnectedTaskControl != null)
            {
                if (shouldSave)
                {
                    await ConnectedTaskControl.SubmitValues();
                }
                ConnectedTaskControl.DisplayValues();
            }
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
                    break;
                case "Cancel":
                    break;
            }
        }

        private async void DeleteTask()
        {
            if (ConnectedTaskControl != null)
            {
                await AppData.DeleteSerializableTask(ConnectedTaskControl);
                await AppData.RemoveGlobalValue(ConnectedTaskControl.TaskCode);

                EnableTaskTile();

                var parent = this.Parent as StackPanel;
                parent.Children.Remove(this);
            }
        }

        private void EnableTaskTile()
        {
            var parent1 = this.Parent as StackPanel;
            if (parent1 == null)
            {
                return;
            }

            var parent2 = parent1.Parent as Grid;
            if (parent2 == null)
            {
                return;
            }

            var parent3 = parent2.Parent as ScrollViewer;
            if (parent3 == null)
            {
                return;
            }

            var parent4 = parent3.Parent as PivotItem;
            if (parent4 == null)
            {
                return;
            }

            var parent5 = parent4.Parent as Pivot;
            if (parent5 == null)
            {
                return;
            }

            var parent6 = parent5.Parent as Grid;
            if (parent6 == null)
            {
                return;
            }

            var svMain = parent6.Children.OfType<ScrollViewer>().FirstOrDefault(sv => sv.Name == "svMain");
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

        private void showTaskOptions_Click(object sender, RoutedEventArgs e)
        {
            if (TaskOptionsOpened)
            {
                showTaskOptions.Content = "\uE09A";
                taskOptionsPanel.Visibility = Visibility.Collapsed;
                TaskOptionsOpened = false;
            }
            else
            {

                showTaskOptions.Content = "\uE09D";
                taskOptionsPanel.Visibility = Visibility.Visible;
                TaskOptionsOpened = true;
            }
        }

        private void collapseTask_Click(object sender, RoutedEventArgs e)
        {
            ShowCollapseTask();
        }

        private void ShowCollapseTask()
        {
            if (childPanel.Visibility == Visibility.Collapsed)
            {
                ChangeMainContentVisibility(Visibility.Visible);
                tbShowCollapse.Text = "Collapse";
            }
            else if (childPanel.Visibility == Visibility.Visible)
            {
                ChangeMainContentVisibility(Visibility.Collapsed);
                tbShowCollapse.Text = "Expand";
            }
        }
    }
}
