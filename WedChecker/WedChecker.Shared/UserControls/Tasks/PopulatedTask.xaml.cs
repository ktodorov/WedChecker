using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        public PopulatedTask(BaseTaskControl control, bool isNew)
        {
            this.InitializeComponent();
            ConnectedTaskControl = control;
            spConnectedControl.Children.Add(ConnectedTaskControl);
            buttonTaskName.Content = control.TaskName.ToUpper();
            SetBackgroundColor();

            if (!isNew)
            {
                ConnectedTaskControl.DisplayValues();
                Task.WaitAll(DisplayConnectedTask(false));
            }
            else
            {
                EditConnectedTask();
            }
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

        private void buttonTaskName_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectedTaskControl != null && ConnectedTaskControl.Visibility == Visibility.Collapsed)
            {
                ChangeMainContentVisibility(Visibility.Visible);
            }
            else if (ConnectedTaskControl != null && ConnectedTaskControl.Visibility == Visibility.Visible)
            {
                ChangeMainContentVisibility(Visibility.Collapsed);
            }
        }

        private void ChangeMainContentVisibility(Visibility visibility)
        {
            ConnectedTaskControl.Visibility = visibility;
            borderSplitter.Visibility = visibility;
            tbTaskHeader.Visibility = visibility;
            editTask.Visibility = visibility;
            displayTask.Visibility = visibility;
        }

        void editTask_Click(object sender, RoutedEventArgs e)
        {
            EditConnectedTask();
        }

        private void EditConnectedTask()
        {
            tbTaskHeader.Text = ConnectedTaskControl.EditHeader ?? string.Empty;
            displayTask.Visibility = Visibility.Visible;
            editTask.Visibility = Visibility.Collapsed;
            if (ConnectedTaskControl != null)
            {
                if (ConnectedTaskControl.Visibility == Visibility.Collapsed)
                {
                    ConnectedTaskControl.Visibility = Visibility.Visible;
                }
                ConnectedTaskControl.EditValues();
            }
        }

        private async void displayTask_Click(object sender, RoutedEventArgs e)
        {
            await DisplayConnectedTask();
        }

        private async Task DisplayConnectedTask(bool shouldSave = true)
        {
            tbTaskHeader.Text = ConnectedTaskControl.DisplayHeader ?? string.Empty;
            displayTask.Visibility = Visibility.Collapsed;
            editTask.Visibility = Visibility.Visible;
            if (ConnectedTaskControl != null)
            {
                if (shouldSave)
                {
                    await ConnectedTaskControl.SubmitValues();
                }
                ConnectedTaskControl.DisplayValues();
            }
        }
    }
}
