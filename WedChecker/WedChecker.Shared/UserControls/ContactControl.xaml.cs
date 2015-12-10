using System;
using System.IO;
using System.Linq;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

            private set
            {
                _storedContact = value;
            }
        }

        public RoutedEventHandler OnDelete
        {
            set
            {
                deleteButton.Click += value;
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

        private bool IsEditable = false;
        private bool EditAlongWith = true;

        public ContactControl()
        {
            this.InitializeComponent();

            SetBackground();
        }

        public ContactControl(bool isEditable)
        {
            this.InitializeComponent();

            IsEditable = isEditable;

            if (IsEditable)
            {
                EditValues();
            }

            SetBackground();
        }

        public ContactControl(Contact storedContact, string alongWith = null, bool editAlongWith = true, bool isEditable = false)
        {
            this.InitializeComponent();

            IsEditable = isEditable;
            EditAlongWith = editAlongWith;

            StoreContact(storedContact);

            int parsedAlongWith;
            if (int.TryParse(alongWith, out parsedAlongWith))
            {
                tbAlongWith.Text = alongWith;
                StoredContact.Notes = alongWith;
            }

            SetBackground();
        }

        public void StoreContact(Contact contact)
        {
            ClearAllFields();

            if (IsEditable && string.IsNullOrEmpty(contact.Id))
            {
                contact.Id = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(contact.Id))
            {
                return;
            }

            _storedContact = contact;

            tbId.Text = StoredContact.Id;

            if (!string.IsNullOrEmpty(StoredContact.FirstName) || !string.IsNullOrEmpty(StoredContact.LastName))
            {
                tbContactName.Text = $"{StoredContact.FirstName} {StoredContact.LastName}";
                tbEditContactName.Text = $"{StoredContact.FirstName} {StoredContact.LastName}";
            }
            else
            {
                tbContactName.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(StoredContact.Notes))
            {
                tbAlongWith.Text = StoredContact.Notes;
                tbCheckboxTextDisplay.Text = StoredContact.Notes;
            }

            if (StoredContact.Emails.Count > 0)
            {
                var emails = string.Empty;

                foreach (var email in StoredContact.Emails)
                {
                    emails += $"{email.Address}{Environment.NewLine}";
                }

                tbContactEmails.Text = emails;
                tbEditContactEmails.Text = emails;
            }
            else
            {
                emailsPanel.Visibility = Visibility.Collapsed;
            }

            if (StoredContact.Phones.Count > 0)
            {
                var phones = string.Empty;

                foreach (var phone in StoredContact.Phones)
                {
                    phones += $"{phone.Number}{Environment.NewLine}";
                }

                tbContactPhones.Text = phones;
                tbEditContactPhones.Text = phones;
            }
            else
            {
                phonesPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearAllFields()
        {
            tbId.Text = string.Empty;
            tbEditContactName.Text = string.Empty;
            tbContactName.Text = string.Empty;
            tbContactEmails.Text = string.Empty;
            tbEditContactEmails.Text = string.Empty;
            tbContactPhones.Text = string.Empty;
            tbEditContactPhones.Text = string.Empty;
            tbCheckboxTextDisplay.Text = string.Empty;
            tbAlongWith.Text = string.Empty;
        }

        public void DisplayValues()
        {
            AdjustVisibility();

            tbContactNameDisplay.Visibility = Visibility.Collapsed;
            tbEditContactEmails.Visibility = Visibility.Collapsed;
            tbEditContactName.Visibility = Visibility.Collapsed;
            tbEditContactPhones.Visibility = Visibility.Collapsed;

            tbContactEmails.Visibility = Visibility.Visible;
            tbContactName.Visibility = Visibility.Visible;
            tbContactPhones.Visibility = Visibility.Visible;

            deleteButton.Visibility = Visibility.Collapsed;
            tbCheckboxText.Visibility = Visibility.Collapsed;
            tbAlongWith.Visibility = Visibility.Collapsed;

            selectContactButton.Visibility = Visibility.Collapsed;

            var alongWith = 0;
            if (int.TryParse(tbAlongWith.Text, out alongWith))
            {
                tbCheckboxTextDisplay.Visibility = Visibility.Visible;
                tbCheckboxTextDisplay.Text = $"Along with {alongWith.ToString()} other";
            }
        }
        public void EditValues()
        {
            AdjustVisibility(true);

            if (IsEditable)
            {
                tbContactNameDisplay.Visibility = Visibility.Visible;
                tbEditContactEmails.Visibility = Visibility.Visible;
                tbEditContactName.Visibility = Visibility.Visible;
                tbEditContactPhones.Visibility = Visibility.Visible;

                selectContactButton.Visibility = Visibility.Visible;

                tbContactEmails.Visibility = Visibility.Collapsed;
                tbContactName.Visibility = Visibility.Collapsed;
                tbContactPhones.Visibility = Visibility.Collapsed;
            }

            deleteButton.Visibility = Visibility.Visible;

            if (EditAlongWith)
            {
                tbCheckboxText.Visibility = Visibility.Visible;
                tbAlongWith.Visibility = Visibility.Visible;
                tbCheckboxTextDisplay.Visibility = Visibility.Collapsed;
            }
        }

        private void AdjustVisibility(bool setAllVisible = false)
        {
            if (!string.IsNullOrEmpty(StoredContact.FirstName) ||
                !string.IsNullOrEmpty(StoredContact.LastName) ||
                setAllVisible)
            {
                tbContactName.Visibility = Visibility.Visible;
            }
            else
            {
                tbContactName.Visibility = Visibility.Collapsed;
            }

            if (StoredContact.Emails.Count > 0 || setAllVisible)
            {
                emailsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                emailsPanel.Visibility = Visibility.Collapsed;
            }

            if (StoredContact.Phones.Count > 0 || setAllVisible)
            {
                phonesPanel.Visibility = Visibility.Visible;
            }
            else
            {
                phonesPanel.Visibility = Visibility.Collapsed;
            }
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
            if (IsEditable)
            {
                SaveFields();
            }

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
                writer.Write("Phones");
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
                    case "Emails":
                        var emailsCount = reader.ReadInt32();
                        for (var j = 0; j < emailsCount; j++)
                        {
                            var email = new ContactEmail();
                            email.Address = reader.ReadString();
                            contact.Emails.Add(email);
                        }
                        break;
                    case "Phones":
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

        private void SaveFields()
        {
            // Id
            if (string.IsNullOrEmpty(StoredContact.Id))
            {
                StoredContact.Id = Guid.NewGuid().ToString();
            }

            // Name
            tbContactName.Text = tbEditContactName.Text;            
            var names = tbEditContactName.Text.Split(' ');
            var lastName = names.LastOrDefault();
            var firstName = string.Empty;

            for (int i = 0; i < names.Length - 1; i++)
            {
                firstName += names[i];
            }

            StoredContact.FirstName = firstName;
            StoredContact.LastName = lastName;

            // Emails
            tbContactEmails.Text = tbEditContactEmails.Text;
            StoredContact.Emails.Clear();

            if (!string.IsNullOrEmpty(tbEditContactEmails.Text))
            {
                var email = new ContactEmail();
                email.Address = tbEditContactEmails.Text;

                StoredContact.Emails.Add(email);
            }

            // Phones
            tbContactPhones.Text = tbEditContactPhones.Text;
            StoredContact.Phones.Clear();

            if (!string.IsNullOrEmpty(tbEditContactPhones.Text))
            {
                var phone = new ContactPhone();
                phone.Number = tbEditContactPhones.Text;

                StoredContact.Phones.Add(phone);
            }
            
            // Notes
            StoredContact.Notes = tbAlongWith.Text;
            tbCheckboxTextDisplay.Text = tbAlongWith.Text;
        }

        private void SetBackground()
        {
            var accentBrushColor = Core.GetPhoneAccentBrush();

            accentBrushColor.A = 30;
            var colorBrush = new SolidColorBrush(accentBrushColor);
            mainBorder.Background = colorBrush;

            accentBrushColor.A = 70;
            colorBrush = new SolidColorBrush(accentBrushColor);
            mainBorder.BorderBrush = colorBrush;
        }

        private async void selectContactButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contact = await picker.PickContactAsync();

            if (contact == null)
            {
                return;
            }

            StoreContact(contact);
        }
    }
}
