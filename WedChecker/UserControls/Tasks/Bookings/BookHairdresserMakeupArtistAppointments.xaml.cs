using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookHairdresserMakeupArtistAppointments : BookTaskBaseControl
    {
        public override string TaskName
        {
            get
            {
                return "Hairdresser and makeup artist appointments";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you booked your hairdresser and makeup artist appointments yet?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the hairdresser and makeup artist appointments so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BookHairdresserMakeupArtistAppointments.ToString();
            }
        }

        protected override List<string> PredefinedItems
        {
            get
            {
                var result = new List<string> { "Hairdresser", "Makeup Artist" };
                return result;
            }
        }
    }
}
