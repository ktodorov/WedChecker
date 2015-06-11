using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class PopulatedTask : UserControl
    {
        private BaseTaskControl ConnectedTaskControl
        {
            get;
            set;
        }

        public PopulatedTask()
        {
            this.InitializeComponent();
            SetBackgroundColor();
        }
        public PopulatedTask(BaseTaskControl control)
        {
            this.InitializeComponent();
            ConnectedTaskControl = control;
            spConnectedControl.Children.Add(ConnectedTaskControl);
            buttonTaskName.Content = control.TaskName.ToUpper();
            SetBackgroundColor();
        }

        private void SetBackgroundColor()
        {
            if (AppData.LocalAppData.Keys.Contains("PopulatedTaskControlColor") &&
                AppData.LocalAppData.Keys.Contains("PopulatedTaskControlBorderBrush"))
            {
                mainGrid.Background = AppData.LocalAppData["PopulatedTaskControlColor"] as SolidColorBrush;
                mainBorder.BorderBrush = AppData.LocalAppData["PopulatedTaskControlBorderBrush"] as SolidColorBrush;
            }
            else
            {
                var phoneAccentBrush = new SolidColorBrush((App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color);
                var color = phoneAccentBrush.Color;
                color.A = 100;
                var colorBrush = new SolidColorBrush(color);
                mainBorder.BorderBrush = colorBrush;
                AppData.LocalAppData["PopulatedTaskControlBorderBrush"] = colorBrush;
                color.A = 25;
                colorBrush = new SolidColorBrush(color);
                mainGrid.Background = colorBrush;
                AppData.LocalAppData["PopulatedTaskControlColor"] = colorBrush;
            }
        }

        void editTask_Click(object sender, RoutedEventArgs e)
        {
            ConnectedTaskControl.EditValues();
        }

        private void buttonTaskName_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectedTaskControl != null && ConnectedTaskControl.Visibility == Visibility.Collapsed)
            {
                ConnectedTaskControl.Visibility = Visibility.Visible;
            }
            else if (ConnectedTaskControl != null && ConnectedTaskControl.Visibility == Visibility.Visible)
            {
                ConnectedTaskControl.Visibility = Visibility.Collapsed;
            }
        }
    }
}
