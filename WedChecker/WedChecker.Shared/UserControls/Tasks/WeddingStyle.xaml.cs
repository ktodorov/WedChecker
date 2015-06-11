﻿using System;
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
using System.Threading.Tasks;

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
            displayPanel.Visibility = Visibility.Visible;
            tbHeader.Text = "This is what you have planned";
            tbStyle.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbStyle.Text = tbStyleDisplay.Text;
            tbStyle.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
            tbHeader.Text = "Here you can save the style,\nplanned for the wedding.";
        }


        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.WeddingStyle.ToString());
            writer.Write(Style);
        }

        public override void Deserialize(BinaryReader reader)
        {
            Style = reader.ReadString();
            DisplayValues();
        }

        public override async Task SubmitValues()
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
                await AppData.InsertGlobalValue(TaskData.Tasks.WeddingStyle.ToString(), weddingStyle);
            }
        }

        private void tbStyle_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbStyleDisplay.Text = tbStyle.Text;
        }
    }
}
