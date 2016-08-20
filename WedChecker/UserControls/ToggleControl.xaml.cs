using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class ToggleControl : UserControl, IStorableTask
    {
        public string Title
        {
            get
            {
                return tbCheckboxTitle.Text;
            }
            set
            {
                tbCheckboxTitle.Text = value;
                tbTitle.Text = value;
            }
        }

        public bool Toggled
        {
            get
            {
                return toggleSwitch.IsChecked.Value;
            }
            set
            {
                toggleSwitch.IsChecked = value;
            }
        }

        public event EventHandler Checked;

        public ToggleControl()
        {
            this.InitializeComponent();
        }

        public void DisplayValues()
        {
            toggleSwitch.Visibility = Visibility.Collapsed;

            //tbTitle.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Visible;
            tbTitle.Text = Title;
            if (toggleSwitch.IsChecked.Value)
            {
                //tbTitle.Text = Title;
                tbTitleSymbol.Text = "\uE10B";
                tbTitleSymbol.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            }
            else
            {
                tbTitleSymbol.Text = "\uE10A";
                tbTitleSymbol.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                //tbTitle.Text = $"Not {Title.ToLower()}";
            }
        }

        public void EditValues()
        {
            toggleSwitch.Visibility = Visibility.Visible;
            //tbTitle.Visibility = Visibility.Collapsed;
            displayPanel.Visibility = Visibility.Collapsed;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Title);
            writer.Write(toggleSwitch.IsChecked.Value);
        }

        public async Task Deserialize(BinaryReader reader)
        {
            Title = reader.ReadString();
            toggleSwitch.IsChecked = reader.ReadBoolean();
        }

        public string GetDataAsText()
        {
            var sb = new StringBuilder();
            sb.Append($" - {Title} - ");
            sb.Append(Toggled ? "Yes" : "No");

            return sb.ToString();
        }

        private void toggleSwitch_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Checked?.Invoke(this, new EventArgs());
        }
    }
}
