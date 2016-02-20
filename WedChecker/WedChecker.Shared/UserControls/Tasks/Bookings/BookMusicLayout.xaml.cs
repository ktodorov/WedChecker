using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookMusicLayout : BaseTaskControl
    {

        public override string TaskName
        {
            get
            {
                return "Music layout";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you booked your music layout yet?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the music layout so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BookMusicLayout.ToString();
            }
        }

        public BookMusicLayout()
        {
            this.InitializeComponent();
        }

        public BookMusicLayout(bool booked)
        {
            this.InitializeComponent();

            musicLayoutBookedToggle.Toggled = true;
        }

        public override void DisplayValues()
        {
            musicLayoutBookedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            musicLayoutBookedToggle.EditValues();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            musicLayoutBookedToggle.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            musicLayoutBookedToggle.Deserialize(reader);
        }
    }
}
