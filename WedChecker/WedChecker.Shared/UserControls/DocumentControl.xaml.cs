using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class DocumentControl : UserControl
    {
        public int Number { get; set; }

        public DocumentControl()
        {
            this.InitializeComponent();
        }

        public DocumentControl(int number, string name)
        {
            this.InitializeComponent();
            Number = number;
            tbDocumentName.Text = name;
            tbDisplayDocumentName.Text = name;
        }

        public void DisplayValues()
        {
            tbDisplayDocumentName.Text = tbDocumentName.Text;
            tbDocumentName.Visibility = Visibility.Collapsed;
            tbDisplayDocumentName.Visibility = Visibility.Visible;
            removeDocumentButton.Visibility = Visibility.Visible;
            saveDocumentButton.Visibility = Visibility.Collapsed;
        }

        public void EditValues()
        {
            tbDocumentName.Text = tbDisplayDocumentName.Text;
            tbDocumentName.Visibility = Visibility.Visible;
            tbDisplayDocumentName.Visibility = Visibility.Collapsed;
            removeDocumentButton.Visibility = Visibility.Collapsed;
            saveDocumentButton.Visibility = Visibility.Visible;
        }
    }
}
