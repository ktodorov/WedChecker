using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public sealed partial class FreshFlowers : BaseTaskControl
    {
        private string FlowersNotes { get; set; } = string.Empty;

        public override string TaskName
        {
            get
            {
                return "Flowers";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can save any info about the flowers, you have planned for the wedding";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is the flowers info you have noted";
            }
        }

        public override string TaskCode
        {
            get
            {
                return Business.Models.Enums.Tasks.FreshFlowers.ToString();
            }
        }

        public FreshFlowers()
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            tbFreshFlowersDisplay.Text = FlowersNotes;
            displayPanel.Visibility = Visibility.Visible;
            tbFreshFlowers.Visibility = Visibility.Collapsed;

            flowersMap.DisplayValues();
        }

        public override void EditValues()
        {
            tbFreshFlowers.Text = tbFreshFlowersDisplay.Text;
            tbFreshFlowers.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;

            flowersMap.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            var decoration = tbFreshFlowers.Text;
            if (FlowersNotes != decoration)
            {
                FlowersNotes = decoration;
            }

            var objectsCount = GetObjectsCount();
            writer.Write(objectsCount);

            if (!string.IsNullOrEmpty(FlowersNotes))
            {
                writer.Write("FlowersNotes");
                writer.Write(FlowersNotes);
            }

            if (flowersMap.HasPinnedLocation())
            {
                writer.Write("PinnedLocation");
                flowersMap.Serialize(writer);
            }
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            for (var i = 0; i < objectsCount; i++)
            {
                var type = reader.ReadString();

                if (type == "FlowersNotes")
                {
                    FlowersNotes = reader.ReadString();
                }
                else if (type == "PinnedLocation")
                {
                    flowersMap.Deserialize(reader);
                }
            }
        }

        private void tbFreshFlowers_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbFreshFlowersDisplay.Text = tbFreshFlowers.Text;
        }

        int GetObjectsCount()
        {
            var objectsCount = 0;

            if (!string.IsNullOrEmpty(FlowersNotes))
            {
                objectsCount++;
            }

            if (flowersMap.HasPinnedLocation())
            {
                objectsCount++;
            }

            return objectsCount;
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            var decoration = tbFreshFlowers.Text;
          
            if (!string.IsNullOrEmpty(decoration))
            {
                sb.AppendLine("Notes:");
                sb.AppendLine(decoration);
            }

            if (flowersMap.HasPinnedLocation())
            {
                sb.AppendLine("Pinned location:");
                var mapLocationAsText = flowersMap.GetDataAsText();
                sb.AppendLine(mapLocationAsText);
            }
        }
    }
}
