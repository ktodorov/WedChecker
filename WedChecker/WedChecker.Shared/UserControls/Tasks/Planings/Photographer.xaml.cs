using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class Photographer : BaseTaskControl
    {
        private string FirstName = string.Empty;
        private string LastName = string.Empty;
        private string Phone = string.Empty;
        private string Email = string.Empty;
        private string Address = string.Empty;
        private string Website = string.Empty;
        private string Notes = string.Empty;

        private bool FirstStart = true;

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
                return "Select the photographer from your contacts and I'll note everything about him under or you can type it on your own";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "Here is the photographer info that you have provided";
            }
        }

        public Photographer()
        {
            this.InitializeComponent();
        }

        public Photographer(Dictionary<string, object> values)
        {
            this.InitializeComponent();

            LoadValues(values);
        }

        private void LoadValues(Dictionary<string, object> values)
        {
            if (values.ContainsKey("FirstName"))
            {
                FirstName = values["FirstName"] as string;
            }
            if (values.ContainsKey("LastName"))
            {
                LastName = values["LastName"] as string;
            }
            if (values.ContainsKey("Phone"))
            {
                Phone = values["Phone"] as string;
            }
            if (values.ContainsKey("Email"))
            {
                Email = values["Email"] as string;
            }
            if (values.ContainsKey("Address"))
            {
                Address = values["Address"] as string;
            }
            if (values.ContainsKey("Website"))
            {
                Website = values["Website"] as string;
            }
            if (values.ContainsKey("Notes"))
            {
                Notes = values["Notes"] as string;
            }
        }

        public override void DisplayValues()
        {
            selectPhotographer.Visibility = Visibility.Collapsed;
            CopyValues(false);
            ChangeDisplayVisibility(editControlsVisibility: Visibility.Collapsed, displayControlsVisibility: Visibility.Visible);
        }

        public override void EditValues()
        {
            selectPhotographer.Visibility = Visibility.Visible;
            CopyValues(true);
            ChangeDisplayVisibility(editControlsVisibility: Visibility.Visible, displayControlsVisibility: Visibility.Collapsed);
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.Photographer.ToString());

            var count = 0;
            if (!string.IsNullOrEmpty(FirstName))
            {
                count++;
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                count++;
            }
            if (!string.IsNullOrEmpty(Phone))
            {
                count++;
            }
            if (!string.IsNullOrEmpty(Email))
            {
                count++;
            }
            if (!string.IsNullOrEmpty(Address))
            {
                count++;
            }
            if (!string.IsNullOrEmpty(Website))
            {
                count++;
            }
            if (!string.IsNullOrEmpty(Notes))
            {
                count++;
            }

            writer.Write(count);

            if (!string.IsNullOrEmpty(FirstName))
            {
                writer.Write("FirstName");
                writer.Write(FirstName);
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                writer.Write("LastName");
                writer.Write(LastName);
            }
            if (!string.IsNullOrEmpty(Phone))
            {
                writer.Write("Phone");
                writer.Write(Phone);
            }
            if (!string.IsNullOrEmpty(Email))
            {
                writer.Write("Email");
                writer.Write(Email);
            }
            if (!string.IsNullOrEmpty(Address))
            {
                writer.Write("Address");
                writer.Write(Address);
            }
            if (!string.IsNullOrEmpty(Website))
            {
                writer.Write("Website");
                writer.Write(Website);
            }
            if (!string.IsNullOrEmpty(Notes))
            {
                writer.Write("Notes");
                writer.Write(Notes);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var records = reader.ReadInt32();

            string firstName = string.Empty, lastName = string.Empty, phone = string.Empty, email = string.Empty,
                    address = string.Empty, website = string.Empty, notes = string.Empty;


            for (int i = 0; i < records; i++)
            {
                var type = reader.ReadString();

                if (type == "FirstName")
                {
                    firstName = reader.ReadString();
                }
                if (type == "LastName")
                {
                    lastName = reader.ReadString();
                }
                if (type == "Phone")
                {
                    phone = reader.ReadString();
                }
                if (type == "Email")
                {
                    email = reader.ReadString();
                }
                if (type == "Address")
                {
                    address = reader.ReadString();
                }
                if (type == "Website")
                {
                    website = reader.ReadString();
                }
                if (type == "Notes")
                {
                    notes = reader.ReadString();
                }
            }
            PopulateFields(firstName, lastName, phone, email, address, website, notes);
            DisplayValues();
            FirstStart = false;
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.Photographer.ToString());
            FirstStart = false;
        }

        private async void selectPhotographer_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contact = await picker.PickContactAsync();

            if (contact == null)
            {
                return;
            }

            PopulateFieldsFromContact(contact);
        }

        private void PopulateFieldsFromContact(Contact contact, string notes = null)
        {
            if (contact == null)
            {
                return;
            }

            var firstName = contact.FirstName;
            var lastName = contact.LastName;
            var phone = string.Empty;
            if (contact.Phones.Any())
            {
                phone = contact.Phones.FirstOrDefault().Number;
            }

            var email = string.Empty;
            if (contact.Emails.Any())
            {
                email = contact.Emails.FirstOrDefault().Address;
            }

            var address = string.Empty;
            if (contact.Addresses.Any())
            {
                var firstAddress = contact.Addresses.FirstOrDefault();
                address = firstAddress.Region;
                if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(firstAddress.StreetAddress))
                {
                    address += ", ";
                }
                if (!string.IsNullOrEmpty(firstAddress.StreetAddress))
                {
                    address += ", " + firstAddress.StreetAddress;
                }
            }
            var website = contact.Websites.FirstOrDefault()?.Uri.AbsolutePath;

            PopulateFields(firstName, lastName, phone, email, address, website, notes);
        }

        private void PopulateFields(string firstName = null, string lastName = null, string phone = null, string email = null,
                                    string address = null, string website = null, string notes = null)
        {
            if (!string.IsNullOrEmpty(firstName))
            {
                FirstName = firstName;
                tbFirstName.Text = FirstName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                LastName = lastName;
                tbLastName.Text = LastName;
            }

            if (!string.IsNullOrEmpty(phone))
            {
                Phone = phone;
                tbPhone.Text = Phone;
            }

            if (!string.IsNullOrEmpty(email))
            {
                Email = email;
                tbEmail.Text = Email;
            }

            if (!string.IsNullOrEmpty(address))
            {
                Address = address;
                tbAddress.Text = Address;
            }

            if (!string.IsNullOrEmpty(website))
            {
                Website = website;
                tbWebsite.Text = Website;
            }

            if (!string.IsNullOrEmpty(notes))
            {
                Notes = notes;
                tbNotes.Text = Notes;
            }
        }

        private void ChangeDisplayVisibility(Visibility editControlsVisibility, Visibility displayControlsVisibility)
        {
            tbAddress.Visibility = editControlsVisibility;
            tbFirstName.Visibility = editControlsVisibility;
            tbLastName.Visibility = editControlsVisibility;
            tbNotes.Visibility = editControlsVisibility;
            tbWebsite.Visibility = editControlsVisibility;
            tbPhone.Visibility = editControlsVisibility;
            tbEmail.Visibility = editControlsVisibility;

            tBlockAddress.Visibility = editControlsVisibility;
            tBlockFirstName.Visibility = editControlsVisibility;
            tBlockFirstName.Visibility = editControlsVisibility;
            tBlockNotes.Visibility = editControlsVisibility;
            tBlockWebsite.Visibility = editControlsVisibility;
            tBlockPhone.Visibility = editControlsVisibility;
            tBlockEmail.Visibility = editControlsVisibility;


            var shouldCheckDisplayBlocks = !FirstStart && (displayControlsVisibility == Visibility.Visible);

            // If we are displaying, then we should check if there are values submitted
            // for the current text block and if there aren't, then we don't display it
            if (!string.IsNullOrEmpty(tbAddressDisplay.Text))
            {
                tbAddressDisplay.Visibility = displayControlsVisibility;
                tBlockAddress.Visibility = Visibility.Visible;
            }
            else if (shouldCheckDisplayBlocks)
            {
                tbAddressDisplay.Visibility = Visibility.Collapsed;
                tBlockAddress.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(tbFirstNameDisplay.Text))
            {
                tbFirstNameDisplay.Visibility = displayControlsVisibility;
                tBlockFirstName.Visibility = Visibility.Visible;
            }
            else if (shouldCheckDisplayBlocks)
            {
                tbFirstNameDisplay.Visibility = Visibility.Collapsed;
                tBlockFirstName.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(tbLastNameDisplay.Text))
            {
                tbLastNameDisplay.Visibility = displayControlsVisibility;
                tBlockLastName.Visibility = Visibility.Visible;
            }
            else if (shouldCheckDisplayBlocks)
            {
                tbLastNameDisplay.Visibility = Visibility.Collapsed;
                tBlockLastName.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(tbNotesDisplay.Text))
            {
                tbNotesDisplay.Visibility = displayControlsVisibility;
                tBlockNotes.Visibility = Visibility.Visible;
            }
            else if (shouldCheckDisplayBlocks)
            {
                tbNotesDisplay.Visibility = Visibility.Collapsed;
                tBlockNotes.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(tbWebsiteDisplay.Text))
            {
                tbWebsiteDisplay.Visibility = displayControlsVisibility;
                tBlockWebsite.Visibility = Visibility.Visible;
            }
            else if (shouldCheckDisplayBlocks)
            {
                tbWebsiteDisplay.Visibility = Visibility.Collapsed;
                tBlockWebsite.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(tbPhoneDisplay.Text))
            {
                tbPhoneDisplay.Visibility = displayControlsVisibility;
                tBlockPhone.Visibility = Visibility.Visible;
            }
            else if (shouldCheckDisplayBlocks)
            {
                tbPhoneDisplay.Visibility = Visibility.Collapsed;
                tBlockPhone.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(tbEmailDisplay.Text))
            {
                tbEmailDisplay.Visibility = displayControlsVisibility;
                tBlockEmail.Visibility = Visibility.Visible;
            }
            else if (shouldCheckDisplayBlocks)
            {
                tbEmailDisplay.Visibility = Visibility.Collapsed;
                tBlockEmail.Visibility = Visibility.Collapsed;
            }

        }

        private void CopyValues(bool displayToEdit)
        {
            if (displayToEdit)
            {
                tbAddress.Text = Address;
                tbFirstName.Text = FirstName;
                tbLastName.Text = LastName;
                tbNotes.Text = Notes;
                tbWebsite.Text = Website;
                tbPhone.Text = Phone;
                tbEmail.Text = Email;
            }
            else
            {
                tbAddressDisplay.Text = Address;
                tbFirstNameDisplay.Text = FirstName;
                tbLastNameDisplay.Text = LastName;
                tbNotesDisplay.Text = Notes;
                tbWebsiteDisplay.Text = Website;
                tbPhoneDisplay.Text = Phone;
                tbEmailDisplay.Text = Email;
            }
        }

        private void tbFirstName_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            FirstName = tbFirstName.Text;
        }

        private void tbLastName_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            LastName = tbLastName.Text;
        }

        private void tbPhone_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            Phone = tbPhone.Text;
        }

        private void tbEmail_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            Email = tbEmail.Text;
        }

        private void tbAddress_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            Address = tbAddress.Text;
        }

        private void tbWebsite_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            Website = tbWebsite.Text;
        }

        private void tbNotes_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            Notes = tbNotes.Text;
        }
    }
}
