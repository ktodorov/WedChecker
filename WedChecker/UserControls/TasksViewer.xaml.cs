using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Helpers;
using WedChecker.UserControls.Tasks;
using Windows.ApplicationModel.DataTransfer;
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

namespace WedChecker.UserControls
{
    public sealed partial class TasksViewer : UserControl
    {
        public TaskCategories TasksCategory;

        private int columns;
        private double taskHeight;

        public MainPage ParentPage
        {
            get; set;
        }

        public List<PopulatedTask> Tasks
        {
            get
            {
                return gvTasks.Items.OfType<PopulatedTask>().ToList();
            }
        }

        public TasksViewer()
        {
            this.InitializeComponent();
        }
        public TasksViewer(TaskCategories category)
        {
            this.InitializeComponent();

            TasksCategory = category;
        }

        public async void PopulateTasks()
        {
            if (!Tasks.Any())
            {
                var controls = await AppData.PopulateAddedControls(TasksCategory);
                AddTasks(controls);
            }
        }

        public void AddTask(PopulatedTask task)
        {
            gvTasks.Items.Add(task);
        }

        public void AddTasks(List<BaseTaskControl> tasks)
        {
            var orderedTasks = OrderTasks(tasks);
            foreach (var task in orderedTasks)
            {
                InsertTaskControl(task, false);
            }
        }

        public void UpdateTasks(List<BaseTaskControl> controls)
        {
            var tasks = gvTasks.Items.OfType<PopulatedTask>().Where(p => !controls.Any(c => c.GetType() == p.ConnectedTaskControl.GetType())).ToList();
            foreach (var task in tasks)
            {
                gvTasks.Items.Remove(task);
            }

            var remainingTasks = gvTasks.Items.OfType<PopulatedTask>().Except(tasks);
            foreach (var remainingTask in remainingTasks)
            {
                remainingTask.RefreshTaskSummary();
            }
        }

        public bool ContainsTask(Type taskType)
        {
            var tasks = gvTasks.Items.OfType<PopulatedTask>().Where(p => p.ConnectedTaskControl.GetType() == taskType).ToList();
            if (tasks.Any())
            {
                return true;
            }

            return false;
        }

        public void ClearTasks()
        {
            gvTasks.Items.Clear();
        }

        public PopulatedTask GetPopulatedTaskByType(Type taskType)
        {
            var task = gvTasks.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType() == taskType);
            if (task != null)
            {
                return task;
            }

            return null;
        }
        public PopulatedTask GetPopulatedTaskByTaskName(string taskName)
        {
            var task = gvTasks.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.GetType().Name == taskName);
            if (task != null)
            {
                return task;
            }

            return null;
        }

        private void gvTasks_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = Window.Current.Bounds.Width;

            columns = (int)Math.Round(width / 400.0);
            if (width < 720)
            {
                columns = 1;
            }

            var gridView = sender as GridView;
            if (gridView == null)
            {
                return;
            }

            var panel = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            var itemWidth = e.NewSize.Width / columns;
            panel.ItemWidth = itemWidth;
            taskHeight = panel.ItemWidth / 2;
            if (taskHeight < 200)
            {
                taskHeight = 200;
            }
            panel.ItemHeight = taskHeight;

            ParentPage.ResizePopup();
        }

        private List<BaseTaskControl> OrderTasks(List<BaseTaskControl> tasks)
        {
            var result = new List<BaseTaskControl>();
            var storedSortType = AppData.GetRoamingSetting<int>("TaskSortType");
            var storedSortOrder = AppData.GetRoamingSetting<int>("TaskSortOrder");
            var sortingType = (TaskSortingType)storedSortType;
            var sortingOrder = (TaskSortingOrder)storedSortOrder;

            if (sortingType == TaskSortingType.Name && sortingOrder == TaskSortingOrder.Ascending)
            {
                result = tasks.OrderBy(c => c.TaskName.ToString()).ToList();
            }
            else if (sortingType == TaskSortingType.Name && sortingOrder == TaskSortingOrder.Descending)
            {
                result = tasks.OrderByDescending(c => c.TaskName.ToString()).ToList();
            }
            else if (sortingType == TaskSortingType.DateCreated && sortingOrder == TaskSortingOrder.Ascending)
            {
                result = tasks.OrderBy(c => c.CreatedOn).ToList();
            }
            else if (sortingType == TaskSortingType.DateCreated && sortingOrder == TaskSortingOrder.Descending)
            {
                result = tasks.OrderByDescending(c => c.CreatedOn).ToList();
            }
            else if (sortingType == TaskSortingType.DateModified && sortingOrder == TaskSortingOrder.Ascending)
            {
                result = tasks.OrderBy(c => c.ModifiedOn).ToList();
            }
            else if (sortingType == TaskSortingType.DateModified && sortingOrder == TaskSortingOrder.Descending)
            {
                result = tasks.OrderByDescending(c => c.ModifiedOn).ToList();
            }

            return result;
        }

        public bool CreateTaskControl(BaseTaskControl taskControl)
        {
            if (taskControl == null)
            {
                return false;
            }

            InsertTaskControl(taskControl, true);

            return true;
        }

        public void InsertTaskControl(BaseTaskControl taskControl, bool isNew = true)
        {
            var newPopulatedTask = new PopulatedTask(taskControl, isNew);

            newPopulatedTask.Tapped += taskTapped;
            newPopulatedTask.OnEdit += onTaskEdit;
            newPopulatedTask.OnDelete += onTaskDelete;
            newPopulatedTask.OnShare += onTaskShare;
            newPopulatedTask.OnExport += onTaskExport;

            var taskType = taskControl.GetType();
            if (!ContainsTask(taskType))
            {
                AddTask(newPopulatedTask);
            }
        }

        #region Task Events

        public void taskTapped(object sender, TappedRoutedEventArgs e)
        {
            var populatedTask = sender as PopulatedTask;

            if (populatedTask != null)
            {
                var popupTask = CreatePopupTaskFromPopulatedTask(populatedTask);

                ParentPage.TaskPopup.Child = popupTask;
                ParentPage.TaskPopup.IsOpen = true;
            }
        }

        private async void onTaskEdit(object sender, EventArgs e)
        {
            var populatedTask = sender as PopulatedTask;
            PopupTask popupTask = null;
            if (sender is PopulatedTask)
            {
                popupTask = CreatePopupTaskFromPopulatedTask(populatedTask);
            }
            else if (sender is PopupTask)
            {
                popupTask = sender as PopupTask;
            }

            if (popupTask == null)
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100));
            await popupTask.Edit();
        }

        private async void onTaskDelete(object sender, EventArgs e)
        {
            BaseTaskControl connectedControl;
            if (sender is PopulatedTask)
            {
                var populatedTask = sender as PopulatedTask;
                connectedControl = populatedTask.ConnectedTaskControl;
                if (connectedControl == null)
                {
                    return;
                }

                populatedTask.RefreshTaskSummary(connectedControl);
            }
            else if (sender is PopupTask)
            {
                var popupTask = sender as PopupTask;
                if (popupTask == null || popupTask.ConnectedTaskControl == null)
                {
                    return;
                }

                connectedControl = popupTask.ConnectedTaskControl;

                var populatedTask = GetPopulatedTaskByType(connectedControl.GetType());
                if (populatedTask != null)
                {
                    populatedTask.RefreshTaskSummary(connectedControl);
                }
            }
            else
            {
                return;
            }

            var frameworkSender = sender as FrameworkElement;

            var deleted = await TasksOperationsHelper.DeleteTaskAsync(this, connectedControl);
            if (sender is PopupTask && deleted)
            {
                var popupTask = sender as PopupTask;
                popupTask.Cancel();
            }
        }

        private void onTaskShare(object sender, EventArgs e)
        {
            var connectedControl = GetTaskControlFromSenderControl(sender);
            if (connectedControl == null)
            {
                return;
            }

            AppData.TextForShare = connectedControl.GetDataAsText();

            DataTransferManager.ShowShareUI();
        }

        private void onTaskExport(object sender, EventArgs e)
        {
            var connectedControl = GetTaskControlFromSenderControl(sender);
            if (connectedControl == null)
            {
                return;
            }

            connectedControl.ExportDataAsTextFile();
        }

        #endregion

        private BaseTaskControl GetTaskControlFromSenderControl(object sender)
        {
            BaseTaskControl connectedControl = null;

            if (sender is PopulatedTask)
            {
                var populatedTask = sender as PopulatedTask;
                connectedControl = populatedTask?.ConnectedTaskControl;
            }
            else if (sender is PopupTask)
            {
                var popupTask = sender as PopupTask;
                connectedControl = popupTask?.ConnectedTaskControl;
            }

            return connectedControl;
        }

        private PopupTask CreatePopupTaskFromPopulatedTask(PopulatedTask populatedTask)
        {
            var baseTaskType = populatedTask.ConnectedTaskControl.GetType();
            var baseTaskControl = Activator.CreateInstance(baseTaskType) as BaseTaskControl;

            var popupTask = new PopupTask(baseTaskControl, false);
            popupTask.SaveClick += PopupTask_SaveClick;
            popupTask.CancelClick += PopupTask_CancelClick;
            popupTask.OnDelete += onTaskDelete;
            popupTask.OnEdit += onTaskEdit;
            popupTask.OnShare += onTaskShare;
            popupTask.OnExport += onTaskExport;
            popupTask.TaskSizeChanged += PopupTask_TaskSizeChanged;

            ParentPage.AppBar.Visibility = Visibility.Collapsed;
            ParentPage.MainSplitView.Pane.Visibility = Visibility.Collapsed;

            var windowWidth = Window.Current.Bounds.Width;
            var windowHeight = Window.Current.Bounds.Height;

            ParentPage.RectBackgroundHide.Visibility = Visibility.Visible;

            ParentPage.CalculateTaskSizes(windowWidth, windowHeight);

            ParentPage.TaskPopup.Child = popupTask;
            ParentPage.TaskPopup.IsOpen = true;

            return popupTask;
        }

        public async Task<bool> CreateTask(string taskName)
        {
            var taskType = TaskData.GetTaskType(taskName);
            var taskControl = await TaskData.CreateTaskControlByType(taskType);
            if (taskControl == null)
            {
                return false;
            }

            var created = CreateTaskControl(taskControl);
            if (!created)
            {
                return false;
            }

            var newPopulatedTask = GetPopulatedTaskByTaskName(taskName);

            taskTapped(newPopulatedTask, new TappedRoutedEventArgs());

            return true;
        }

        private void PopupTask_TaskSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ParentPage.ResizePopup();
        }

        private void PopupTask_CancelClick(object sender, RoutedEventArgs e)
        {
            ParentPage.HidePopupTask();
        }

        private void PopupTask_SaveClick(object sender, RoutedEventArgs e)
        {
            ParentPage.HidePopupTask();

            var popupTask = sender as PopupTask;
            var updatedTask = popupTask.ConnectedTaskControl;

            var populatedTask = GetPopulatedTaskByType(updatedTask.GetType());
            if (populatedTask != null)
            {
                populatedTask.RefreshTaskSummary(updatedTask);
            }

            TaskChanged(updatedTask);
        }

        private void TaskChanged(BaseTaskControl task)
        {
            var currentTasks = gvTasks.Items.OfType<PopulatedTask>().Select(pt => pt.ConnectedTaskControl).ToList();

            var changedTask = currentTasks.FirstOrDefault(c => c.TaskCode == task.TaskCode);
            var oldIndex = currentTasks.IndexOf(changedTask);
            currentTasks.Remove(changedTask);
            currentTasks.Add(task);
            var orderedTasks = OrderTasks(currentTasks);

            var newIndex = orderedTasks.IndexOf(task);
            if (newIndex == oldIndex)
            {
                return;
            }

            var populatedTask = gvTasks.Items.OfType<PopulatedTask>().FirstOrDefault(p => p.ConnectedTaskControl.TaskCode == task.TaskCode);
            gvTasks.Items.Remove(populatedTask);
            gvTasks.Items.Insert(newIndex, populatedTask);

            var verticalOffset = 0.0;
            if (newIndex > 0)
            {
                verticalOffset = (newIndex / columns) * taskHeight;
            }

            svTasks.ChangeView(null, verticalOffset, null);
        }

        public void RemoveTask(BaseTaskControl task)
        {
            var taskToRemove = Tasks.FirstOrDefault(pt => pt.ConnectedTaskControl.TaskCode == task.TaskCode);
            gvTasks.Items.Remove(taskToRemove);
        }
    }
}
