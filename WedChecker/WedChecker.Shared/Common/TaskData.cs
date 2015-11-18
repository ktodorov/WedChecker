using System;
using System.Collections.Generic;
using System.Text;
using WedChecker.UserControls.Tasks;
using WedChecker.UserControls.Tasks.Planings;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Reflection;

#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
#endif
namespace WedChecker.Common
{
    public static partial class TaskData
    {
        public static void CreateTaskControl(Page currentPage, KeyValuePair<string, object> populatedControl)
        {
            object value = populatedControl.Value;
            var type = GetTaskType(populatedControl.Key);
            BaseTaskControl taskControl = null;
            taskControl = CreateTaskControl(type, value);
            AppData.InsertSerializableTask(taskControl);

            InsertTaskControl(currentPage, type, taskControl);
        }

        public static void InsertTaskControl(Page currentPage, Type type, BaseTaskControl taskControl, bool isNew = true)
        {
#if WINDOWS_PHONE_APP
            var lbTasks = currentPage.FindName("LbTasks") as ItemsControl;
            lbTasks.Visibility = Visibility.Collapsed;

            var appBar = currentPage.FindName("appBar") as AppBar;
            appBar.Visibility = Visibility.Visible;

            var mainPivot = currentPage.FindName("mainPivot") as Pivot;
            mainPivot.Visibility = Visibility.Visible;

            var pivotName = GetPivotNameFromType(type, mainPivot);
            var pivotStackPanel = currentPage.FindName(pivotName) as StackPanel;
            pivotStackPanel.Children.Add(new PopulatedTask(taskControl, isNew));

#else

#endif
        }

        private static string GetPivotNameFromType(Type taskType, Pivot mainPivot = null)
        {
            var result = string.Empty;

            if (taskType.FullName.StartsWith("WedChecker.UserControls.Tasks.Bookings"))
            {
                result = "spBookings";
                if (mainPivot != null)
                {
                    mainPivot.SelectedIndex = 3;
                }
            }
            else if (taskType.FullName.StartsWith("WedChecker.UserControls.Tasks.Planings"))
            {
                result = "spPlanings";
                if (mainPivot != null)
                {
                    mainPivot.SelectedIndex = 1;
                }
            }
            else if (taskType.FullName.StartsWith("WedChecker.UserControls.Tasks.Purchases"))
            {
                result = "spPurchases";
                if (mainPivot != null)
                {
                    mainPivot.SelectedIndex = 2;
                }
            }
            else
            {
                throw new Exception("Could not recognize the task type");
            }

            return result;
        }

        public static BaseTaskControl GetTaskControlFromString(string taskControlName)
        {
            var type = GetTaskType(taskControlName);
            return Activator.CreateInstance(type) as BaseTaskControl;
        }

        public static Type GetTaskType(string taskName)
        {
            Type type;

            type = Type.GetType($"WedChecker.UserControls.Tasks.Bookings.{taskName}");
            if (type == null)
            {
                type = Type.GetType($"WedChecker.UserControls.Tasks.Planings.{taskName}");
            }

            if (type == null)
            {
                type = Type.GetType($"WedChecker.UserControls.Tasks.Purchases.{taskName}");
            }

            if (type == null)
            {
                throw new Exception("Could not recognize the task type");
            }

            return type;
        }

        private static BaseTaskControl CreateTaskControl(Type taskType, object value)
        {
            if (value == null)
            {
                return Activator.CreateInstance(taskType) as BaseTaskControl;
            }

            return Activator.CreateInstance(taskType, value) as BaseTaskControl;
        }
        
        public static void DisableAddedTasks(ItemsControl itemsControl)
        {
            foreach (var item in itemsControl.Items)
            {
                if (!(item is Button))
                {
                    continue;
                }

                if (AppData.GetValue((item as Button).Name) != null)
                {
                    (item as Button).IsEnabled = false;
                }
            }
        }

        public static string[] TaskControls =
        {
            // Plan
            "WeddingBudget", "WeddingStyle", "RegistryPlace", "ReligiousPlace",
            "DocumentsRequired", "Restaurant", "RestaurantFood", "BestManMaidOfHonor",
            "BridesmaidsGroomsmen", "Decoration", "FreshFlowers", "MusicLayout",
            "Photographer", "BrideAccessories", "BrideClothes", "GroomAccessories",
            "GroomClothes", "BestManMaidOfHonorAccessories", "BestManMaidOfHonorClothes", "BridesmaidsGroomsmenAccessories",
            "BridesmaidsGroomsmenClothes", "HoneymoonDestination", "GuestsList", "ForeignGuestsAccomodation",
            "HairdresserMakeupArtist", "AccomodationPlaces",
            // Purchase
            "PurchaseInvitations", "PurchaseBrideAccessories", "PurchaseBrideClothes", "PurchaseGroomAccessories",
            "PurchaseGroomClothes", "PurchaseBMMOHAccessories", "PurchaseBMMOHClothes", "PurchaseBAGAccessories",
            "PurchaseBAGClothes", "PurchaseRestaurantFood", "PurchaseFreshFlowers", "PurchaseRings", "PurchaseCake",
            // Bookings
            "BookMusicLayout", "BookPhotographer", "BookHoneymoonDestination", "BookGuestsAccomodation",
            "BookHairdresserMakeupArtistAppointments",
            "SendInvitations", "RestaurantAccomodationPlan"
        };
    }
}
