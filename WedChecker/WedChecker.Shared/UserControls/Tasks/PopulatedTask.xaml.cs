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
        }
        public PopulatedTask(BaseTaskControl control)
        {
            this.InitializeComponent();
            ConnectedTaskControl = control;
            tbTaskName.Text = control.TaskName;
        }

        void editTask_Click(object sender, RoutedEventArgs e)
        {
            ConnectedTaskControl.EditValues();
        }
    }
}
