using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseRestaurantFood : PurchaseTaskBaseControl
    {
        protected override List<string> ItemsAppDataName
        {
            get
            {
                var result = new List<string> { "RestaurantFood" };
                return result;
            }
        }

        protected override string ItemsMissingExceptionText
        {
            get
            {
                return "You must first add restaurant food in order to mark it purchased after that!";
            }
        }

        public override string TaskName
        {
            get
            {
                return "Restaurant food";
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
                return TaskData.Tasks.PurchaseRestaurantFood.ToString();
            }
        }
    }
}
