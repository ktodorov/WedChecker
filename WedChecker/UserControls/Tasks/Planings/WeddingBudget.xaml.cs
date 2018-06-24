using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WedChecker.Common;
using System.Threading.Tasks;
using System.Text;
using WedChecker.Exceptions;
using WedChecker.Helpers;
using WedChecker.Extensions;
using System.ComponentModel;

namespace WedChecker.UserControls.Tasks.Plannings
{
    public partial class WeddingBudget : BaseTaskControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        private double? _budget;
        public double? Budget
        {
            get
            {
                if (_budget == null)
                {
                    _budget = 0;
                }
                return _budget;
            }
            set
            {
                if (_budget == value)
                {
                    return;
                }
                _budget = value;
                RaiseProperty("Budget");
            }
        }

        private double? storedBudget;

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
                return Business.Models.Enums.Tasks.WeddingBudget.ToString();
            }
        }

        public WeddingBudget()
        {
            this.InitializeComponent();
            this.DataContext = this;
            Loaded += WeddingBudget_Loaded;
        }

        private void WeddingBudget_Loaded(object sender, RoutedEventArgs e)
        {
            tbCurrency.Text = CultureInfoHelper.GetCurrentCurrencyString();
        }

        public override void DisplayValues()
        {
            tbBudgetDisplay.Visibility = Visibility.Visible;
            tbBudget.Visibility = Visibility.Collapsed;
            tbCurrency.Visibility = Visibility.Visible;
        }

        public override void EditValues()
        {
            tbBudget.Visibility = Visibility.Visible;
            tbBudgetDisplay.Visibility = Visibility.Collapsed;
            tbBudget.Text = tbBudget.Text.Replace(" ", string.Empty);
            tbCurrency.Visibility = Visibility.Collapsed;
        }

        public override void Serialize(BinaryWriter writer)
        {
            var budgetText = tbBudget.Text.Replace(",", ".");
            if (!budgetText.IsValidPrice())
            {
                Core.ShowErrorMessage("Please enter a valid number for the budget (number, higher than zero and less than 2 million)");
                Budget = storedBudget;
            }

            writer.Write(1);
            writer.Write("BudgetAsDouble");
            writer.Write(Budget.Value);
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
                else if (type == "BudgetAsDouble")
                {
                    Budget = reader.ReadDouble();
                }
            }

            storedBudget = Budget;
            AppData.PlannedBudget = Budget;
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            sb.AppendLine(tbBudgetDisplay.Text);
        }
    }
}
