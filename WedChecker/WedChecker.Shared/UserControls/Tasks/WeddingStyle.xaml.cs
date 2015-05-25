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
using WedChecker.Common;
using WedChecker.UserControls.Tasks;
using System.IO.Compression;
using System.Text;

namespace WedChecker.UserControls.Tasks
{
    public partial class WeddingStyle : BaseTaskControl
    {
        private string Style
        {
            get;
            set;
        }

        public override string TaskName
        {
            get
            {
                return "Style";
            }
        }

        public WeddingStyle()
        {
            this.InitializeComponent();
        }

        public WeddingStyle(string value)
        {
            this.InitializeComponent();
            Style = value;
            DisplayValues();
        }

        public override void DisplayValues()
        {
            tbStyleDisplay.Text = Style;
            tbStyleDisplay.Visibility = Visibility.Visible;
            tbHeader.Text = "This is what you have planned";
            stylePickerButton.Visibility = Visibility.Collapsed;
            tbStyle.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbStyle.Text = tbStyleDisplay.Text;
            tbStyle.Visibility = Visibility.Visible;
            tbStyleDisplay.Visibility = Visibility.Collapsed;
            tbHeader.Text = "Here you can save the style,\nplanned for the wedding.";
            stylePickerButton.Visibility = Visibility.Visible;
        }


        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.WeddingStyle);
            writer.Write(Style);
        }

        public override BaseTaskControl Deserialize(BinaryReader reader)
        {
            var style = reader.ReadString();

            return new WeddingStyle(style);
        }

        private async void stylePickerButton_Click(object sender, RoutedEventArgs e)
        {
            var weddingStyle = tbStyle.Text;
            if (string.IsNullOrEmpty(weddingStyle))
            {
                tbErrorMessage.Text = "Please, enter not an empty style.";
                return;
            }

            if (Style != weddingStyle)
            {
                Style = weddingStyle;
                await AppData.InsertGlobalValue(TaskData.Tasks.WeddingStyle, weddingStyle);
            }
            DisplayValues();
        }

        private void tbStyle_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbStyleDisplay.Text = tbStyle.Text;
        }
    }
}
