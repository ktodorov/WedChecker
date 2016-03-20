using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class ElementControl : UserControl
    {
        public int Number { get; set; }

        public string Title { get; set; }

        public ElementControl()
        {
            this.InitializeComponent();
        }

        public ElementControl(string name)
        {
            this.InitializeComponent();
            Number = 0;
            tbElementName.Text = name;
            tbDisplayElementName.Text = name;
            Title = name;
        }

        public ElementControl(int number, string name)
        {
            this.InitializeComponent();
            Number = number;
            tbElementName.Text = name;
            tbDisplayElementName.Text = name;
            Title = name;
        }

        public void DisplayValues()
        {
            tbDisplayElementName.Text = Title;
            tbElementName.Visibility = Visibility.Collapsed;
            tbDisplayElementName.Visibility = Visibility.Visible;
            removeElementButton.Visibility = Visibility.Collapsed;
        }

        public void EditValues()
        {
            tbElementName.Text = Title;
            tbElementName.Visibility = Visibility.Visible;
            tbDisplayElementName.Visibility = Visibility.Collapsed;
            removeElementButton.Visibility = Visibility.Visible;
        }

        public override string ToString()
        {
            return $"{Number}.{tbElementName.Text}";
        }

        private void tbElementName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Title = tbElementName.Text;
        }
    }
}
