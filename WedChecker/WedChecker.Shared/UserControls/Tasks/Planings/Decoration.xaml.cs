using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class Decoration : BaseTaskControl
    {
        private string PlannedDecoration { get; set; } = string.Empty;

        public override string TaskName
        {
            get
            {
                return "Decoration";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can save the decoration, planned for the wedding";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is the decoration you have planned";
            }
        }

        public Decoration()
        {
            this.InitializeComponent();
        }

        public Decoration(string value)
        {
            this.InitializeComponent();
            PlannedDecoration = value;
        }

        public override void DisplayValues()
        {
            tbDecorationDisplay.Text = PlannedDecoration;
            displayPanel.Visibility = Visibility.Visible;
            tbDecoration.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbDecoration.Text = tbDecorationDisplay.Text;
            tbDecoration.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
        }


        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.Decoration.ToString());
            writer.Write(PlannedDecoration);
        }

        public override void Deserialize(BinaryReader reader)
        {
            PlannedDecoration = reader.ReadString();
            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            var decoration = tbDecoration.Text;
            if (string.IsNullOrEmpty(decoration))
            {
                tbErrorMessage.Text = "Please, do not enter an empty decoration.";
                return;
            }

            if (PlannedDecoration != decoration)
            {
                PlannedDecoration = decoration;
                await AppData.InsertGlobalValue(TaskData.Tasks.Decoration.ToString(), decoration);
            }
        }

        private void tbDecoration_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbDecorationDisplay.Text = tbDecoration.Text;
        }
    }
}
