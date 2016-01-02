using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class BridesmaidsGroomsmen : BaseTaskControl
    {

        private List<ContactControl> Bridesmaids
        {
            get
            {
                return spBridesmaids.Children.OfType<ContactControl>().ToList();
            }
        }
        private List<ContactControl> Groomsmen
        {
            get
            {
                return spGroomsmen.Children.OfType<ContactControl>().ToList();
            }
        }


        public override string TaskName
        {
            get
            {
                return "Bridesmaids and groomsmen";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "You can choose as many bridesmaids and groomsmen as you will have";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "Here are the bridesmaids and groomsmen for your wedding";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BridesmaidsGroomsmen.ToString();
            }
        }

        public BridesmaidsGroomsmen()
        {
            this.InitializeComponent();
        }

        public BridesmaidsGroomsmen(List<KeyValuePair<string, Contact>> contacts)
        {
            this.InitializeComponent();
            var bridesmaids = contacts.Where(c => c.Key == "Bridesmaid")?.Select(c => c.Value);
            var groomsmen = contacts.Where(c => c.Key == "Groomsman")?.Select(c => c.Value);
            foreach (var bridesmaid in bridesmaids)
            {
                var bridesmaidControl = new ContactControl(bridesmaid);
                bridesmaidControl.OnDelete = deleteBridesmaidButton_Click;
                bridesmaidControl.Visibility = Visibility.Visible;
                spBridesmaids.Children.Add(bridesmaidControl);
            }

            foreach (var groomsman in groomsmen)
            {
                var groomsmanControl = new ContactControl(groomsman);
                groomsmanControl.OnDelete = deleteGroomsmanButton_Click;
                groomsmanControl.Visibility = Visibility.Visible;
                spGroomsmen.Children.Add(groomsmanControl);
            }
        }

        public override void DisplayValues()
        {
            selectBridesmaids.Visibility = Visibility.Collapsed;
            selectGroomsmen.Visibility = Visibility.Collapsed;
            addNewBridesmaidButton.Visibility = Visibility.Collapsed;
            addNewGroomsmanButton.Visibility = Visibility.Collapsed;

            var bridesmaidControls = spBridesmaids.Children.OfType<ContactControl>().ToList();
            foreach (var bridesmaidControl in bridesmaidControls)
            {
                bridesmaidControl.DisplayValues();
            }
            var groomsmenControls = spGroomsmen.Children.OfType<ContactControl>().ToList();
            foreach (var groomsmenControl in groomsmenControls)
            {
                groomsmenControl.DisplayValues();
            }
        }

        public override void EditValues()
        {
            selectBridesmaids.Visibility = Visibility.Visible;
            selectGroomsmen.Visibility = Visibility.Visible;
            addNewBridesmaidButton.Visibility = Visibility.Visible;
            addNewGroomsmanButton.Visibility = Visibility.Visible;

            var bridesmaidControls = spBridesmaids.Children.OfType<ContactControl>().ToList();
            foreach (var bridesmaidControl in bridesmaidControls)
            {
                bridesmaidControl.EditValues();
            }
            var groomsmenControls = spGroomsmen.Children.OfType<ContactControl>().ToList();
            foreach (var groomsmenControl in groomsmenControls)
            {
                groomsmenControl.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskCode);

            var bridesmaidContacts = spBridesmaids.Children.OfType<ContactControl>().ToList();
            writer.Write(bridesmaidContacts.Count);
            foreach (var bridesmaidContact in bridesmaidContacts)
            {
                bridesmaidContact.SerializeContact(writer);
            }

            var groomsmenContacts = spGroomsmen.Children.OfType<ContactControl>().ToList();
            writer.Write(groomsmenContacts.Count);
            foreach (var groomsmanContact in groomsmenContacts)
            {
                groomsmanContact.SerializeContact(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var bridesmaidsCount = reader.ReadInt32();

            for (int i = 0; i < bridesmaidsCount; i++)
            {
                var contactControl = new ContactControl();
                contactControl.DeserializeContact(reader);
                contactControl.OnDelete = deleteBridesmaidButton_Click;
                spBridesmaids.Children.Add(contactControl);
            }

            var groomsmenCount = reader.ReadInt32();
            for (int i = 0; i < groomsmenCount; i++)
            {
                var contactControl = new ContactControl();
                contactControl.DeserializeContact(reader);
                contactControl.OnDelete = deleteGroomsmanButton_Click;
                spGroomsmen.Children.Add(contactControl);
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskCode);
        }

        void deleteBridesmaidButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteBridesmaid(contactControl);
        }

        void deleteGroomsmanButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGroomsman(contactControl);
        }

        private async void DeleteBridesmaid(ContactControl controlToRemove)
        {
            if (controlToRemove != null)
            {
                spBridesmaids.Children.Remove(controlToRemove);
            }

            await AppData.SerializeData();
        }

        private async void DeleteGroomsman(ContactControl controlToRemove)
        {
            if (controlToRemove != null)
            {
                spGroomsmen.Children.Remove(controlToRemove);
            }

            await AppData.SerializeData();
        }

        private async void selectBridesmaids_Click(object sender, RoutedEventArgs e)
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
                if (!Bridesmaids.Any(c => c.StoredContact.Id == contact.Id))
                {
                    var contactControl = new ContactControl(contact);
                    contactControl.OnDelete = deleteBridesmaidButton_Click;

                    spBridesmaids.Children.Add(contactControl);
                }
            }
        }


        private async void selectGroomsmen_Click(object sender, RoutedEventArgs e)
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
                if (!Groomsmen.Any(c => c.StoredContact.Id == contact.Id))
                {
                    var contactControl = new ContactControl(contact);
                    contactControl.OnDelete = deleteGroomsmanButton_Click;

                    spGroomsmen.Children.Add(contactControl);
                }
            }
        }

        private void addNewBridesmaidButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = new ContactControl(true);
            contactControl.OnDelete = deleteBridesmaidButton_Click;
            spBridesmaids.Children.Add(contactControl);
        }

        private void addNewGroomsmanButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = new ContactControl(true);
            contactControl.OnDelete = deleteGroomsmanButton_Click;
            spGroomsmen.Children.Add(contactControl);
        }
    }
}
