using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
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
	public sealed partial class ContactControl : UserControl, IStorableTask
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

		public ContactControl(Contact storedContact, string alongWith = null, bool editAlongWith = false, bool isEditable = false, bool isReplaceable = false)
		{
			this.InitializeComponent();

			IsEditable = isEditable;
			EditAlongWith = editAlongWith;
			IsReplaceable = isReplaceable;

			StoreContact(storedContact);

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

			StoredContact = contact;

			tbId.Text = StoredContact.Id;

			if (!string.IsNullOrEmpty(StoredContact.FirstName) || !string.IsNullOrEmpty(StoredContact.LastName))
			{
				tbContactName.Text = $"{StoredContact.FirstName} {StoredContact.LastName}";
				tbEditContactName.Text = $"{StoredContact.FirstName} {StoredContact.LastName}";
			}

			if (StoredContact.Emails.Count > 0)
			{
				var emails = string.Empty;

				foreach (var email in StoredContact.Emails)
				{
					emails += $"{email.Address}";
				}

				var hyperlinkText = CreateNewHyperlink($"mailto:{emails}", emails);
				tbContactEmails.Inlines.Clear();
				tbContactEmails.Inlines.Add(hyperlinkText);
				tbEditContactEmails.Text = emails;
			}

			if (StoredContact.Phones.Count > 0)
			{
				var phones = string.Empty;

				foreach (var phone in StoredContact.Phones)
				{
					phones += $"{phone.Number}";
				}

				var hyperlinkText = CreateNewHyperlink($"tel:{phones}", phones);
				tbContactPhones.Inlines.Clear();
				tbContactPhones.Inlines.Add(hyperlinkText);
				tbEditContactPhones.Text = phones;
			}

			if (!string.IsNullOrEmpty(StoredContact.Notes))
			{
				tbAlongWith.Text = StoredContact.Notes;
				tbCheckboxTextDisplay.Text = StoredContact.Notes;
			}

			AdjustVisibility();
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

		public void ClearContact()
		{
			StoredContact = null;
			ClearAllFields();

			AdjustVisibility();

			EditValues();
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

			if (!string.IsNullOrEmpty(tbCheckboxTextDisplay.Text))
			{
				tbCheckboxTextDisplay.Visibility = Visibility.Visible;
			}
			else
			{
				tbCheckboxTextDisplay.Visibility = Visibility.Collapsed;
			}

			selectContactButton.Visibility = Visibility.Collapsed;
		}

		public void EditValues()
		{
			AdjustVisibility(IsEditable);

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

			if (EditAlongWith)
			{
				tbCheckboxText.Visibility = Visibility.Visible;
				tbAlongWith.Visibility = Visibility.Visible;
				tbCheckboxTextDisplay.Visibility = Visibility.Collapsed;
			}

			var namesEntered = (!string.IsNullOrEmpty(StoredContact.FirstName) || !string.IsNullOrEmpty(StoredContact.LastName));
			if (!namesEntered)
			{
				return;
			}

			deleteButton.Visibility = Visibility.Visible;
		}

		private void AdjustVisibility(bool setAllVisible = false)
		{
			var namesEntered = (!string.IsNullOrEmpty(StoredContact.FirstName) || !string.IsNullOrEmpty(StoredContact.LastName));

			if (namesEntered ||
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

			if (!ShowAlongWithPanel || (!ShowAlongWithPanel && string.IsNullOrEmpty(StoredContact.Notes)))
			{
				alongWithPanel.Visibility = Visibility.Collapsed;
			}
			else if (namesEntered)
			{
				alongWithPanel.Visibility = Visibility.Visible;

				var alongWith = 0;
				if (int.TryParse(StoredContact.Notes, out alongWith))
				{
					tbCheckboxTextDisplay.Visibility = Visibility.Visible;
					tbCheckboxTextDisplay.Text = $"Along with {StoredContact.Notes.ToString()} other";
				}

				if (EditAlongWith)
				{
					tbCheckboxTextDisplay.Visibility = Visibility.Collapsed;

					tbCheckboxText.Visibility = Visibility.Visible;
					tbAlongWith.Visibility = Visibility.Visible;
				}
				else
				{
					tbAlongWith.Visibility = Visibility.Collapsed;
					tbCheckboxText.Visibility = Visibility.Collapsed;
				}
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
			if (StoredContact.Notes != null && ShowAlongWithPanel) result++;
			if (StoredContact.Emails.Count > 0) result++;
			if (StoredContact.Phones.Count > 0) result++;

			return result;
		}

		public void Serialize(BinaryWriter writer)
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
			if (StoredContact.Notes != null && ShowAlongWithPanel)
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

		public void Deserialize(BinaryReader reader)
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
			var hyperlinkText = CreateNewHyperlink($"mailto:{tbEditContactEmails.Text}", tbEditContactEmails.Text);
			tbContactEmails.Inlines.Clear();
			tbContactEmails.Inlines.Add(hyperlinkText);

			StoredContact.Emails.Clear();

			if (!string.IsNullOrEmpty(tbEditContactEmails.Text))
			{
				var email = new ContactEmail();
				email.Address = tbEditContactEmails.Text;

				StoredContact.Emails.Add(email);
			}

			// Phones
			hyperlinkText = CreateNewHyperlink($"tel:{tbEditContactPhones.Text}", tbEditContactPhones.Text);
			tbContactPhones.Inlines.Clear();
			tbContactPhones.Inlines.Add(hyperlinkText);
			StoredContact.Phones.Clear();

			if (!string.IsNullOrEmpty(tbEditContactPhones.Text))
			{
				var phone = new ContactPhone();
				phone.Number = tbEditContactPhones.Text;

				StoredContact.Phones.Add(phone);
			}

			// Notes
			StoredContact.Notes = string.Empty;
			tbCheckboxTextDisplay.Text = string.Empty;

			if (!string.IsNullOrEmpty(tbAlongWith.Text))
			{
				StoredContact.Notes = tbAlongWith.Text;
				tbCheckboxTextDisplay.Text = $"Along with {StoredContact.Notes.ToString()} other";
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

			StoreContact(contact);

			EditValues();
		}

		private Hyperlink CreateNewHyperlink(string navigateUri, string text)
		{
			var currentAccentColorHex = Core.GetSystemAccentColor();

			// Navigate URI
			var hyperlinkText = new Hyperlink();
			hyperlinkText.NavigateUri = new Uri(navigateUri);

			// Inline text
			var line = new Run();
			line.Text = text;
			hyperlinkText.Inlines.Add(line);

			// Foreground
			SolidColorBrush backColor = new SolidColorBrush(currentAccentColorHex);
			hyperlinkText.Foreground = backColor;

			return hyperlinkText;
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			ClearContact();
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
	}
}
