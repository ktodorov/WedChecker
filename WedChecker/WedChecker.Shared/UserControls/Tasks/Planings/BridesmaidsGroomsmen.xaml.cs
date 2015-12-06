using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
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

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class BridesmaidsGroomsmen : BaseTaskControl
    {

        private List<Contact> Bridesmaids = new List<Contact>();
        private List<Contact> Groomsmen = new List<Contact>();


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
            var bridesmaidControls = spBridesmaids.Children.OfType<ContactControl>();
            foreach (var bridesmaidControl in bridesmaidControls)
            {
                bridesmaidControl.DisplayValues();
            }
            var groomsmenControls = spGroomsmen.Children.OfType<ContactControl>();
            foreach (var groomsmenControl in groomsmenControls)
            {
                groomsmenControl.DisplayValues();
            }
        }

        public override void EditValues()
        {
            selectBridesmaids.Visibility = Visibility.Visible;
            selectGroomsmen.Visibility = Visibility.Visible;
            var bridesmaidControls = spBridesmaids.Children.OfType<ContactControl>();
            foreach (var bridesmaidControl in bridesmaidControls)
            {
                bridesmaidControl.EditValues();
            }
            var groomsmenControls = spGroomsmen.Children.OfType<ContactControl>();
            foreach (var groomsmenControl in groomsmenControls)
            {
                groomsmenControl.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.BridesmaidsGroomsmen.ToString());

            writer.Write(Bridesmaids.Count);
            foreach (var bridesmaid in Bridesmaids)
            {
                writer.Write(bridesmaid.Id);
                writer.Write(bridesmaid.FirstName);
                writer.Write(bridesmaid.LastName);
            }

            writer.Write(Groomsmen.Count);
            foreach (var groomsman in Groomsmen)
            {
                writer.Write(groomsman.Id);
                writer.Write(groomsman.FirstName);
                writer.Write(groomsman.LastName);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var bridesmaidsCount = reader.ReadInt32();

            for (int i = 0; i < bridesmaidsCount; i++)
            {
                var contact = new Contact();
                contact.Id = reader.ReadString();
                contact.FirstName = reader.ReadString();
                contact.LastName = reader.ReadString();
                Bridesmaids.Add(contact);

                var contactControl = new ContactControl(contact);
                contactControl.OnDelete = deleteBridesmaidButton_Click;
                contactControl.Visibility = Visibility.Visible;
                spBridesmaids.Children.Add(contactControl);
            }

            var groomsmenCount = reader.ReadInt32();
            for (int i = 0; i < groomsmenCount; i++)
            {
                var contact = new Contact();
                contact.Id = reader.ReadString();
                contact.FirstName = reader.ReadString();
                contact.LastName = reader.ReadString();
                Groomsmen.Add(contact);

                var contactControl = new ContactControl(contact);
                contactControl.OnDelete = deleteGroomsmanButton_Click;
                contactControl.Visibility = Visibility.Visible;
                spGroomsmen.Children.Add(contactControl);
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.BridesmaidsGroomsmen.ToString());
        }

        void deleteBridesmaidButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteBridesmaid(contactControl.StoredContact.Id);
        }

        void deleteGroomsmanButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGroomsman(contactControl.StoredContact.Id);
        }

        private async void DeleteBridesmaid(string id)
        {
            var bridesmaidToRemove = Bridesmaids.FirstOrDefault(g => g.Id == id);

            if (bridesmaidToRemove != null)
            {
                Bridesmaids.Remove(bridesmaidToRemove);
            }

            var controlToRemove = spBridesmaids.Children.OfType<ContactControl>().FirstOrDefault(c => c.StoredContact.Id == bridesmaidToRemove.Id);
            if (controlToRemove != null)
            {
                spBridesmaids.Children.Remove(controlToRemove);
            }

            await AppData.SerializeData();
        }

        private async void DeleteGroomsman(string id)
        {
            var groomsmanToRemove = Groomsmen.FirstOrDefault(g => g.Id == id);

            if (groomsmanToRemove != null)
            {
                Groomsmen.Remove(groomsmanToRemove);
            }

            var controlToRemove = spGroomsmen.Children.OfType<ContactControl>().FirstOrDefault(c => c.StoredContact.Id == groomsmanToRemove.Id);
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

            if (!contacts.Any())
            {
                return;
            }

            foreach (var contact in contacts)
            {
                if (!Bridesmaids.Any(c => c.Id == contact.Id))
                {
                    Bridesmaids.Add(contact);
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

            if (!contacts.Any())
            {
                return;
            }

            foreach (var contact in contacts)
            {
                if (!Groomsmen.Any(c => c.Id == contact.Id))
                {
                    Groomsmen.Add(contact);
                    var contactControl = new ContactControl(contact);
                    contactControl.OnDelete = deleteGroomsmanButton_Click;

                    spGroomsmen.Children.Add(contactControl);
                }
            }
        }
    }
}
