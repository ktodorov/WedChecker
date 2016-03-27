using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Extensions;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
	public abstract partial class BaseTaskControl : UserControl
	{
		public BaseTaskControl()
		{
			this.InitializeComponent();
		}

		public static string TaskName { get; }
		public virtual string EditHeader { get; }
		public static string DisplayHeader { get; }
		public virtual string TaskCode { get; }

		public string ErrorMessage { get; set; }

		public bool PriorityTask;

		public virtual void DisplayValues()
		{

		}

		public virtual void EditValues()
		{

		}

		protected virtual void Serialize(BinaryWriter writer)
		{

		}

		protected virtual void Deserialize(BinaryReader reader)
		{
		}

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
	}
}
