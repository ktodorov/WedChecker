using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public static new string DisplayHeader
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
            var bridesmaidContacts = spBridesmaids.Children.OfType<ContactControl>().ToList();
            writer.Write(bridesmaidContacts.Count);
            foreach (var bridesmaidContact in bridesmaidContacts)
            {
                bridesmaidContact.Serialize(writer);
            }

            var groomsmenContacts = spGroomsmen.Children.OfType<ContactControl>().ToList();
            writer.Write(groomsmenContacts.Count);
            foreach (var groomsmanContact in groomsmenContacts)
            {
                groomsmanContact.Serialize(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var bridesmaidsCount = reader.ReadInt32();

            for (int i = 0; i < bridesmaidsCount; i++)
            {
                var contactControl = new ContactControl();
                contactControl.Deserialize(reader);
                contactControl.OnDelete = deleteBridesmaidButton_Click;
                spBridesmaids.Children.Add(contactControl);
            }

            var groomsmenCount = reader.ReadInt32();
            for (int i = 0; i < groomsmenCount; i++)
            {
                var contactControl = new ContactControl();
                contactControl.Deserialize(reader);
                contactControl.OnDelete = deleteGroomsmanButton_Click;
                spGroomsmen.Children.Add(contactControl);
            }
        }

        void deleteBridesmaidButton_Click(object sender, RoutedEventArgs e)
        {
			var contactControl = (sender as Button).FindAncestorByType<ContactControl>();
            if (contactControl != null)
            {
                DeleteBridesmaid(contactControl);
            }
        }

        void deleteGroomsmanButton_Click(object sender, RoutedEventArgs e)
        {
			var contactControl = (sender as Button).FindAncestorByType<ContactControl>();
            if (contactControl != null)
            {
                DeleteGroomsman(contactControl);
            }
        }

        private void DeleteBridesmaid(ContactControl controlToRemove)
        {
            if (controlToRemove != null)
            {
                spBridesmaids.Children.Remove(controlToRemove);
            }
        }

        private void DeleteGroomsman(ContactControl controlToRemove)
        {
            if (controlToRemove != null)
            {
                spGroomsmen.Children.Remove(controlToRemove);
            }
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

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            if (Groomsmen != null && Groomsmen.Any())
            {
                sb.AppendLine("Groomsmen:");
                foreach (var groomsman in Groomsmen)
                {
                    var groomsmanAsText = groomsman.GetDataAsText();
                    sb.AppendLine(groomsmanAsText);
                }
            }
            if (Bridesmaids != null && Bridesmaids.Any())
            {
                sb.AppendLine("Bridesmaids:");
                foreach (var bridesmaid in Bridesmaids)
                {
                    var bridesmaidAsText = bridesmaid.GetDataAsText();
                    sb.AppendLine(bridesmaidAsText);
                }
            }
        }
    }
}
