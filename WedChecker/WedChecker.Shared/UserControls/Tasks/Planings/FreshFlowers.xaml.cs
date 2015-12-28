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

        public override string DisplayHeader
        {
            get
            {
                return "This is the flowers info you have noted";
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
            writer.Write(TaskData.Tasks.FreshFlowers.ToString());

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
                flowersMap.SerializeMapData(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
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
                    flowersMap.DeserializeMapData(reader);
                }
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            var decoration = tbFreshFlowers.Text;
            if (string.IsNullOrEmpty(decoration))
            {
                tbErrorMessage.Text = "Please, do not enter an empty flowers information";
                return;
            }

            if (FlowersNotes != decoration)
            {
                FlowersNotes = decoration;
                await AppData.InsertGlobalValue(TaskData.Tasks.FreshFlowers.ToString());
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
    }
}
