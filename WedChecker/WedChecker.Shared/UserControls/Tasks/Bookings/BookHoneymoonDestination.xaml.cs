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
    public sealed partial class BookHoneymoonDestination : BaseTaskControl
    {

        public override string TaskName
        {
            get
            {
                return "Honeymoon destination";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you booked the honeymoon destination yet?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the honeymoon destination so far";
            }
        }

        public BookHoneymoonDestination()
        {
            this.InitializeComponent();
        }

        public BookHoneymoonDestination(bool booked)
        {
            this.InitializeComponent();

            honeymoonDestinationBookedToggle.Toggled = booked;
        }

        public override void DisplayValues()
        {
            honeymoonDestinationBookedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            honeymoonDestinationBookedToggle.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.BookHoneymoonDestination.ToString());

            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            honeymoonDestinationBookedToggle.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            honeymoonDestinationBookedToggle.Deserialize(reader);

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.BookHoneymoonDestination.ToString());
        }
    }
}
