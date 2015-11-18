using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
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

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseInvitations : BaseTaskControl
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
                return "Have you purchased your invitations yet?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the invitations so far";
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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.PurchaseInvitations.ToString());

            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            invitationPurchasedToggle.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            invitationPurchasedToggle.Deserialize(reader);

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.PurchaseInvitations.ToString());
        }
    }
}
