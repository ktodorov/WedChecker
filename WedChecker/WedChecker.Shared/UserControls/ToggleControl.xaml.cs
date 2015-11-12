using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
                return tbTitle.Text;
            }
            set
            {
                tbTitle.Text = value;
            }
        }

        public bool Toggled
        {
            get
            {
                return toggleSwitch.IsOn;
            }
            set
            {
                toggleSwitch.IsOn = value;
            }
        }

        public ToggleControl()
        {
            this.InitializeComponent();
        }

        public ToggleControl(string text, bool value = false)
        {
            Title = text;
            toggleSwitch.IsOn = value;
        }

        public void DisplayValues()
        {
            toggleSwitch.IsEnabled = false;
        }

        public void EditValues()
        {
            toggleSwitch.IsEnabled = true;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Title);
            writer.Write(toggleSwitch.IsOn);
        }

        public void Deserialize(BinaryReader reader)
        {
            Title = reader.ReadString();
            toggleSwitch.IsOn = reader.ReadBoolean();
        }
    }
}
