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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class BudgetPicker : UserControl
    {
        public BudgetPicker()
        {
            this.InitializeComponent();
        }

        public BudgetPicker(int value)
        {
            this.InitializeComponent();
            tbBudget.Text = value.ToString();
            tbHeader.Text = "This is your planned budget";
            budgetPickerButton.Visibility = Visibility.Collapsed;
        }

        private async void budgetPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var weddingBudget = tbBudget.Text;
            if (string.IsNullOrEmpty(weddingBudget) || Convert.ToInt32(weddingBudget) < 0)
            {
                tbErrorMessage.Text = "Please, enter a valid budget!";
                return;
            }

            await AppData.InsertGlobalValue("WeddingBudget", weddingBudget);
        }
    }
}
