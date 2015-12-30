using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Contacts;
using WedChecker.Common;
using System.IO.Compression;
using System.Text;
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

        public override string TaskName
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

        public override string DisplayHeader
        {
            get
            {
                return "These are the guests you have added so far";
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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.GuestsList.ToString());
            writer.Write(Guests.Count);
            foreach (var guest in Guests)
            {
                guest.SerializeContact(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
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
            DisplayValues();
        }

        private async void selectContacts_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.ApplicationModel.Contacts.ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(Windows.ApplicationModel.Contacts.ContactFieldType.PhoneNumber);
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

        public override async Task SubmitValues()
        {
            var guestsString = Guests.Count.ToString();
            foreach (var guest in Guests)
            {
                guestsString += $"{guest.StoredContact.Id}{AppData.GLOBAL_SEPARATOR}";
                guestsString += $"{guest.StoredContact.FirstName}{AppData.GLOBAL_SEPARATOR}";
                guestsString += $"{guest.StoredContact.LastName}{AppData.GLOBAL_SEPARATOR}";
                guestsString += $"{guest.StoredContact.Notes}{AppData.GLOBAL_SEPARATOR}";
            }

            await AppData.InsertGlobalValue(TaskData.Tasks.GuestsList.ToString(), guestsString);
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGuest(contactControl.StoredContact.Id);
        }

        private async void DeleteGuest(string id)
        {
            var guestToRemove = Guests.FirstOrDefault(g => g.StoredContact.Id == id);

            if (guestToRemove != null)
            {
                spContacts.Children.Remove(guestToRemove);
            }

            await AppData.SerializeData();
        }

        private void addNewContactButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = new ContactControl(true, true, true);
            contactControl.OnDelete = deleteButton_Click;
            spContacts.Children.Add(contactControl);
        }
    }
}
