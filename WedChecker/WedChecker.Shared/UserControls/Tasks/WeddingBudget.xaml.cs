using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WedChecker.Common;
using WedChecker.UserControls.Tasks;
using System.IO.Compression;
using System.Text;

namespace WedChecker.UserControls.Tasks
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

        public WeddingBudget()
        {
            this.InitializeComponent();
        }

        public WeddingBudget(int value)
        {
            this.InitializeComponent();
            Budget = value;
            DisplayValues();
        }

        public override void DisplayValues()
        {
            tbBudgetDisplay.Text = Budget.ToString();
            tbBudgetDisplay.Visibility = Visibility.Visible;
            tbHeader.Text = "This is what you have planned";
            budgetPickerButton.Visibility = Visibility.Collapsed;
            tbBudget.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbBudget.Text = tbBudgetDisplay.Text;
            tbBudget.Visibility = Visibility.Visible;
            tbBudgetDisplay.Visibility = Visibility.Collapsed;
            tbHeader.Text = "What is your budget?\nYou can edit this at any time.";
            budgetPickerButton.Visibility = Visibility.Visible;
        }


        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.WeddingBudget);
            writer.Write(Budget);
        }

        public override void Deserialize(BinaryReader reader)
        {
            Budget = reader.ReadInt32();
            
            DisplayValues();
        }

        private async void budgetPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var weddingBudget = tbBudget.Text;
            if (string.IsNullOrEmpty(weddingBudget) || Convert.ToInt32(weddingBudget) < 0)
            {
                tbErrorMessage.Text = "Please, enter a valid budget!";
                return;
            }

            if (Budget != Convert.ToInt32(weddingBudget))
            {
                Budget = Convert.ToInt32(weddingBudget);
                await AppData.InsertGlobalValue(TaskData.Tasks.WeddingBudget, weddingBudget);
            }
            DisplayValues();
        }

        private void tbBudget_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBudgetDisplay.Text = tbBudget.Text;
        }
    }
}
