using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using WedChecker.UserControls.Tasks;
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

        public TasksViewer()
        {
            this.InitializeComponent();
        }

        public TasksViewer(TaskCategories category)
        {
            this.InitializeComponent();

            TasksCategory = category;
        }

        public void AddTask(PopulatedTask task)
        {
            gvTasks.Items.Add(task);
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

            var columns = (int)Math.Round(width / 400.0);
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
            var newHeight = panel.ItemWidth / 2;
            if (newHeight < 200)
            {
                newHeight = 200;
            }
            panel.ItemHeight = newHeight;
        }
    }
}
