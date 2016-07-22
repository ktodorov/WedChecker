using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class SendInvitations : BookTaskBaseControl
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
                return "Have you sent the invitations yet?";
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
                return TaskData.Tasks.SendInvitations.ToString();
            }
        }

        protected override List<string> PredefinedItems
        {
            get
            {
                var result = new List<string> { "Booked" };
                return result;
            }
        }
    }
}
