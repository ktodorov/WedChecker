using System;
using System.Collections.Generic;
using System.Text;
using WedChecker.UserControls.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Reflection;
using System.Linq;
using WedChecker.UserControls;

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

            InsertTaskControl(currentPage, type, taskControl, true);
        }

        public static void InsertTaskControl(Page currentPage, Type type, BaseTaskControl taskControl, bool isNew = true)
        {
#if WINDOWS_PHONE_APP
            var svMain = currentPage.FindName("svMain") as ScrollViewer;
            svMain.Visibility = Visibility.Collapsed;

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

#if WINDOWS_PHONE_APP
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

#endif

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
        
        public static void DisableAddedTasks(GridView itemsControl)
        {
            foreach (var item in itemsControl.Items)
            {
                if (!(item is TaskTileControl))
                {
                    continue;
                }

                if (AppData.GetValue((item as TaskTileControl).Name) != null)
                {
                    (item as TaskTileControl).IsEnabled = false;
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

        public static List<TaskListItem> LoadPlanningTaskItems()
        {
            var planItems = new List<TaskListItem>();

            planItems.Add(new TaskListItem { Title = "Budget for your wedding", TaskName = Tasks.WeddingBudget.ToString() });
            planItems.Add(new TaskListItem { Title = "Wedding style and colors", TaskName = Tasks.WeddingStyle.ToString() });
            planItems.Add(new TaskListItem { Title = "Registry marriage place", TaskName = Tasks.RegistryPlace.ToString() });
            planItems.Add(new TaskListItem { Title = "Religious marriage place", TaskName = Tasks.ReligiousPlace.ToString() });
            planItems.Add(new TaskListItem { Title = "Documents required for marriage", TaskName = Tasks.DocumentsRequired.ToString() });
            planItems.Add(new TaskListItem { Title = "Restaurant", TaskName = Tasks.Restaurant.ToString() });
            planItems.Add(new TaskListItem { Title = "Restaurant food", TaskName = Tasks.RestaurantFood.ToString() });
            planItems.Add(new TaskListItem { Title = "Best man and maid of honor", TaskName = Tasks.BestManMaidOfHonor.ToString() });
            planItems.Add(new TaskListItem { Title = "Bridesmaids and groomsmen", TaskName = Tasks.BridesmaidsGroomsmen.ToString() });
            planItems.Add(new TaskListItem { Title = "Decoration", TaskName = Tasks.Decoration.ToString() });
            planItems.Add(new TaskListItem { Title = "Fresh flowers", TaskName = Tasks.FreshFlowers.ToString() });
            planItems.Add(new TaskListItem { Title = "Music layout", TaskName = Tasks.MusicLayout.ToString() });
            planItems.Add(new TaskListItem { Title = "Photographer", TaskName = Tasks.Photographer.ToString() });
            planItems.Add(new TaskListItem { Title = "Bride accessories", TaskName = Tasks.BrideAccessories.ToString() });
            planItems.Add(new TaskListItem { Title = "Bride clothes", TaskName = Tasks.BrideClothes.ToString() });
            planItems.Add(new TaskListItem { Title = "Groom accessories", TaskName = Tasks.GroomAccessories.ToString() });
            planItems.Add(new TaskListItem { Title = "Groom clothes", TaskName = Tasks.GroomClothes.ToString() });
            planItems.Add(new TaskListItem { Title = "Best man and maid of honor accessories", TaskName = Tasks.BestManMaidOfHonorAccessories.ToString() });
            planItems.Add(new TaskListItem { Title = "Best man and maid of honor clothes", TaskName = Tasks.BestManMaidOfHonorClothes.ToString() });
            planItems.Add(new TaskListItem { Title = "Bridesmaids and groomsmen accessories", TaskName = Tasks.BridesmaidsGroomsmenAccessories.ToString() });
            planItems.Add(new TaskListItem { Title = "Bridesmaids and groomsmen clothes", TaskName = Tasks.BridesmaidsGroomsmenClothes.ToString() });
            planItems.Add(new TaskListItem { Title = "Honeymoon destination", TaskName = Tasks.HoneymoonDestination.ToString() });
            planItems.Add(new TaskListItem { Title = "Guests list", TaskName = Tasks.GuestsList.ToString() });
            planItems.Add(new TaskListItem { Title = "Accomodation places", TaskName = Tasks.AccomodationPlaces.ToString() });
            planItems.Add(new TaskListItem { Title = "Foreign guests accomodation", TaskName = Tasks.ForeignGuestsAccomodation.ToString() });
            planItems.Add(new TaskListItem { Title = "Hairdresser and makeup artist", TaskName = Tasks.HairdresserMakeupArtist.ToString() });
            planItems.Add(new TaskListItem { Title = "Restaurant accomodation plan", TaskName = Tasks.RestaurantAccomodationPlan.ToString() });

            planItems.OrderBy(i => i.Title).ToList();

            return planItems;
        }

        public static List<TaskListItem> LoadPurchasingTaskItems()
        {

            var purchaseItems = new List<TaskListItem>();

            purchaseItems.Add(new TaskListItem { Title = "Invitations", TaskName = Tasks.PurchaseInvitations.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Bride accessories", TaskName = Tasks.PurchaseBrideAccessories.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Bride clothes", TaskName = Tasks.PurchaseBrideClothes.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Groom accessories", TaskName = Tasks.PurchaseGroomAccessories.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Groom clothes", TaskName = Tasks.PurchaseGroomClothes.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Best man and maid of honor accessories", TaskName = Tasks.PurchaseBMMOHAccessories.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Best man and maid of honor clothes", TaskName = Tasks.PurchaseBMMOHClothes.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Bridesmaids and groomsmen accessories", TaskName = Tasks.PurchaseBAGAccessories.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Bridesmaids and groomsmen clothes", TaskName = Tasks.PurchaseBAGClothes.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Restaurant food", TaskName = Tasks.PurchaseRestaurantFood.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Fresh flowers", TaskName = Tasks.PurchaseFreshFlowers.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Rings", TaskName = Tasks.PurchaseRings.ToString() });
            purchaseItems.Add(new TaskListItem { Title = "Cake", TaskName = Tasks.PurchaseCake.ToString() });

            purchaseItems.OrderBy(i => i.Title).ToList();

            return purchaseItems;
        }

        public static List<TaskListItem> LoadBookingTaskItems()
        {
            var bookItems = new List<TaskListItem>();

            bookItems.Add(new TaskListItem { Title = "Music layout", TaskName = Tasks.BookMusicLayout.ToString() });
            bookItems.Add(new TaskListItem { Title = "Photographer", TaskName = Tasks.BookPhotographer.ToString() });
            bookItems.Add(new TaskListItem { Title = "Honeymoon destination", TaskName = Tasks.BookHoneymoonDestination.ToString() });
            bookItems.Add(new TaskListItem { Title = "Guests accomodation", TaskName = Tasks.BookGuestsAccomodation.ToString() });
            bookItems.Add(new TaskListItem { Title = "Hairdresser and makeup artist appointments", TaskName = Tasks.BookHairdresserMakeupArtistAppointments.ToString() });
            bookItems.Add(new TaskListItem { Title = "Send invitations", TaskName = Tasks.SendInvitations.ToString() });

            bookItems.OrderBy(i => i.Title).ToList();

            return bookItems;
        }
    }
}
