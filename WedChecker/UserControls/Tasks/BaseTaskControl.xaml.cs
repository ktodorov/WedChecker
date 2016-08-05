using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Extensions;
using WedChecker.Interfaces;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public abstract partial class BaseTaskControl : UserControl, IStorableTask
    {
        public BaseTaskControl()
        {
            this.InitializeComponent();
        }

        public virtual string TaskName { get; }
        public virtual string EditHeader { get; }
        public static string DisplayHeader { get; }
        public virtual string TaskCode { get; }

        public virtual bool HasOwnScrollViewer { get { return false; } }

        public string ErrorMessage { get; set; }

        public virtual void SetOwnScrollViewerHeight(double maxHeight, double maxWidth)
        {
        }

        public bool PriorityTask;

        public DateTime CreatedOn;
        public DateTime ModifiedOn;

        public abstract void DisplayValues();

        public abstract void EditValues();

        public abstract void Serialize(BinaryWriter writer);

        public abstract void Deserialize(BinaryReader reader);

        protected virtual void SetLocalStorage()
        {
        }

        public async Task DeserializeValues()
        {
            try
            {
                var fileName = $"{TaskCode}.txt";
                var folder = Core.RoamingFolder;
                StorageFile file = await folder.GetFileAsync(fileName);

                using (Stream stream = await file.OpenStreamForReadAsync())
                using (BinaryReader reader = new BinaryReader(stream, Encoding.Unicode, true))
                {
                    try
                    {
                        var major = reader.ReadUInt16();
                        var minor = reader.ReadUInt16();
                        var build = reader.ReadUInt16();
                        var revision = reader.ReadUInt16();

                        this.Deserialize(reader);
                    }
                    // Then we have reached end of stream and failed to return before that
                    catch (EndOfStreamException)
                    {
                        return;
                    }
                }

                CreatedOn = File.GetCreationTime(file.Path);
                ModifiedOn = File.GetLastWriteTime(file.Path);

                SetLocalStorage();

                DisplayValues();
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
                await ex.HandleException();
            }
        }

        public async Task SubmitValues()
        {
            try
            {
                var fileName = $"{TaskCode}.txt";
                var folder = Core.RoamingFolder;
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using (Stream stream = await file.OpenStreamForWriteAsync())
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode, true))
                {
                    writer.Write(Package.Current.Id.Version.Major);
                    writer.Write(Package.Current.Id.Version.Minor);
                    writer.Write(Package.Current.Id.Version.Build);
                    writer.Write(Package.Current.Id.Version.Revision);

                    this.Serialize(writer);
                }

                ModifiedOn = DateTime.Now;

                SetLocalStorage();
            }
            catch (Exception ex)
            {
                await ex.HandleException();
            }

            AppData.AllTasks.InsertTask(TaskCode);
        }

        public async Task DeleteValues()
        {
            AppData.AllTasks.DeleteTask(TaskCode);

            try
            {
                var fileName = $"{TaskCode}.txt";
                var folder = Core.RoamingFolder;
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await file.DeleteAsync();
            }
            catch (Exception ex)
            {
                await ex.HandleException();
            }
        }

        public string GetDataAsText()
        {
            var sb = new StringBuilder();

            sb.AppendLine(TaskName);

            LoadTaskDataAsText(sb);

            return sb.ToString();
        }

        public async void ExportDataAsTextFile()
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            savePicker.SuggestedFileName = "WedChecker export";

            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
            {
                return;
            }

            var text = GetDataAsText();

            // Prevent updates to the remote version of the file until
            // we finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(file);

            // write to file
            await FileIO.WriteTextAsync(file, text);

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

        protected abstract void LoadTaskDataAsText(StringBuilder sb);
    }
}
