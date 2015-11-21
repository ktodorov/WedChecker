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
    public sealed partial class BookPhotographer : BaseTaskControl
    {

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
                return "Have you booked the photographer yet?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the photographer so far";
            }
        }

        public BookPhotographer()
        {
            this.InitializeComponent();
        }

        public BookPhotographer(bool booked)
        {
            this.InitializeComponent();

            photographerBookedToggle.Toggled = booked;
        }

        public override void DisplayValues()
        {
            photographerBookedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            photographerBookedToggle.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.BookPhotographer.ToString());

            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            photographerBookedToggle.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            photographerBookedToggle.Deserialize(reader);

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.BookPhotographer.ToString());
        }
    }
}
