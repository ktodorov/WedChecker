using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WedChecker.Common;
using System.Threading.Tasks;

namespace WedChecker.UserControls.Tasks.Planings
{
    public partial class WeddingBudget : BaseTaskControl
    {
        private int Budget
        {
            get;
            set;
        }

        public override string TaskName
        {
            get
            {
                return "Budget";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "What is your budget? You can edit this at any time";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is the wedding budget you have planned";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.WeddingBudget.ToString();
            }
        }

        public WeddingBudget()
        {
            this.InitializeComponent();
        }

        public WeddingBudget(int value)
        {
            this.InitializeComponent();
            Budget = value;
        }

        public override void DisplayValues()
        {
            tbBudgetDisplay.Text = Budget.ToString();
            displayPanel.Visibility = Visibility.Visible;
            tbBudget.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbBudget.Text = tbBudgetDisplay.Text;
            tbBudget.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
        }


        protected override void Serialize(BinaryWriter writer)
        {
            var weddingBudget = tbBudget.Text;
            Budget = Convert.ToInt32(weddingBudget);

            writer.Write(1);
            writer.Write("Budget");
            writer.Write(Budget);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            for (var i = 0; i < objectsCount; i++)
            {
                var type = reader.ReadString();
                if (type == "Budget")
                {
                    Budget = reader.ReadInt32();
                }
            }
        }

        private void tbBudget_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBudgetDisplay.Text = tbBudget.Text;
        }
    }
}
