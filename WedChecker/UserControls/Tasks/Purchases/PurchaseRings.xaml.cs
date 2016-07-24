using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseRings : PurchaseTaskBaseControl
    {
        protected override List<List<string>> PredefinedItems
        {
            get
            {
                var result = new List<List<string>> { new List<string> { "Purchased" } };
                return result;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Wedding rings";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you purchased your rings yet?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the rings so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.PurchaseRings.ToString();
            }
        }
    }
}
