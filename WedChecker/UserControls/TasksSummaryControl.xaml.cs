using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using WedChecker.Extensions;
using WedChecker.Interfaces;
using WedChecker.UserControls.Tasks;
using WedChecker.UserControls.Tasks.Bookings;
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
    public sealed partial class TasksSummaryControl : UserControl
    {
        private List<BaseTaskControl> tasks;

        public TasksSummaryControl()
        {
            this.InitializeComponent();
        }

        public void LoadTasksData(List<BaseTaskControl> populatedTasks)
        {
            tasks = populatedTasks;

            var purchasesCount = 0;
            var purchasedCount = 0;
            var bookingsCount = 0;
            var bookedCount = 0;
            var planningsCount = 0;

            foreach (var populatedTask in populatedTasks)
            {
                var taskType = populatedTask.GetType();
                var taskCategory = Core.GetTaskCategory(taskType);

                switch (taskCategory)
                {
                    case TaskCategories.Booking:
                        var bookingTask = populatedTask as ICompletableTask;
                        if (bookingTask != null)
                        {
                            var booked = bookingTask.GetCompletedItems();
                            var unbooked = bookingTask.GetUncompletedItems();
                            bookedCount += booked;
                            bookingsCount += booked + unbooked;
                        }
                        else
                        {
                            bookedCount++;
                            bookingsCount++;
                        }
                        break;

                    case TaskCategories.Purchase:
                        var purchasingTask = populatedTask as ICompletableTask;
                        if (purchasingTask != null)
                        {
                            var purchased = purchasingTask.GetCompletedItems();
                            var unpurchased = purchasingTask.GetUncompletedItems();
                            purchasedCount += purchased;
                            purchasesCount += purchased + unpurchased;
                        }
                        else
                        {
                            purchasedCount++;
                            purchasesCount++;
                        }
                        break;

                    case TaskCategories.Planing:
                        planningsCount++;
                        break;
                }
            }

            purchasedTasksCountBlock.Text = purchasedCount.ToString();
            purchasingTasksCountBlock.Text = purchasesCount.ToString();
            bookedTasksCountBlock.Text = bookedCount.ToString();
            bookingTasksCountBlock.Text = bookingsCount.ToString();
            planningTasksCountBlock.Text = planningsCount.ToString();

            mainBorder.Visibility = Visibility.Visible;
        }

        public void AddNewTaskInfo(BaseTaskControl task)
        {
            tasks.Add(task);

            var taskType = task.GetType();
            var taskCategory = Core.GetTaskCategory(taskType);

            switch (taskCategory)
            {
                case TaskCategories.Booking:
                    var currentBooked = bookedTasksCountBlock.Text.ToInteger();
                    var currentBooking = bookingTasksCountBlock.Text.ToInteger();
                    var booked = 0;
                    var unbooked = 1;

                    var bookingTask = task as ICompletableTask;
                    if (bookingTask != null)
                    {
                        booked = bookingTask.GetCompletedItems();
                        unbooked = bookingTask.GetUncompletedItems();
                    }

                    var newBooked = booked + currentBooked;
                    var newBooking = currentBooking + booked + unbooked;

                    bookedTasksCountBlock.Text = newBooked.ToString();
                    bookingTasksCountBlock.Text = newBooking.ToString();
                    break;

                case TaskCategories.Purchase:
                    var currentPurchased = purchasedTasksCountBlock.Text.ToInteger();
                    var currentPurchasing = purchasingTasksCountBlock.Text.ToInteger();
                    var purchased = 0;
                    var unpurchased = 1;

                    var purchasingTask = task as ICompletableTask;
                    if (purchasingTask != null)
                    {
                        purchased = purchasingTask.GetCompletedItems();
                        unpurchased = purchasingTask.GetUncompletedItems();
                    }

                    var newPurchased = purchased + currentPurchased;
                    var newPurchasing = currentPurchasing + purchased + unpurchased;

                    purchasedTasksCountBlock.Text = newPurchased.ToString();
                    purchasingTasksCountBlock.Text = newPurchasing.ToString();
                    break;

                case TaskCategories.Planing:
                    var currentPlanning = planningTasksCountBlock.Text.ToInteger();
                    var newPlanning = currentPlanning + 1;
                    planningTasksCountBlock.Text = newPlanning.ToString();
                    break;
            }
        }

        public void RemoveTaskInfo(BaseTaskControl task)
        {
            tasks.Remove(task);

            var taskType = task.GetType();
            var taskCategory = Core.GetTaskCategory(taskType);

            switch (taskCategory)
            {
                case TaskCategories.Booking:
                    var currentBooked = bookedTasksCountBlock.Text.ToInteger();
                    var currentBooking = bookingTasksCountBlock.Text.ToInteger();
                    var booked = 0;
                    var unbooked = 1;

                    var bookingTask = task as ICompletableTask;
                    if (bookingTask != null)
                    {
                        booked = bookingTask.GetCompletedItems();
                        unbooked = bookingTask.GetUncompletedItems();
                    }

                    var newBooked = currentBooked - booked;
                    var newBooking = currentBooking - (booked + unbooked);

                    bookedTasksCountBlock.Text = newBooked.ToString();
                    bookingTasksCountBlock.Text = newBooking.ToString();
                    break;

                case TaskCategories.Purchase:
                    var currentPurchased = purchasedTasksCountBlock.Text.ToInteger();
                    var currentPurchasing = purchasingTasksCountBlock.Text.ToInteger();
                    var purchased = 0;
                    var unpurchased = 1;

                    var purchasingTask = task as ICompletableTask;
                    if (purchasingTask != null)
                    {
                        purchased = purchasingTask.GetCompletedItems();
                        unpurchased = purchasingTask.GetUncompletedItems();
                    }

                    var newPurchased = currentPurchased - purchased;
                    var newPurchasing = currentPurchasing - (purchased + unpurchased);

                    purchasedTasksCountBlock.Text = newPurchased.ToString();
                    purchasingTasksCountBlock.Text = newPurchasing.ToString();
                    break;

                case TaskCategories.Planing:
                    var currentPlanning = planningTasksCountBlock.Text.ToInteger();
                    var newPlanning = currentPlanning - 1;
                    planningTasksCountBlock.Text = newPlanning.ToString();
                    break;
            }
        }

        public void UpdateTaskInfo(BaseTaskControl task)
        {
            var savedTask = tasks.FirstOrDefault(t => t.TaskCode == task.TaskCode);
            tasks.Remove(savedTask);
            tasks.Add(task);

            LoadTasksData(tasks);
        }
    }
}
