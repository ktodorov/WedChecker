using System.IO;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class HairdresserMakeupArtist : BaseTaskControl
    {

        public static new string TaskName
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

        protected override void Serialize(BinaryWriter writer)
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
                ccHairdresser.SerializeContact(writer);
            }

            if (ccMakeupArtist.IsStored)
            {
                writer.Write("MakeupArtist");
                ccMakeupArtist.SerializeContact(writer);
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var storedContactsCount = reader.ReadInt32();

            for (var i = 0; i < storedContactsCount; i++)
            {
                var contactType = reader.ReadString();

                if (contactType == "Hairdresser")
                {
                    ccHairdresser.DeserializeContact(reader);
                }
                else if (contactType == "MakeupArtist")
                {
                    ccMakeupArtist.DeserializeContact(reader);
                }
            }
        }
    }
}
