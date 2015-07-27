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
