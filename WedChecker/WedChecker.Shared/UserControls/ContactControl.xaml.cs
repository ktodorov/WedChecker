using System;
using System.IO;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class ContactControl : UserControl
    {
        private Contact _storedContact;
        public Contact StoredContact
        {
            get
            {
                if (_storedContact == null)
                {
                    _storedContact = new Contact();
                }

                return _storedContact;
            }
        }
        public bool IsStored
        {
            get
            {
                if (StoredContact != null && StoredContact.Id != null)
                {
                    return true;
                }

                return false;
            }
        }

        private bool EditAlongWith = true;

        public ContactControl()
        {
            this.InitializeComponent();
        }

        public ContactControl(Contact storedContact, string alongWith = null, bool editAlongWith = true)
        {
            this.InitializeComponent();

            StoreContact(storedContact);

            int parsedAlongWith;
            if (int.TryParse(alongWith, out parsedAlongWith))
            {
                tbAlongWith.Text = alongWith;
                StoredContact.Notes = alongWith;
            }

            EditAlongWith = editAlongWith;
        }

        public void StoreContact(Contact contact)
        {
            if (string.IsNullOrEmpty(contact.Id))
            {
                return;
            }

            _storedContact = contact;

            tbId.Text = StoredContact.Id;

            if (!string.IsNullOrEmpty(StoredContact.FirstName) || !string.IsNullOrEmpty(StoredContact.LastName))
            {
                tbContactName.Visibility = Visibility.Visible;
                tbContactName.Text = $"{StoredContact.FirstName} {StoredContact.LastName}";
            }
            else
            {
                tbContactName.Visibility = Visibility.Collapsed;
            }

            //if (!string.IsNullOrEmpty(StoredContact.Notes))
            //{
            //    tbContactName.Text = $"{StoredContact.FirstName} {StoredContact.LastName}";
            //}

            if (StoredContact.Emails.Count > 0)
            {
                emailsPanel.Visibility = Visibility.Visible;
                var emails = string.Empty;

                foreach (var email in StoredContact.Emails)
                {
                    emails += $"{email.Address};{Environment.NewLine}";
                }

                tbContactEmails.Text = emails;
            }
            else
            {
                emailsPanel.Visibility = Visibility.Collapsed;
            }

            if (StoredContact.Phones.Count > 0)
            {
                phonesPanel.Visibility = Visibility.Visible;
                var phones = string.Empty;

                foreach (var phone in StoredContact.Phones)
                {
                    phones += $"{phone.Number};{Environment.NewLine}";
                }

                tbContactPhones.Text = phones;
            }
            else
            {
                phonesPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void DisplayValues()
        {
            deleteButton.Visibility = Visibility.Collapsed;
            tbCheckboxText.Visibility = Visibility.Collapsed;
            tbAlongWith.Visibility = Visibility.Collapsed;

            var alongWith = 0;
            if (int.TryParse(tbAlongWith.Text, out alongWith))
            {
                tbCheckboxTextDisplay.Visibility = Visibility.Visible;
                tbCheckboxTextDisplay.Text = $"Along with {alongWith.ToString()} other";
            }
        }
        public void EditValues()
        {
            deleteButton.Visibility = Visibility.Visible;

            if (EditAlongWith)
            {
                tbCheckboxText.Visibility = Visibility.Visible;
                tbAlongWith.Visibility = Visibility.Visible;
                tbCheckboxTextDisplay.Visibility = Visibility.Collapsed;
            }
        }

        private void tbAlongWith_TextChanged(object sender, TextChangedEventArgs e)
        {
            StoredContact.Notes = tbAlongWith.Text;
        }

        private int GetFilledFields()
        {
            if (!IsStored)
            {
                return 0;
            }

            var result = 0;

            if (StoredContact.Id != null) result++;
            if (StoredContact.FirstName != null) result++;
            if (StoredContact.LastName != null) result++;
            if (StoredContact.Notes != null) result++;
            if (StoredContact.Emails.Count > 0) result++;
            if (StoredContact.Phones.Count > 0) result++;

            return result;
        }

        public void SerializeContact(BinaryWriter writer)
        {
            var filledFields = GetFilledFields();

            if (filledFields == 0)
            {
                return;
            }
            else
            {
                writer.Write(filledFields);
            }

            if (StoredContact.Id != null)
            {
                writer.Write("Id");
                writer.Write(StoredContact.Id);
            }
            if (StoredContact.FirstName != null)
            {
                writer.Write("FirstName");
                writer.Write(StoredContact.FirstName);
            }
            if (StoredContact.LastName != null)
            {
                writer.Write("LastName");
                writer.Write(StoredContact.LastName);
            }
            if (StoredContact.Notes != null)
            {
                writer.Write("Notes");
                writer.Write(StoredContact.Notes);
            }
            if (StoredContact.Emails.Count > 0)
            {
                writer.Write("Emails");
                writer.Write(StoredContact.Emails.Count);
                foreach (var email in StoredContact.Emails)
                {
                    writer.Write(email.Address);
                }
            }
            if (StoredContact.Phones.Count > 0)
            {
                writer.Write("Phone");
                writer.Write(StoredContact.Phones.Count);
                foreach (var phone in StoredContact.Phones)
                {
                    writer.Write(phone.Number);
                }
            }
        }

        public void DeserializeContact(BinaryReader reader)
        {
            var contact = new Contact();

            var fieldsToCollect = reader.ReadInt32();

            if (fieldsToCollect <= 0)
            {
                return;
            }

            var readString = string.Empty;
            for (var i = 0; i < fieldsToCollect; i++)
            {
                readString = reader.ReadString();

                switch (readString)
                {
                    case "Id":
                        contact.Id = reader.ReadString();
                        break;
                    case "FirstName":
                        contact.FirstName = reader.ReadString();
                        break;
                    case "LastName":
                        contact.LastName = reader.ReadString();
                        break;
                    case "Notes":
                        contact.Notes = reader.ReadString();
                        break;
                    case "Email":
                        var emailsCount = reader.ReadInt32();
                        for (var j = 0; j < emailsCount; j++)
                        {
                            var email = new ContactEmail();
                            email.Address = reader.ReadString();
                            contact.Emails.Add(email);
                        }
                        break;
                    case "Phone":
                        var phonesCount = reader.ReadInt32();
                        for (var j = 0; j < phonesCount; j++)
                        {
                            var phone = new ContactPhone();
                            phone.Number = reader.ReadString();
                            contact.Phones.Add(phone);
                        }
                        break;
                }
            }

            StoreContact(contact);
        }
    }
}
