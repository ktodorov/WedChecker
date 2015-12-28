using System.IO;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class HoneymoonDestination : BaseTaskControl
    {
        private string HoneymoonNotes
        {
            get;
            set;
        }

        public override string TaskName
        {
            get
            {
                return "Honeymoon destination";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can add address or some notes about your planned honeymoon destination";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "Here is what you have about your honeymoon";
            }
        }

        public HoneymoonDestination()
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            tbHoneymoonNotesDisplay.Text = HoneymoonNotes ?? string.Empty;
            displayPanel.Visibility = Visibility.Visible;
            tbHoneymoonNotes.Visibility = Visibility.Collapsed;

            honeymoonMap.DisplayValues();
        }

        public override void EditValues()
        {
            tbHoneymoonNotes.Text = tbHoneymoonNotesDisplay.Text;
            tbHoneymoonNotes.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;

            honeymoonMap.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.HoneymoonDestination.ToString());

            var objectsCount = GetObjectsCount();
            writer.Write(objectsCount);

            if (!string.IsNullOrEmpty(HoneymoonNotes))
            {
                writer.Write("HoneymoonNotes");
                writer.Write(HoneymoonNotes);
            }
            if (honeymoonMap.HasPinnedLocation())
            {
                writer.Write("HoneymoonMap");
                honeymoonMap.SerializeMapData(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            for (var i = 0; i < objectsCount; i++)
            {
                var type = reader.ReadString();
                if (type == "HoneymoonNotes")
                {
                    HoneymoonNotes = reader.ReadString();
                }
                else if (type == "HoneymoonMap")
                {
                    honeymoonMap.DeserializeMapData(reader);
                }
            }

            DisplayValues();
        }

        int GetObjectsCount()
        {
            var objectsCount = 0;

            if (!string.IsNullOrEmpty(HoneymoonNotes))
            {
                objectsCount++;
            }

            if (honeymoonMap.HasPinnedLocation())
            {
                objectsCount++;
            }

            return objectsCount;
        }

        public override async Task SubmitValues()
        {
            var registryNotes = tbHoneymoonNotes.Text;

            if (HoneymoonNotes != registryNotes)
            {
                HoneymoonNotes = registryNotes;
                await AppData.InsertGlobalValue(TaskData.Tasks.HoneymoonDestination.ToString());
            }
        }

        private void tbHoneymoonNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbHoneymoonNotesDisplay.Text = tbHoneymoonNotes.Text;
        }
    }
}
