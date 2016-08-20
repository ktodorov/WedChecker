using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Helpers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public sealed partial class HairdresserMakeupArtist : BaseTaskControl
    {

        public override string TaskName
        {
            get
            {
                return "Hairdresser and makeup artist";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Select the haidresser or the makeup artist or you know - both of them";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "Here is the info that you have provided about those two";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.HairdresserMakeupArtist.ToString();
            }
        }

        public HairdresserMakeupArtist()
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            ccHairdresser.DisplayValues();
            ccMakeupArtist.DisplayValues();
        }

        public override void EditValues()
        {
            ccHairdresser.EditValues();
            ccMakeupArtist.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            if (!ccHairdresser.IsStored && !ccMakeupArtist.IsStored)
            {
                return;
            }

            var storedContactsCount = 0;
            if (ccHairdresser.IsStored)
            {
                storedContactsCount++;
            }
            if (ccMakeupArtist.IsStored)
            {
                storedContactsCount++;
            }

            writer.Write(storedContactsCount);

            if (ccHairdresser.IsStored)
            {
                writer.Write("Hairdresser");
                ccHairdresser.Serialize(writer);
            }

            if (ccMakeupArtist.IsStored)
            {
                writer.Write("MakeupArtist");
                ccMakeupArtist.Serialize(writer);
            }

            if (IsNew)
            {
                TasksOperationsHelper.AddGuests(ccHairdresser.StoredContact, ccMakeupArtist.StoredContact);
            }
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            var storedContactsCount = reader.ReadInt32();

            for (var i = 0; i < storedContactsCount; i++)
            {
                var contactType = reader.ReadString();

                if (contactType == "Hairdresser")
                {
                    ccHairdresser.Deserialize(reader);
                }
                else if (contactType == "MakeupArtist")
                {
                    ccMakeupArtist.Deserialize(reader);
                }
            }
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            if (ccHairdresser.IsStored)
            {
                sb.AppendLine("Hairdresser:");
                var contactAsText = ccHairdresser.GetDataAsText();
                sb.AppendLine(contactAsText);
            }

            if (ccMakeupArtist.IsStored)
            {
                sb.AppendLine("Makeup artist:");
                var contactAsText = ccMakeupArtist.GetDataAsText();
                sb.AppendLine(contactAsText);
            }
        }
    }
}
