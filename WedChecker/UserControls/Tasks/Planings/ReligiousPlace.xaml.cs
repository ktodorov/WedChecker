using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public sealed partial class ReligiousPlace : BaseTaskControl
    {
        private string ReligiousNotes
        {
            get;
            set;
        }

        public override string TaskName
        {
            get
            {
                return "Religious place";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can add address or notes or whatever you like for your religious place";
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
                return TaskData.Tasks.ReligiousPlace.ToString();
            }
        }

        public ReligiousPlace()
        {
            this.InitializeComponent();
        }

        public ReligiousPlace(string value)
        {
            this.InitializeComponent();
            ReligiousNotes = value;
        }

        public override void DisplayValues()
        {
            tbReligiousNotesDisplay.Text = ReligiousNotes ?? string.Empty;
            tbReligiousNotesDisplay.Visibility = Visibility.Visible;
            tbReligiousNotes.Visibility = Visibility.Collapsed;

            religiousPlaceMap.DisplayValues();
        }

        public override void EditValues()
        {
            tbReligiousNotes.Text = tbReligiousNotesDisplay.Text;
            tbReligiousNotes.Visibility = Visibility.Visible;
            tbReligiousNotesDisplay.Visibility = Visibility.Collapsed;

            religiousPlaceMap.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            var religiousNotes = tbReligiousNotes.Text;
            ReligiousNotes = religiousNotes;

            var objectsCount = GetObjectsCount();
            writer.Write(objectsCount);

            if (!string.IsNullOrEmpty(ReligiousNotes))
            {
                writer.Write("ReligiousNotes");
                writer.Write(ReligiousNotes);
            }
            if (religiousPlaceMap.HasPinnedLocation())
            {
                writer.Write("ReligiousPlaceMap");
                religiousPlaceMap.Serialize(writer);
            }
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            for (var i = 0; i < objectsCount; i++)
            {
                var type = reader.ReadString();

                if (type == "ReligiousNotes")
                {
                    ReligiousNotes = reader.ReadString();
                }
                else if (type == "ReligiousPlaceMap")
                {
                    religiousPlaceMap.Deserialize(reader);
                }
            }
        }

        int GetObjectsCount()
        {
            var objectsCount = 0;

            if (!string.IsNullOrEmpty(ReligiousNotes))
            {
                objectsCount++;
            }

            if (religiousPlaceMap.HasPinnedLocation())
            {
                objectsCount++;
            }

            return objectsCount;
        }

        private void tbReligiousNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbReligiousNotesDisplay.Text = tbReligiousNotes.Text;
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            var religiousNotes = tbReligiousNotes.Text;

            if (!string.IsNullOrEmpty(religiousNotes))
            {
                sb.AppendLine("Notes:");
                sb.AppendLine(religiousNotes);
            }

            if (religiousPlaceMap.HasPinnedLocation())
            {
                sb.AppendLine("Pinned location:");
                var mapLocationAsText = religiousPlaceMap.GetDataAsText();
                sb.AppendLine(mapLocationAsText);
            }
        }
    }
}
