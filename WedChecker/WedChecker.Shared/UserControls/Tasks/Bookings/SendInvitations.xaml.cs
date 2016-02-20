using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class SendInvitations : BaseTaskControl
    {

        public override string TaskName
        {
            get
            {
                return "Invitations";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you sent the invitations yet?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the invitations so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.SendInvitations.ToString();
            }
        }

        public SendInvitations()
        {
            this.InitializeComponent();
        }

        public SendInvitations(bool booked)
        {
            this.InitializeComponent();

            invitationsSentToggle.Toggled = booked;
        }

        public override void DisplayValues()
        {
            invitationsSentToggle.DisplayValues();
        }

        public override void EditValues()
        {
            invitationsSentToggle.EditValues();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            invitationsSentToggle.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            invitationsSentToggle.Deserialize(reader);
        }
    }
}
