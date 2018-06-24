using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Business.Models.Enums;
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
        public EventHandler PageLoaded;

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

        public TasksViewPage()
        {
            this.InitializeComponent();
            this.Loaded += TasksViewPage_Loaded;
        }

        public TasksViewPage(TaskCategories category, MainPage parent)
        {
            this.InitializeComponent();
            this.Loaded += TasksViewPage_Loaded;

            tasksViewer.TasksCategory = category;
            parentPage = parent;
        }

        private void TasksViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTasks();

            PageLoaded?.Invoke(this, new EventArgs());
        }

        private void LoadTasks()
        {
            tasksViewer.PopulateTasks();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Params result = (Params)e.Parameter;
            tasksViewer.TasksCategory = result.Category;
            parentPage = result.CurrentPage;
            tasksViewer.ParentPage = parentPage;

            base.OnNavigatedTo(e);
        }

        public void UpdateTasks(List<BaseTaskControl> controls)
        {
            tasksViewer.UpdateTasks(controls);
        }

        public async Task<bool> CreateTask(string taskName)
        {
            var result = await tasksViewer.CreateTask(taskName);
            return result;
        }
    }
}
