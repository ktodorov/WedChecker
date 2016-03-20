using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookHoneymoonDestination : BaseTaskControl
    {

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
                return "Have you booked the honeymoon destination yet?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the honeymoon destination so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BookHoneymoonDestination.ToString();
            }
        }

        public BookHoneymoonDestination()
        {
            this.InitializeComponent();
        }

        public BookHoneymoonDestination(bool booked)
        {
            this.InitializeComponent();

            honeymoonDestinationBookedToggle.Toggled = booked;
        }

        public override void DisplayValues()
        {
            honeymoonDestinationBookedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            honeymoonDestinationBookedToggle.EditValues();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            honeymoonDestinationBookedToggle.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            honeymoonDestinationBookedToggle.Deserialize(reader);
        }
    }
}
