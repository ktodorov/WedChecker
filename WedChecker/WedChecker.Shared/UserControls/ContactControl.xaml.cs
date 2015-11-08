using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class ContactControl : UserControl
    {
        public string Id
        {
            get
            {
                return tbId.Text;
            }
        }

        public Contact StoredContact { get; set; }

        private bool EditAlongWith = true;

        public ContactControl()
        {
            this.InitializeComponent();
        }

        public ContactControl(Contact storedContact, string alongWith = null, bool editAlongWith = true)
        {
            this.InitializeComponent();

            StoredContact = storedContact;
            tbId.Text = storedContact.Id;
            tbContactName.Text = $"{storedContact.FirstName} {storedContact.LastName}";

            int parsedAlongWith;
            if (int.TryParse(alongWith, out parsedAlongWith))
            {
                tbAlongWith.Text = alongWith;
                StoredContact.Notes = alongWith;
            }

            EditAlongWith = editAlongWith;
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
    }
}
