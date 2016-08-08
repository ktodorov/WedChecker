using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WedChecker.Common;
using System.Threading.Tasks;
using System.Text;
using WedChecker.Exceptions;

namespace WedChecker.UserControls.Tasks.Plannings
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

        public static new string DisplayHeader
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


        public override void Serialize(BinaryWriter writer)
        {
            var weddingBudget = tbBudget.Text;
            var tempBudget = 0;
            if (!int.TryParse(weddingBudget, out tempBudget))
            {
                Core.ShowErrorMessage("Please enter a valid number for the budget");
            }
            else
            {
                Budget = tempBudget;
            }

            writer.Write(1);
            writer.Write("Budget");
            writer.Write(Budget);
        }

        public override void Deserialize(BinaryReader reader)
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

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            sb.AppendLine(tbBudgetDisplay.Text);
        }
    }
}
