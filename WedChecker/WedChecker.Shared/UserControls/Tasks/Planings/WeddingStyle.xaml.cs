using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WedChecker.Common;
using System.Threading.Tasks;

namespace WedChecker.UserControls.Tasks.Planings
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
        }

        public override void DisplayValues()
        {
            tbStyleDisplay.Text = Style;
            displayPanel.Visibility = Visibility.Visible;
            tbHeader.Text = "This is the wedding style you have planned";
            tbStyle.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbStyle.Text = tbStyleDisplay.Text;
            tbStyle.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
            tbHeader.Text = "Here you can save the style,\nplanned for the wedding";
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
                tbErrorMessage.Text = "Please, do not enter an empty style.";
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
