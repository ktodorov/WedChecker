using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseBAGClothes : PurchaseTaskBaseControl
    {
        protected override List<string> CategoryNames
        {
            get
            {
                var categories = new List<string> { "Bridesmaids", "Groomsmen" };
                return categories;
            }
        }

        protected override List<string> ItemsAppDataName
        {
            get
            {
                var result = new List<string> { "BridesmaidsClothes", "GroomsmenClothes" };
                return result;
            }
        }

        protected override string ItemsMissingExceptionText
        {
            get
            {
                return "You must first add clothes for the bridesmaids or the groomsmen in order to mark them purchased after that!";
            }
        }

        public override string TaskName
        {
            get
            {
                return "Bridesmaids and groomsmen clothes";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "What have you purchased so far?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have purchased so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.PurchaseBAGClothes.ToString();
            }
        }
    }
}
