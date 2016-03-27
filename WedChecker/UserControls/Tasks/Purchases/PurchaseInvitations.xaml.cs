using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseInvitations : BaseTaskControl
    {
        public static new string TaskName
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
                return "Have you purchased your invitations yet?";
            }
        }

        public static new string DisplayHeader
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
                return TaskData.Tasks.PurchaseInvitations.ToString();
            }
        }

        public PurchaseInvitations()
        {
            this.InitializeComponent();
        }

        public PurchaseInvitations(bool purchased)
        {
            this.InitializeComponent();

            invitationPurchasedToggle.Toggled = true;
        }

        public override void DisplayValues()
        {
            invitationPurchasedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            invitationPurchasedToggle.EditValues();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            invitationPurchasedToggle.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            invitationPurchasedToggle.Deserialize(reader);
        }
    }
}
