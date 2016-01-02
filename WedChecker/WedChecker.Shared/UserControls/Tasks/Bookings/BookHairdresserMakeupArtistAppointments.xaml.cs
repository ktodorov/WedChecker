using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookHairdresserMakeupArtistAppointments : BaseTaskControl
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

        public override string DisplayHeader
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

        public BookHairdresserMakeupArtistAppointments()
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            hairdresserAppointmentsBookedToggle.DisplayValues();
            makeupArtistAppointmentsBookedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            hairdresserAppointmentsBookedToggle.EditValues();
            makeupArtistAppointmentsBookedToggle.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskCode);

            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());

            writer.Write("Hairdresser");
            hairdresserAppointmentsBookedToggle.Serialize(writer);

            writer.Write("MakeupArtist");
            makeupArtistAppointmentsBookedToggle.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var type = reader.ReadString();

                if (type == "Hairdresser")
                {
                    hairdresserAppointmentsBookedToggle.Deserialize(reader);
                }
                else if (type == "MakeupArtist")
                {
                    makeupArtistAppointmentsBookedToggle.Deserialize(reader);
                }
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskCode);
        }
    }
}
