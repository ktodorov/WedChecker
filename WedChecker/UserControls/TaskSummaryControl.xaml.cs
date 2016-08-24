using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using WedChecker.Extensions;
using WedChecker.Helpers;
using WedChecker.Interfaces;
using WedChecker.UserControls.Tasks;
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

namespace WedChecker.UserControls
{
    public sealed partial class TaskSummaryControl : UserControl
    {
        private BaseTaskControl task;

        public TaskSummaryControl()
        {
            this.InitializeComponent();
        }

        public void LoadTaskData(BaseTaskControl populatedTask)
        {
            task = populatedTask;

            var taskType = populatedTask.GetType();
            var taskCategory = Core.GetTaskCategory(taskType);

            var completableTask = populatedTask as ICompletableTask;
            if (completableTask != null)
            {
                var finishedCount = completableTask.GetCompletedItems();
                var unfinishedCount = completableTask.GetUncompletedItems();
                var allCount = finishedCount + unfinishedCount;

                if (allCount != finishedCount)
                {
                    unfinishedPanel.Visibility = Visibility.Visible;
                    finishedPanel.Visibility = Visibility.Collapsed;

                    tasksCountBlock.Text = finishedCount.ToString();
                    allTasksCountBlock.Text = allCount.ToString();
                }
                else
                {
                    unfinishedPanel.Visibility = Visibility.Collapsed;
                    finishedPanel.Visibility = Visibility.Visible;
                }

                if (populatedTask is IPricedTask)
                {
                    var purchaseValue = (populatedTask as IPricedTask).GetPurchasedItemsValue();
                    if (purchaseValue > 0)
                    {
                        tbPurchaseValue.Text = purchaseValue.ToCurrency();
                        purchaseValuePanel.Visibility = Visibility.Visible;
                        return;
                    }
                }

                purchaseValuePanel.Visibility = Visibility.Collapsed;
                tbPurchaseValue.Text = string.Empty;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        public void RefreshTaskData(BaseTaskControl taskControl)
        {
            LoadTaskData(taskControl);
        }
    }
}
