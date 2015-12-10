using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
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
    public sealed partial class TaskTileControl : UserControl
    {
        public string TaskTitle
        {
            get
            {
                return tbTaskTitle.Text;
            }
            set
            {
                tbTaskTitle.Text = value.ToUpper();
            }
        }

        public string TaskImageUrl { get; set; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    EnabledChanged();
                }
            }
        }

        public TaskTileControl()
        {
            this.InitializeComponent();
        }

        private void EnabledChanged()
        {
            var phoneBrush = Core.GetPhoneAccentBrush();

            if (IsEnabled)
            {
                tileGrid.Background = new SolidColorBrush(Windows.UI.Colors.White); //new SolidColorBrush(phoneBrush);
                tbTaskTitle.Foreground = new SolidColorBrush(phoneBrush); // new SolidColorBrush(Windows.UI.Colors.White);    
            }
            else
            {
                phoneBrush.A = 30;
                tileGrid.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                tbTaskTitle.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkGray);
            }
        }
    }
}
