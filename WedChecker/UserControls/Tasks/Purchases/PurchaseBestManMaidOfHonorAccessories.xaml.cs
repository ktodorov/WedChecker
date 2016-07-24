using System.Collections.Generic;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseBMMOHAccessories : PurchaseTaskBaseControl
    {
        protected override List<string> CategoryNames
        {
            get
            {
                var categories = new List<string> { "BestMan", "MaidOfHonor" };
                return categories;
            }
        }

        protected override List<string> ItemsAppDataName
        {
            get
            {
                var result = new List<string> { "BestMenAccessories", "MaidsOfHonorAccessories" };
                return result;
            }
        }

        protected override string ItemsMissingExceptionText
        {
            get
            {
                return "You must first add accessories for the best men or maids of honor in order to mark them purchased after that!";
            }
        }

        public override string TaskName
        {
            get
            {
                return "Best man and maid of honor accessories";
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
                return TaskData.Tasks.PurchaseBMMOHAccessories.ToString();
            }
        }
    }
}
