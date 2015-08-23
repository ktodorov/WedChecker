using System.IO;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class MusicLayout : BaseTaskControl
    {
        private string PlannedLayout { get; set; } = string.Empty;

        public override string TaskName
        {
            get
            {
                return "Music layout";
            }
        }

        public MusicLayout()
        {
            this.InitializeComponent();
        }

        public MusicLayout(string value)
        {
            this.InitializeComponent();
            PlannedLayout = value;
        }

        public override void DisplayValues()
        {
            tbMusicLayoutDisplay.Text = PlannedLayout;
            displayPanel.Visibility = Visibility.Visible;
            tbHeader.Text = "This is the music layout you have planned";
            tbMusicLayout.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbMusicLayout.Text = tbMusicLayoutDisplay.Text;
            tbMusicLayout.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
            tbHeader.Text = "Here you can save the notes about music layout, planned for the wedding";
        }


        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.MusicLayout.ToString());
            writer.Write(PlannedLayout);
        }

        public override void Deserialize(BinaryReader reader)
        {
            PlannedLayout = reader.ReadString();
            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            var decoration = tbMusicLayout.Text;
            if (string.IsNullOrEmpty(decoration))
            {
                tbErrorMessage.Text = "Please, do not enter an empty music notes.";
                return;
            }

            if (PlannedLayout != decoration)
            {
                PlannedLayout = decoration;
                await AppData.InsertGlobalValue(TaskData.Tasks.MusicLayout.ToString());
            }
        }

        private void tbMusicLayout_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbMusicLayoutDisplay.Text = tbMusicLayout.Text;
        }
    }
}
