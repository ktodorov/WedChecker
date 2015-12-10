using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class ToggleControl : UserControl
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

        public ToggleControl()
        {
            this.InitializeComponent();
        }

        public ToggleControl(string text, bool value = false)
        {
            Title = text;
            toggleSwitch.IsChecked = value;
        }

        public void DisplayValues()
        {
            toggleSwitch.Visibility = Visibility.Collapsed;

            tbTitle.Visibility = Visibility.Visible;
            if (toggleSwitch.IsChecked.Value)
            {
                tbTitle.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                tbTitle.Text = Title;
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                tbTitle.Text = $"Not {Title.ToLower()}";
            }
        }

        public void EditValues()
        {
            toggleSwitch.Visibility = Visibility.Visible;
            tbTitle.Visibility = Visibility.Collapsed;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Title);
            writer.Write(toggleSwitch.IsChecked.Value);
        }

        public void Deserialize(BinaryReader reader)
        {
            Title = reader.ReadString();
            toggleSwitch.IsChecked = reader.ReadBoolean();
        }
    }
}
