using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
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

        public static new string DisplayHeader
        {
            get
            {
                return "This is the decoration you have planned";
            }
        }

        public override string TaskCode
        {
            get
            {
                return Business.Models.Enums.Tasks.Decoration.ToString();
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
            var decoration = tbDecoration.Text;
            PlannedDecoration = decoration;

            writer.Write(PlannedDecoration);
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            PlannedDecoration = reader.ReadString();
        }

        protected override void SetLocalStorage()
        {
            //if (string.IsNullOrEmpty(decoration))
            //{
            //    tbErrorMessage.Text = "Please, do not enter an empty decoration.";
            //    return;
            //}
            AppData.SetStorage("Decoration", PlannedDecoration);
        }

        private void tbDecoration_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbDecorationDisplay.Text = tbDecoration.Text;
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            sb.AppendLine(tbDecoration.Text);
        }
    }
}
