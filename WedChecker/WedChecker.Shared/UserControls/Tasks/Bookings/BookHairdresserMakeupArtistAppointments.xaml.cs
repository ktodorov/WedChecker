using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            writer.Write(TaskData.Tasks.BookHairdresserMakeupArtistAppointments.ToString());

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
            await AppData.InsertGlobalValue(TaskData.Tasks.BookHairdresserMakeupArtistAppointments.ToString());
        }
    }
}
