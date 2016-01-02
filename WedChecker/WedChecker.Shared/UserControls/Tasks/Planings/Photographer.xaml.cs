using System.IO;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
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

        public override string DisplayHeader
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
                return TaskData.Tasks.Photographer.ToString();
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
            writer.Write(TaskCode);

            ccPhotographer.SerializeContact(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            ccPhotographer.DeserializeContact(reader);

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskCode);
        }
    }
}
