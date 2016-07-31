using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Infrastructure;
using WedChecker.Interfaces;
using Windows.ApplicationModel.Contacts;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
	public sealed partial class ContactControl : UserControl, IStorableTask, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

		private WedCheckerContact _storedContact;
		public WedCheckerContact StoredContact
		{
			get
			{
				if (_storedContact == null)
				{
					_storedContact = new WedCheckerContact();
				}

				return _storedContact;
			}

			private set
			{
				_storedContact = value;
                this.DataContext = StoredContact;
                NotifyPropertyChanged("StoredContact");
            }
        }

        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
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

		private bool _isEditable;
		public bool IsEditable
		{
			get
			{
				return _isEditable;
			}
			set
			{
				_isEditable = value;
			}
		}

		private bool _isReplaceable;
		public bool IsReplaceable
		{
			get
			{
				return _isReplaceable;
			}
			set
			{
				_isReplaceable = value;
			}
		}

		private bool _editAlongWith;
		public bool EditAlongWith
		{
			get
			{
				return _editAlongWith;
			}
			set
			{
				_editAlongWith = value;
			}
		}

		private bool _showAlongWithPanel = true;
		public bool ShowAlongWithPanel
		{
			get
			{
				return _showAlongWithPanel;
			}
			set
			{
				_showAlongWithPanel = value;
			}
		}

		public string Header
		{
			get
			{
				return tbHeader.Text;
			}
			set
			{
				if (tbHeader.Visibility == Visibility.Collapsed)
				{
					tbHeader.Visibility = Visibility.Visible;
				}

				tbHeader.Text = value;
			}
		}

		public string RemoveButtonContent
		{
			get
			{
				return deleteButton.Content as string;
			}
			set
			{
				deleteButton.Content = value;
			}
		}


		private bool _clearOnRemove;
		public bool ClearOnRemove
		{
			get
			{
				return _clearOnRemove;
			}
			set
			{
				_clearOnRemove = value;
				if (_clearOnRemove)
				{
					deleteButton.Content = "Clear";
					deleteButton.Click += DeleteButton_Click;
				}
				else
				{
					deleteButton.Content = "Remove";
					deleteButton.Click -= DeleteButton_Click;
				}
			}
		}

		public bool IncludeName
		{
			get
			{
				return namePanel.Visibility == Visibility.Visible;
			}
			set
			{
				namePanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public bool IncludeEmail
		{
			get
			{
				return emailsPanel.Visibility == Visibility.Visible;
			}
			set
			{
				emailsPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public bool IncludePhones
		{
			get
			{
				return phonesPanel.Visibility == Visibility.Visible;
			}
			set
			{
				phonesPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public bool IncludeAlongWith
		{
			get
			{
				return alongWithPanel.Visibility == Visibility.Visible;
			}
			set
			{
				alongWithPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public ContactControl()
		{
			this.InitializeComponent();

			IsEditable = false;
			EditAlongWith = false;
			IsReplaceable = false;

			SetBackground();
		}

		public ContactControl(bool isEditable = false, bool editAlongWith = false, bool isReplaceable = false)
		{
			this.InitializeComponent();

			IsEditable = isEditable;
			EditAlongWith = editAlongWith;
			IsReplaceable = isReplaceable;

			SetBackground();
		}

		public ContactControl(Contact contact, string alongWith = null, bool editAlongWith = false, bool isEditable = false, bool isReplaceable = false)
		{
			this.InitializeComponent();
            IsEditable = isEditable;
			EditAlongWith = editAlongWith;
			IsReplaceable = isReplaceable;

			SetBackground();

            StoreContact(contact);

            EditValues();
		}

		public void ClearContact()
		{
			StoredContact = null;

			EditValues();
		}

		public void DisplayValues()
        {
            phonesPanel.Visibility = Visibility.Visible;
            emailsPanel.Visibility = Visibility.Visible;

            tbContactNameDisplay.Visibility = Visibility.Collapsed;
            tbEditContactEmails.Visibility = Visibility.Collapsed;
            tbEditContactName.Visibility = Visibility.Collapsed;
            tbEditContactPhones.Visibility = Visibility.Collapsed;

            if (StoredContact.Emails.Any())
            {
                tbContactEmails.Visibility = Visibility.Visible;
            }
            else
            {
                emailsPanel.Visibility = Visibility.Collapsed;
            }

            if (StoredContact.Phones.Any())
            {
                tbContactPhones.Visibility = Visibility.Visible;
            }
            else
            {
                phonesPanel.Visibility = Visibility.Collapsed;
            }
            
            if (!string.IsNullOrEmpty(StoredContact.FullName))
            {
                tbContactName.Visibility = Visibility.Visible;
            }

            deleteButton.Visibility = Visibility.Collapsed;
            tbAlongWithText.Visibility = Visibility.Collapsed;
            tbAlongWith.Visibility = Visibility.Collapsed;

            if (!string.IsNullOrEmpty(StoredContact.Notes))
            {
                tbAlongWithDisplay.Visibility = Visibility.Visible;
            }

            selectContactButton.Visibility = Visibility.Collapsed;
        }

		public void EditValues()
		{
            phonesPanel.Visibility = Visibility.Visible;
            emailsPanel.Visibility = Visibility.Visible;

            if (IsReplaceable)
            {
                selectContactButton.Visibility = Visibility.Visible;
            }

            if (IsEditable)
            {
                tbContactNameDisplay.Visibility = Visibility.Visible;
                tbEditContactEmails.Visibility = Visibility.Visible;
                tbEditContactName.Visibility = Visibility.Visible;
                tbEditContactPhones.Visibility = Visibility.Visible;

                tbContactEmails.Visibility = Visibility.Collapsed;
                tbContactName.Visibility = Visibility.Collapsed;
                tbContactPhones.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbContactNameDisplay.Visibility = Visibility.Collapsed;
                tbEditContactEmails.Visibility = Visibility.Collapsed;
                tbEditContactName.Visibility = Visibility.Collapsed;
                tbEditContactPhones.Visibility = Visibility.Collapsed;

                tbContactName.Visibility = Visibility.Visible;

                if (!StoredContact.Emails.Any())
                {
                    emailsPanel.Visibility = Visibility.Collapsed;
                }

                if (!StoredContact.Phones.Any())
                {
                    phonesPanel.Visibility = Visibility.Collapsed;
                }
            }

            tbAlongWithDisplay.Visibility = Visibility.Collapsed;
            if (EditAlongWith)
            {
                tbAlongWithText.Visibility = Visibility.Visible;
                tbAlongWith.Visibility = Visibility.Visible;
            }
            else
            {
                tbAlongWithText.Visibility = Visibility.Collapsed;
                tbAlongWith.Visibility = Visibility.Collapsed;
                if (!IsEditable)
                {
                    tbAlongWithDisplay.Visibility = Visibility.Collapsed;
                }
            }

            if (string.IsNullOrEmpty(StoredContact.FullName))
            {
                return;
            }

            deleteButton.Visibility = Visibility.Visible;
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
			if (StoredContact.Notes != null && ShowAlongWithPanel) result++;
			if (StoredContact.Emails.Count > 0) result++;
			if (StoredContact.Phones.Count > 0) result++;

			return result;
		}

		public void Serialize(BinaryWriter writer)
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
			if (StoredContact.Notes != null && ShowAlongWithPanel)
			{
				writer.Write("Notes");
				writer.Write(StoredContact.Notes);
			}
			if (StoredContact.Emails.Any())
			{
				writer.Write("Emails");
				writer.Write(StoredContact.Emails.Count);
				foreach (var email in StoredContact.Emails)
				{
					writer.Write(email.Address);
				}
			}
			if (StoredContact.Phones.Any())
			{
				writer.Write("Phones");
				writer.Write(StoredContact.Phones.Count);
				foreach (var phone in StoredContact.Phones)
				{
					writer.Write(phone.Number);
				}
			}
		}

		public void Deserialize(BinaryReader reader)
		{
			StoredContact = new WedCheckerContact();

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
						StoredContact.Id = reader.ReadString();
						break;
					case "FirstName":
						StoredContact.FirstName = reader.ReadString();
						break;
					case "LastName":
						StoredContact.LastName = reader.ReadString();
						break;
					case "Notes":
						StoredContact.Notes = reader.ReadString();
						break;
					case "Emails":
						var emailsCount = reader.ReadInt32();
						for (var j = 0; j < emailsCount; j++)
						{
							var email = new ContactEmail();
							email.Address = reader.ReadString();
							StoredContact.Emails.Add(email);
                            StoredContact.Email = email;
                            StoredContact.EmailAddress = email.Address;
                        }
						break;
					case "Phones":
						var phonesCount = reader.ReadInt32();
						for (var j = 0; j < phonesCount; j++)
						{
							var phone = new ContactPhone();
							phone.Number = reader.ReadString();
							StoredContact.Phones.Add(phone);
                            StoredContact.Phone = phone;
                            StoredContact.PhoneNumber = phone.Number;
                        }
						break;
				}
			}
		}

		private void SetBackground()
		{
			var accentBrushColor = Core.GetSystemAccentColor();

			accentBrushColor.A = 50;
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

            StoredContact = new WedCheckerContact(contact);

			EditValues();
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			ClearContact();
		}

        public void StoreContact(Contact contact)
        {
            StoredContact = new WedCheckerContact(contact);
        }

        public string GetDataAsText()
        {
            var sb = new StringBuilder();

            if (StoredContact.FirstName != null)
            {
                sb.Append("First name: ");
                sb.AppendLine(StoredContact.FirstName);
            }
            if (StoredContact.LastName != null)
            {
                sb.Append("Last name: ");
                sb.AppendLine(StoredContact.LastName);
            }
            if (StoredContact.Emails.Any())
            {
                sb.Append("Email: ");
                sb.AppendLine(StoredContact.Emails.FirstOrDefault().Address);
            }
            if (StoredContact.Phones.Any())
            {
                sb.Append("Phone: ");
                sb.AppendLine(StoredContact.Phones.FirstOrDefault().Number);
            }
            if (!string.IsNullOrEmpty(StoredContact.Notes) && ShowAlongWithPanel)
            {
                sb.Append("Along with: ");
                sb.AppendLine(StoredContact.Notes);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return StoredContact.FullName;
        }
    }
}
