using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Infrastructure;
using WedChecker.UserControls;
using WedChecker.UserControls.Tasks;
using WedChecker.UserControls.Tasks.Plannings;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
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

        public static async Task<bool> DeleteTasksAsync(TasksViewer tasksViewer, List<BaseTaskControl> taskControls)
        {
            var msgDialog = new MessageDialog("Are you sure you want to delete these tasks?", "Please confirm");
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
                foreach (var taskControl in taskControls)
                {
                    await DeleteTask(tasksViewer, taskControl);
                }
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

        public static async void ShareAllTasks()
        {
            var allControls = await AppData.PopulateAddedControls();
            ShareTasks(allControls);
        }

        public async static void ShareTasks(List<BaseTaskControl> controls)
        {
            if (controls == null || !controls.Any())
            {
                var dialog = new MessageDialog("Please choose a task before sharing", "Error");
                await dialog.ShowAsync();
                return;
            }

            var text = new StringBuilder();

            foreach (var control in controls)
            {
                text.AppendLine("----------------------------------------------------------");
                var controlText = control.GetDataAsText();
                text.Append(controlText);
                text.AppendLine("----------------------------------------------------------");
            }

            AppData.TextForShare = text.ToString();

            DataTransferManager.ShowShareUI();
        }

        public static async void ExportAllTasks()
        {
            var allControls = await AppData.PopulateAddedControls();
            ExportTasks(allControls);
        }

        public static async void ExportTasks(List<BaseTaskControl> controls)
        {
            if (controls == null || !controls.Any())
            {
                var dialog = new MessageDialog("Please choose a task before exporting", "Error");
                await dialog.ShowAsync();
                return;
            }

            var text = new StringBuilder();

            foreach (var control in controls)
            {
                text.AppendLine("----------------------------------------------------------");
                var controlText = control.GetDataAsText();
                text.Append(controlText);
                text.AppendLine("----------------------------------------------------------");
            }

            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            savePicker.SuggestedFileName = "WedChecker export";

            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
            {
                return;
            }

            // Prevent updates to the remote version of the file until
            // we finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(file);

            // write to file
            await FileIO.WriteTextAsync(file, text.ToString());

            // Let Windows know that we're finished changing the file so
            // the other app can update the remote version of the file.
            // Completing updates may require Windows to ask for user input.
            Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
            {
                var dialog = new MessageDialog("File saved", "Success");
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new MessageDialog("File could not be saved", "Something happened");
                await dialog.ShowAsync();
            }
        }

        public static async Task AddGuest(WedCheckerContact guest)
        {
            var msgDialog = new MessageDialog($"Do you want to add {guest.FullName} to the guests list?", "Please confirm");
            msgDialog.Commands.Add(new UICommand("Yes")
            {
                Id = 0
            });
            msgDialog.Commands.Add(new UICommand("No")
            {
                Id = 1
            });

            msgDialog.DefaultCommandIndex = 0;
            msgDialog.CancelCommandIndex = 1;

            var result = await msgDialog.ShowAsync();

            if ((int)result.Id != 0)
            {
                return;
            }

            if (guest == null)
            {
                return;
            }

            var newContactControl = await CreateContactControl(guest);
            if (newContactControl == null)
            {
                return;
            }

            var guestsListTask = new GuestsList();
            await guestsListTask.DeserializeValues();
            guestsListTask.Guests.Add(newContactControl);
            await guestsListTask.SubmitValues();
        }

        private static async Task<ContactControl> CreateContactControl(WedCheckerContact guest)
        {
            if (guest == null)
            {
                return null;
            }

            var guests = AppData.Guests;
            if (guests == null)
            {
                guests = await AppData.DeserializeGuests();
                AppData.Guests = guests;
            }

            if (guests.Any(g => g.FullName == guest.FullName))
            {
                var msgDialog = new MessageDialog($"There is already a guest named '{guest.FullName}'. Are you sure you want to add new guest with the same name?", "Please confirm");
                msgDialog.Commands.Add(new UICommand("Yes")
                {
                    Id = 0
                });
                msgDialog.Commands.Add(new UICommand("No")
                {
                    Id = 1
                });

                msgDialog.DefaultCommandIndex = 0;
                msgDialog.CancelCommandIndex = 1;

                var result = await msgDialog.ShowAsync();

                if ((int)result.Id != 0)
                {
                    return null;
                }
            }

            var newContactControl = new ContactControl(guest.ToContact());
            return newContactControl;
        }

        public static async Task AddGuests(params WedCheckerContact[] guestsToAdd)
        {
            var validGuests = guestsToAdd.Where(g => g != null && !string.IsNullOrEmpty(g.FullName)).ToList();
            if (!validGuests.Any())
            {
                return;
            }

            var message = $"Do you want to add those {validGuests.Count()} contacts to the guests list?";
            if (validGuests.Count == 1)
            {
                message = $"Do you want to add {validGuests.FirstOrDefault()?.FullName} to the guests list?";
            }

            var msgDialog = new MessageDialog(message, "Please confirm");
            msgDialog.Commands.Add(new UICommand("Yes")
            {
                Id = 0
            });
            msgDialog.Commands.Add(new UICommand("No")
            {
                Id = 1
            });

            msgDialog.DefaultCommandIndex = 0;
            msgDialog.CancelCommandIndex = 1;

            var result = await msgDialog.ShowAsync();

            if ((int)result.Id != 0)
            {
                return;
            }

            var guestsListTask = new GuestsList();
            await guestsListTask.DeserializeValues();

            foreach (var guest in validGuests)
            {
                var newContactControl = await CreateContactControl(guest);
                if (newContactControl == null)
                {
                    return;
                }

                guestsListTask.Guests.Add(newContactControl);
            }

            await guestsListTask.SubmitValues();
        }
    }
}
