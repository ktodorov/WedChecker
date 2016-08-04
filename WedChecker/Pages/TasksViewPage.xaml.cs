using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Helpers;
using WedChecker.Infrastructure;
using WedChecker.Interfaces;
using WedChecker.UserControls;
using WedChecker.UserControls.Elements;
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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WedChecker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TasksViewPage : Page, IUpdateableTasks
    {
        private MainPage parentPage;

        public TasksViewer AttachedTasksViewer
        {
            get
            {
                return tasksViewer;
            }
        }

        public TaskCategories TasksCategory
        {
            get
            {
                return tasksViewer.TasksCategory;
            }
        }

        private Popup taskPopup
        {
            get
            {
                return parentPage.TaskPopup;
            }
        }

        private Rectangle rectBackgroundHide
        {
            get
            {
                return parentPage.RectBackgroundHide;
            }
        }

        public TasksViewPage()
        {
            this.InitializeComponent();
            this.Loaded += TasksViewPage_Loaded;
        }

        private void TasksViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            var storedSortType = AppData.GetRoamingSetting<int>("TaskSortType");
            var sortType = (TaskSortingType)storedSortType;
            LoadTasks(sortType);
        }

        private async void LoadTasks(TaskSortingType sortingType = TaskSortingType.ByName)
        {
            var controls = await AppData.PopulateAddedControls();

            AddPopulatedControls(controls, sortingType);

            var currentTitleBar = Core.CurrentTitleBar;
            if (currentTitleBar != null)
            {
                currentTitleBar.ProgressActive = false;
            }
        }

        public TasksViewPage(TaskCategories category, MainPage parent)
        {
            this.InitializeComponent();
            this.Loaded += TasksViewPage_Loaded;

            tasksViewer.TasksCategory = category;
            parentPage = parent;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Params result = (Params)e.Parameter;
            tasksViewer.TasksCategory = result.Category;
            parentPage = result.CurrentPage;

            base.OnNavigatedTo(e);
        }

        public void UpdateTasks(List<BaseTaskControl> controls)
        {
            tasksViewer.UpdateTasks(controls);
        }

        private bool TaskAlreadyAdded(Type taskType)
        {
            if (tasksViewer.ContainsTask(taskType))
            {
                return true;
            }

            return false;
        }

        public void AddPopulatedControls(List<BaseTaskControl> populatedControls, TaskSortingType sortingType = TaskSortingType.ByName)
        {
            var currentTypeControls = populatedControls.Where(t => Core.GetTaskCategory(t.GetType()) == TasksCategory).ToList();
            if (sortingType == TaskSortingType.ByName)
            {
                currentTypeControls = currentTypeControls.OrderBy(c => c.TaskName.ToString()).ToList();
            }
            else if (sortingType == TaskSortingType.ByNameReversed)
            {
                currentTypeControls = currentTypeControls.OrderByDescending(c => c.TaskName.ToString()).ToList();
            }
            else if (sortingType == TaskSortingType.OldestFirst)
            {
                currentTypeControls = currentTypeControls.OrderBy(c => c.CreatedOn).ToList();
            }
            else if (sortingType == TaskSortingType.NewestFirst)
            {
                currentTypeControls = currentTypeControls.OrderByDescending(c => c.CreatedOn).ToList();
            }

            foreach (var populatedControl in currentTypeControls)
            {
                var type = populatedControl.GetType();
                if (!TaskAlreadyAdded(type))
                {
                    InsertTaskControl(populatedControl, false, taskTapped, onTaskEdit, onTaskDelete, onTaskShare, onTaskExport);
                }
            }
        }

        public bool CreateTaskControl(BaseTaskControl taskControl, TappedEventHandler tappedEvent = null,
                                             EventHandler onTaskEdit = null, EventHandler onTaskDelete = null, EventHandler onTaskShare = null,
                                             EventHandler onTaskExport = null)
        {
            if (taskControl == null)
            {
                return false;
            }

            InsertTaskControl(taskControl, true, tappedEvent, onTaskEdit, onTaskDelete);

            return true;
        }

        public void InsertTaskControl(BaseTaskControl taskControl, bool isNew = true, TappedEventHandler tappedEvent = null,
                                             EventHandler onTaskEdit = null, EventHandler onTaskDelete = null, EventHandler onTaskShare = null,
                                             EventHandler onTaskExport = null)
        {
            var newPopulatedTask = new PopulatedTask(taskControl, isNew);
            if (tappedEvent != null)
            {
                newPopulatedTask.Tapped += tappedEvent;
            }
            if (onTaskEdit != null)
            {
                newPopulatedTask.OnEdit += onTaskEdit;
            }
            if (onTaskDelete != null)
            {
                newPopulatedTask.OnDelete += onTaskDelete;
            }
            if (onTaskShare != null)
            {
                newPopulatedTask.OnShare += onTaskShare;
            }
            if (onTaskExport != null)
            {
                newPopulatedTask.OnExport += onTaskExport;
            }

            var taskType = taskControl.GetType();
            if (!tasksViewer.ContainsTask(taskType))
            {
                tasksViewer.AddTask(newPopulatedTask);
            }
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

            parentPage.AppBar.Visibility = Visibility.Collapsed;
            parentPage.MainSplitView.Pane.Visibility = Visibility.Collapsed;

            var windowWidth = Window.Current.Bounds.Width;
            var windowHeight = Window.Current.Bounds.Height;

            rectBackgroundHide.Visibility = Visibility.Visible;

            parentPage.CalculateTaskSizes(windowWidth, windowHeight);

            taskPopup.Child = popupTask;
            taskPopup.IsOpen = true;

            return popupTask;
        }

        public void ResizePopup()
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

            var windowHeight = parentPage.MainSplitView.ActualHeight;
            var windowWidth = parentPage.MainSplitView.ActualWidth;

            if (windowWidth < 720)
            {
                windowHeight = windowHeight + parentPage.MainTitleBar.ActualHeight;
            }

            popupTask.ResizeContent(windowWidth, windowHeight);

            double NewHorizontalOffset = ((windowWidth - popupTask.ActualWidth) / 2);
            double NewVerticalOffset = (windowHeight - popupTask.ActualHeight) / 2;

            if (ActualHorizontalOffset != NewHorizontalOffset || ActualVerticalOffset != NewVerticalOffset)
            {
                taskPopup.HorizontalOffset = NewHorizontalOffset;
                taskPopup.VerticalOffset = NewVerticalOffset;
            }
        }

        private void PopupTask_TaskSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePopup();
        }

        private void PopupTask_CancelClick(object sender, RoutedEventArgs e)
        {
            parentPage.HidePopupTask();
        }

        private void PopupTask_SaveClick(object sender, RoutedEventArgs e)
        {
            parentPage.HidePopupTask();

            var popupTask = sender as PopupTask;
            var updatedTask = popupTask.ConnectedTaskControl;

            var populatedTask = GetPopulatedTaskByType(updatedTask.GetType());
            if (populatedTask != null)
            {
                populatedTask.RefreshTaskSummary(updatedTask);
            }
        }

        private PopulatedTask GetPopulatedTaskByType(Type taskType)
        {
            var task = tasksViewer.GetPopulatedTaskByType(taskType);
            if (task != null)
            {
                return task;
            }

            return null;
        }

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

        public void taskTapped(object sender, TappedRoutedEventArgs e)
        {
            var populatedTask = sender as PopulatedTask;

            if (populatedTask != null)
            {
                var popupTask = CreatePopupTaskFromPopulatedTask(populatedTask);

                taskPopup.Child = popupTask;
                taskPopup.IsOpen = true;
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

            parentPage.TextForShare = connectedControl.GetDataAsText();

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

        public PopulatedTask GetPopulatedTaskByTaskName(string taskName)
        {
            return tasksViewer.GetPopulatedTaskByTaskName(taskName);
        }

        public async Task<bool> CreateTask(string taskName)
        {
            var taskType = TaskData.GetTaskType(taskName);
            var taskControl = await TaskData.CreateTaskControlByType(taskType);
            if (taskControl == null)
            {
                return false;
            }

            var created = CreateTaskControl(taskControl, taskTapped, onTaskEdit, onTaskDelete, onTaskShare, onTaskExport);
            if (!created)
            {
                return false;
            }

            var newPopulatedTask = GetPopulatedTaskByTaskName(taskName);

            taskTapped(newPopulatedTask, new TappedRoutedEventArgs());

            return true;
        }
    }
}
