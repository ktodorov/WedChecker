﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookPhotographer : BaseTaskControl
    {

        public static new string TaskName
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

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the photographer so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BookPhotographer.ToString();
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

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            photographerBookedToggle.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            photographerBookedToggle.Deserialize(reader);
        }
    }
}