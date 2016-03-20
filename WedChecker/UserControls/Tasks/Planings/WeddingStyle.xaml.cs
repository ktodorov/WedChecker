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

        public override string EditHeader
        {
            get
            {
                return "Here you can save the style, planned for the wedding";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is the wedding style you have planned";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.WeddingStyle.ToString();
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
            tbStyle.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbStyle.Text = tbStyleDisplay.Text;
            tbStyle.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
        }


        protected override void Serialize(BinaryWriter writer)
        {
            var weddingStyle = tbStyle.Text;
            Style = weddingStyle;

            writer.Write(Style);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            Style = reader.ReadString();
        }

        private void tbStyle_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbStyleDisplay.Text = tbStyle.Text;
        }
    }
}
