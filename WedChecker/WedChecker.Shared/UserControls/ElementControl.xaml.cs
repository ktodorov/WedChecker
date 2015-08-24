using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class ElementControl : UserControl
    {
        public int Number { get; set; }

        public ElementControl()
        {
            this.InitializeComponent();
        }

        public ElementControl(int number, string name)
        {
            this.InitializeComponent();
            Number = number;
            tbElementName.Text = name;
            tbDisplayElementName.Text = name;
        }

        public void DisplayValues()
        {
            tbDisplayElementName.Text = tbElementName.Text;
            tbElementName.Visibility = Visibility.Collapsed;
            tbDisplayElementName.Visibility = Visibility.Visible;
            removeElementButton.Visibility = Visibility.Collapsed;
        }

        public void EditValues()
        {
            tbElementName.Text = tbDisplayElementName.Text;
            tbElementName.Visibility = Visibility.Visible;
            tbDisplayElementName.Visibility = Visibility.Collapsed;
            removeElementButton.Visibility = Visibility.Visible;
        }
    }
}
