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

namespace WedChecker.CustomControls
{
    public sealed partial class LaunchControl : UserControl
    {
        public LaunchControl()
        {
            this.InitializeComponent();
            HeaderDialogTextBlock.Text = "Hello,\nwelcome to";
            TitleDialogTextBlock.Text = "WedChecker";
            DialogTextBlock.Text = "No doubt we will make a wonderful wedding.\nCan I know your name first?\nIt will help me to know you better.";
        }
    }
}
