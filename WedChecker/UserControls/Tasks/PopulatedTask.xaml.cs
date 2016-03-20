using System;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class PopulatedTask : UserControl
    {
        public BaseTaskControl ConnectedTaskControl
        {
            get;
            private set;
        }

        private bool InEditMode = false;
        private bool TaskOptionsOpened = false;

        public PopulatedTask()
        {
            this.InitializeComponent();
            SetBackgroundColor();
        }


        public PopulatedTask(BaseTaskControl control, bool isNew, bool setVisible = false)
        {
            this.InitializeComponent();

            try
            {
                ConnectedTaskControl = control;
                ConnectedTaskControl.Margin = new Thickness(10);
                buttonTaskName.Text = control.TaskName.ToUpper();
                SetBackgroundColor();

                if (!isNew)
                {
                    Task.WaitAll(DisplayConnectedTask(false));
                }
                else
                {
                    EditConnectedTask();
                }

                if (setVisible)
                {
                }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }

        private void SetBackgroundColor()
        {
            var phoneAccentColor = Core.GetSystemAccentColor();

            phoneAccentColor.A = 50;
            var colorBrush = new SolidColorBrush(phoneAccentColor);
            mainPanel.Background = colorBrush;

            //phoneAccentColor.A = 45;
            //colorBrush = new SolidColorBrush(phoneAccentColor);
            //taskOptionsPanel.Background = colorBrush;
            //showTaskOptionsPanel.Background = colorBrush;
        }

        private void EditConnectedTask()
        {
            InEditMode = true;
            tbTaskHeader.Text = ConnectedTaskControl.EditHeader ?? string.Empty;
            if (ConnectedTaskControl != null)
            {
                if (ConnectedTaskControl.Visibility == Visibility.Collapsed)
                {
                    ConnectedTaskControl.Visibility = Visibility.Visible;
                }
                ConnectedTaskControl.EditValues();
            }
        }

        private async Task DisplayConnectedTask(bool shouldSave = true)
        {
            InEditMode = false;
            tbTaskHeader.Text = ConnectedTaskControl.DisplayHeader ?? string.Empty;
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
