using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using WedChecker.Interfaces;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WedChecker.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page, IUpdateableTasks
    {
        public EventHandler PageLoaded;

        public TaskCategories TasksCategory
        {
            get
            {
                return TaskCategories.Home;
            }
        }

        public HomePage()
        {
            this.InitializeComponent();

            this.Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            var controls = await AppData.PopulateAddedControls();
            UpdateTasks(controls);
            
            PageLoaded?.Invoke(this, new EventArgs());
        }

        private void tbCountdownTimer_WeddingPassed(object sender, EventArgs e)
        {
            tbWeddingPassed.Visibility = Visibility.Visible;
            tbCountdownTimer.Visibility = Visibility.Collapsed;
        }

        public void UpdateTasks(List<BaseTaskControl> controls)
        {
            tbGreetUser.Text = $"Hello, {Core.GetSetting("Name")}";
            tbGreetUser.Visibility = Visibility.Visible;
            tbCountdownTimer.Visibility = Visibility.Visible;

            tasksSummary.LoadTasksData(controls);
        }
    }
}
