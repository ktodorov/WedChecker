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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class GuestsList : BaseTaskControl
    {
        private List<Contact> Guests
        {
            get;
            set;
        }

        public override string TaskName
        {
            get
            {
                return "Guests list";
            }
        }

        public GuestsList()
        {
            this.InitializeComponent();
        }

        public GuestsList(List<Contact> contacts)
        {
            this.InitializeComponent();
            Guests = contacts;
            foreach (var guest in Guests)
            {
                var contactControl = new ContactControl(guest.Id, guest.FirstName + " " + guest.LastName);
                contactControl.deleteButton.Click += deleteButton_Click;

                spContacts.Children.Add(contactControl);
            }

            DisplayValues(-1);
        }

        public override void DisplayValues(int value)
        {
            spContacts.Visibility = Visibility.Visible;
            selectContacts.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            spContacts.Visibility = Visibility.Collapsed;
            selectContacts.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.GuestsList);
            writer.Write(Guests.Count);
            foreach (var guest in Guests)
            {
                writer.Write(guest.Id);
                writer.Write(guest.FirstName);
                writer.Write(guest.LastName);
            }
        }

        public override BaseTaskControl Deserialize(BinaryReader reader)
        {
            var contacts = new List<Contact>();

            //Read in the number of records
            var records = reader.ReadInt32();

            for (long i = 0; i < records; i++)
            {
                var guestId = reader.ReadString();

                var guestFirstName = reader.ReadString();
                var guestLastName = reader.ReadString();

                var contact = new Contact();
                contact.Id = guestId;
                contact.FirstName = guestFirstName;
                contact.LastName = guestLastName;

                contacts.Add(contact);
            }

            return new GuestsList(contacts);
        }

        private async void selectContacts_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.ApplicationModel.Contacts.ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(Windows.ApplicationModel.Contacts.ContactFieldType.PhoneNumber);
            var contacts = await picker.PickContactsAsync();

            if (!contacts.Any())
            {
                return;
            }

            if (Guests == null)
            {
                Guests = new List<Contact>();
            }

            foreach (var contact in contacts)
            {
                if (!Guests.Contains(contact))
                {
                    Guests.Add(contact);
                }

                var contactControl = new ContactControl(contact.Id, contact.FirstName + " " + contact.LastName);
                contactControl.deleteButton.Click += deleteButton_Click;

                spContacts.Children.Add(contactControl);
            }

            DisplayValues(-1);
            AppData.InsertGlobalValue(TaskData.Tasks.GuestsList, "true");
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGuest(contactControl.tbId.Text);
        }

        private void DeleteGuest(string id)
        {
            var guestToRemove = Guests.FirstOrDefault(g => g.Id == id);

            if (guestToRemove != null)
            {
                Guests.Remove(guestToRemove);
            }

            var controlToRemove = spContacts.Children.FirstOrDefault(c => c is ContactControl && ((ContactControl)c).tbId.Text == guestToRemove.Id);
            if (controlToRemove != null)
            {
                spContacts.Children.Remove(controlToRemove);
            }
        }

    }
}
