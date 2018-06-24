using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Interfaces;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookGuestsAccomodation : BookTaskBaseControl
    {

        protected override string ItemsAppDataName
        {
            get
            {
                return "AccomodationPlaces";
            }
        }

        protected override string ItemsMissingExceptionText
        {
            get
            {
                return "You must first add accomodation places in order to mark it purchased after that!";
            }
        }

        public override string TaskName
        {
            get
            {
                return "Accomodation places";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "What have you booked so far?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have booked so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return Business.Models.Enums.Tasks.BookGuestsAccomodation.ToString();
            }
        }
    }
}
