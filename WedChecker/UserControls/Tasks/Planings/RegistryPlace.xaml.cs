using System.IO;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public static new string TaskName
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

        public static new string DisplayHeader
        {
            get
            {
                return "These are your notes";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.RegistryPlace.ToString();
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

        protected override void Serialize(BinaryWriter writer)
        {
            var registryNotes = tbRegistryNotes.Text;
            RegistryNotes = registryNotes;

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

        protected override void Deserialize(BinaryReader reader)
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

        private void tbRegistryNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbRegistryNotesDisplay.Text = tbRegistryNotes.Text;
        }
    }
}
