using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Helpers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public sealed partial class Photographer : BaseTaskControl
    {
        private bool FirstStart = true;

        public override string TaskName
        {
            get
            {
                return "Photographer";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Select the photographer from your contacts and I'll note everything about him under or you can type it on your own";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "Here is the photographer info that you have provided";
            }
        }

        public override string TaskCode
        {
            get
            {
                return Business.Models.Enums.Tasks.Photographer.ToString();
            }
        }

        public Photographer()
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            ccPhotographer.DisplayValues();
        }

        public override void EditValues()
        {
            ccPhotographer.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            ccPhotographer.Serialize(writer);

            if (IsNew)
            {
                TasksOperationsHelper.AddGuest(ccPhotographer.StoredContact);
            }
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            ccPhotographer.Deserialize(reader);
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            if (ccPhotographer.IsStored)
            {
                var contactAsText = ccPhotographer.GetDataAsText();
                sb.AppendLine(contactAsText);
            }
        }
    }
}
