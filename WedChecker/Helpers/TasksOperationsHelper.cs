using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.UserControls;
using WedChecker.UserControls.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WedChecker.Helpers
{
    public class TasksOperationsHelper
    {
        public static async Task<bool> DeleteTaskAsync(TasksViewer tasksViewer, BaseTaskControl taskControl)
        {
            var msgDialog = new MessageDialog("Are you sure you want to delete this task?", "Please confirm");
            msgDialog.Commands.Add(new UICommand("Delete")
            {
                Id = 0
            });
            msgDialog.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });

            msgDialog.DefaultCommandIndex = 0;
            msgDialog.CancelCommandIndex = 1;

            var result = await msgDialog.ShowAsync();

            if ((int)result.Id == 0)
            {
                await DeleteTask(tasksViewer, taskControl);
            }
            else
            {
                return false;
            }

            return true;
        }

        private static async Task DeleteTask(TasksViewer tasksViewer, BaseTaskControl taskControl)
        {
            if (taskControl != null)
            {
                EnableTaskTile(tasksViewer.ParentPage, taskControl);

                tasksViewer.RemoveTask(taskControl);

                await taskControl.DeleteValues();
            }
        }

        private static void EnableTaskTile(MainPage page, BaseTaskControl taskControl)
        {
            var mainGrid = page.FindName("mainGrid") as Grid;
            if (mainGrid == null)
            {
                return;
            }

            var taskDialog = mainGrid.Children.OfType<TaskDialog>().FirstOrDefault();
            if (taskDialog != null)
            {
                taskDialog.IsTileEnabled(taskControl.TaskCode, true);
            }
        }
    }
}
