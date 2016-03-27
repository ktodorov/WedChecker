using System;
using System.Linq;
using System.Reflection;
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
        public Type ConnectedTaskControlType
        {
            get;
            private set;
        }

        private bool TaskOptionsOpened = false;

        public PopulatedTask()
        {
            this.InitializeComponent();
            SetBackgroundColor();
        }


        public PopulatedTask(Type controlType, bool isNew, bool setVisible = false)
        {
            this.InitializeComponent();

            try
            {
                ConnectedTaskControlType = controlType;
				var taskName = controlType.GetProperty("TaskName")?.GetValue(null, null).ToString();
				if (taskName != null)
				{
					buttonTaskName.Text = taskName; // TaskName.ToUpper();
					this.Name = taskName;
				}
                SetBackgroundColor();

				var displayHeader = controlType.GetProperty("DisplayHeader")?.GetValue(null, null).ToString();
				tbTaskHeader.Text = displayHeader;

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
    }
}
