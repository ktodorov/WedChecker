using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WedChecker.Common;
using System.Threading.Tasks;
using System.Text;
using WedChecker.Exceptions;
using WedChecker.Helpers;

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
            Loaded += WeddingBudget_Loaded;
        }

        private void WeddingBudget_Loaded(object sender, RoutedEventArgs e)
        {
            tbCurrency.Text = CultureInfoHelper.GetCurrentCurrencyString();
        }

        public override void DisplayValues()
        {
            tbBudgetDisplay.Text = Budget.ToString();
            tbBudgetDisplay.Visibility = Visibility.Visible;
            tbBudget.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbBudget.Text = tbBudgetDisplay.Text;
            tbBudget.Visibility = Visibility.Visible;
            tbBudgetDisplay.Visibility = Visibility.Collapsed;
        }


        public override void Serialize(BinaryWriter writer)
        {
            var weddingBudget = tbBudget.Text;
            var tempBudget = 0;
            if (!int.TryParse(weddingBudget, out tempBudget) || tempBudget < 0)
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

        public override async Task Deserialize(BinaryReader reader)
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

            if (Budget > 0)
            {
                AppData.PlannedBudget = Budget;
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
