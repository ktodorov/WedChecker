﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public sealed partial class BookGuestsAccomodation : BaseTaskControl
    {
        private List<string> _storedAccomodationPlaces;

        public List<string> StoredAccomodationPlaces
        {
            get
            {
                if (_storedAccomodationPlaces == null)
                {
                    _storedAccomodationPlaces = AppData.GetStorage("AccomodationPlaces") as List<string>;
                }

                if (_storedAccomodationPlaces == null || !_storedAccomodationPlaces.Any())
                {
                    throw new WedCheckerInvalidDataException("You must first add accomodation places in order to mark it purchased after that!");
                }

                return _storedAccomodationPlaces;
            }
        }

        public static new string TaskName
        {
            get
            {
                return "Accomodation places";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "What have you booked so far?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have booked so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BookGuestsAccomodation.ToString();
            }
        }

        public BookGuestsAccomodation()
        {
            this.InitializeComponent();

            foreach (var accomodationPlace in StoredAccomodationPlaces)
            {
                var toggle = new ToggleControl();
                toggle.Title = accomodationPlace;
                AddToggle(toggle);
            }
        }

        public BookGuestsAccomodation(Dictionary<string, bool> accomodationPlaces)
        {
            this.InitializeComponent();

            foreach (var accomodationPlace in accomodationPlaces)
            {
                var toggle = new ToggleControl(accomodationPlace.Key, accomodationPlace.Value);
                AddToggle(toggle);
            }

            var remainingStoredAccomodationPlaces = StoredAccomodationPlaces.Where(sa => !accomodationPlaces.Any(a => a.Key == sa));
            foreach (var accomodationPlace in remainingStoredAccomodationPlaces)
            {
                var toggle = new ToggleControl(accomodationPlace);
                AddToggle(toggle);
            }
        }

        public override void DisplayValues()
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.EditValues();
            }
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            foreach (var toggle in toggles)
            {
                toggle.Serialize(writer);
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var toggle = new ToggleControl();
                toggle.Deserialize(reader);

                AddToggle(toggle);
            }
        }

        private void AddToggle(ToggleControl toggle)
        {
            if (!StoredAccomodationPlaces.Contains(toggle.Title))
            {
                return;
            }

            var allToggles = mainPanel.Children.OfType<ToggleControl>();

            if (allToggles.Any(t => t.Title == toggle.Title))
            {
                mainPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                return;
            }

            mainPanel.Children.Add(toggle);
        }
    }
}