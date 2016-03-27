using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Contacts;
using WedChecker.Common;
using System.Threading.Tasks;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class GuestsList : BaseTaskControl
    {
        public List<ContactControl> Guests
        {
            get
            {
                return spContacts.Children.OfType<ContactControl>().ToList();
            }
        }

        public static new string TaskName
        {
            get
            {
                return "Guests list";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can select the guests for your weddings";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "These are the guests you have added so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.GuestsList.ToString();
            }
        }

        public GuestsList()
        {
            this.InitializeComponent();
        }

        public override void DisplayValues()
        {
            editPanel.Visibility = Visibility.Collapsed;
            var guestControls = spContacts.Children.OfType<ContactControl>();
            foreach (var guestControl in guestControls)
            {
                guestControl.DisplayValues();
            }
        }

        public override void EditValues()
        {
            editPanel.Visibility = Visibility.Visible;
            var guestControls = spContacts.Children.OfType<ContactControl>();
            foreach (var guestControl in guestControls)
            {
                guestControl.EditValues();
            }
        }

        protected override void Serialize(BinaryWriter writer)
        {
            writer.Write(Guests.Count);
            foreach (var guest in Guests)
            {
                guest.SerializeContact(writer);
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var records = reader.ReadInt32();

            for (long i = 0; i < records; i++)
            {
                var contactControl = new ContactControl(true, true, true);

                contactControl.DeserializeContact(reader);
                contactControl.OnDelete = deleteButton_Click;

                spContacts.Children.Add(contactControl);
            }

            tbGuestsAdded.Text = string.Format("{0} guests added", Guests.Count);
        }

        private async void selectContacts_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contacts = await picker.PickContactsAsync();

            if (contacts == null || !contacts.Any())
            {
                return;
            }

            foreach (var contact in contacts)
            {
                if (!Guests.Any(g => g.StoredContact.Id == contact.Id))
                {
                    var contactControl = new ContactControl(contact, null, true, true, true);
                    contactControl.OnDelete = deleteButton_Click;

                    spContacts.Children.Add(contactControl);
                }
            }

            tbGuestsAdded.Text = string.Format("{0} guests added", Guests.Count);
        }

        protected override void SetLocalStorage()
        {
            var guestsContacts = Guests.Select(g => g.StoredContact).ToList();
            AppData.SetStorage("GuestsList", guestsContacts);
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGuest(contactControl.StoredContact.Id);
        }

        private void DeleteGuest(string id)
        {
            var guestToRemove = Guests.FirstOrDefault(g => g.StoredContact.Id == id);

            if (guestToRemove != null)
            {
                spContacts.Children.Remove(guestToRemove);
            }
        }

        private void addNewContactButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = new ContactControl(true, true, true);
            contactControl.OnDelete = deleteButton_Click;
            spContacts.Children.Add(contactControl);
        }
    }
}
