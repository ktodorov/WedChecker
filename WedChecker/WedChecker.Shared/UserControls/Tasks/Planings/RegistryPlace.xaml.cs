using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.Devices.Geolocation;
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

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class RegistryPlace : BaseTaskControl
    {
        private string RegistryNotes
        {
            get;
            set;
        }

        public override string TaskName
        {
            get
            {
                return "Registry place";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can add address or notes or whatever you like for your registry place";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "These are your notes";
            }
        }

        public RegistryPlace()
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            tbRegistryNotesDisplay.Text = RegistryNotes ?? string.Empty;
            tbRegistryNotesDisplay.Visibility = Visibility.Visible;
            tbRegistryNotes.Visibility = Visibility.Collapsed;

            registryMap.DisplayValues();
        }

        public override void EditValues()
        {
            tbRegistryNotes.Text = tbRegistryNotesDisplay.Text;
            tbRegistryNotes.Visibility = Visibility.Visible;
            tbRegistryNotesDisplay.Visibility = Visibility.Collapsed;

            registryMap.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.RegistryPlace.ToString());

            var objectsCount = GetObjectsCount();
            writer.Write(objectsCount);

            if (!string.IsNullOrEmpty(RegistryNotes))
            {
                writer.Write("RegistryNotes");
                writer.Write(RegistryNotes);
            }
            if (registryMap.HasPinnedLocation())
            {
                writer.Write("RegistryMap");
                registryMap.SerializeMapData(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            for (var i = 0; i < objectsCount; i++)
            {
                var type = reader.ReadString();

                if (type == "RegistryNotes")
                {
                    RegistryNotes = reader.ReadString();
                }
                else if (type == "RegistryMap")
                {
                    registryMap.DeserializeMapData(reader);
                }
            }

            DisplayValues();
        }

        int GetObjectsCount()
        {
            var objectsCount = 0;

            if (!string.IsNullOrEmpty(RegistryNotes))
            {
                objectsCount++;
            }

            if (registryMap.HasPinnedLocation())
            {
                objectsCount++;
            }

            return objectsCount;
        }

        public override async Task SubmitValues()
        {
            var registryNotes = tbRegistryNotes.Text;

            if (RegistryNotes != registryNotes)
            {
                RegistryNotes = registryNotes;
                await AppData.InsertGlobalValue(TaskData.Tasks.RegistryPlace.ToString());
            }
        }

        private void tbRegistryNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbRegistryNotesDisplay.Text = tbRegistryNotes.Text;
        }
    }
}
