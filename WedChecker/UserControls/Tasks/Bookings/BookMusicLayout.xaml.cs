using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookMusicLayout : BookTaskBaseControl
    {

        public override string TaskName
        {
            get
            {
                return "Music layout";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you booked your music layout yet?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the music layout so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return Business.Models.Enums.Tasks.BookMusicLayout.ToString();
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
