using System;
using System.Collections.Generic;
using System.Text;
using WedChecker.UserControls.Tasks;
using WedChecker.UserControls.Tasks.Planings;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            BaseTaskControl taskControl = null;
            switch (populatedControl.Key)
            {
                case "WeddingBudget":
                    taskControl = CreateWeddingBudgetControl(value);
                    break;
                case "WeddingStyle":
                    taskControl = CreateWeddingStyleControl(value);
                    break;
                case "RegistryPlace":
                    taskControl = CreateRegistryPlaceControl(value);
                    break;
                case "ReligiousPlace":
                    taskControl = CreateReligiousPlaceControl(value);
                    break;
                case "DocumentsRequired":
                    taskControl = CreateDocumentsRequiredControl(value);
                    break;
                case "Restaurant":
                    taskControl = CreateRestaurantControl(value);
                    break;

                case "RestaurantFood":
                    taskControl = CreateRestaurantFoodControl(value);
                    break;

                case "BestMan_MaidOfHonour":

                    break;

                case "BridesmaidsGroomsmen":

                    break;

                case "Decoration":

                    break;

                case "FreshFlowers":

                    break;

                case "MusicLayout":

                    break;

                case "Photographer":

                    break;

                case "BrideAccessories":

                    break;

                case "BrideClothes":

                    break;

                case "GroomAccessories":

                    break;

                case "GroomClothes":

                    break;

                case "BMMOHAccessories":

                    break;

                case "BMMOHClothes":

                    break;

                case "BAGAccessories":

                    break;

                case "BAGClothes":

                    break;

                case "HoneymoonDestination":

                    break;

                case "GuestsList":
                    taskControl = CreateGuestsListControl(value);
                    break;

                case "ForeignGuestsAccomodation":

                    break;

                case "HairdresserMakeupArtist":

                    break;

                case "Invitations":

                    break;

                case "PurchaseBrideAccessories":

                    break;

                case "PurchaseBrideClothes":

                    break;

                case "PurchaseGroomAccessories":

                    break;

                case "PurchaseGroomClothes":

                    break;

                case "PurchaseBMMOHAccessories":

                    break;

                case "PurchaseBMMOHClothes":

                    break;

                case "PurchaseBAGAccessories":

                    break;

                case "PurchaseBAGClothes":

                    break;

                case "PurchaseRestaurantFood":

                    break;

                case "PurchaseFreshFlowers":

                    break;

                case "PurchaseRings":

                    break;

                case "PurchaseCake":

                    break;

                case "BookMusicLayout":

                    break;

                case "BookPhotographer":

                    break;

                case "BookHoneymoonDestination":

                    break;

                case "BookGuestsAccomodation":

                    break;

                case "BookHairdresserMakeupArtistAppointments":

                    break;

                case "SendInvitations":

                    break;

                case "RestaurantAccomodationPlan":

                    break;

                default:
                    break;

            }

            AppData.InsertSerializableTask(taskControl);

#if WINDOWS_PHONE_APP
            var pivotStackPanel = currentPage.FindName("spPlanings") as StackPanel;
            pivotStackPanel.Children.Add(new PopulatedTask(taskControl, true));

            var lbTasks = currentPage.FindName("LbTasks") as ItemsControl;
            lbTasks.Visibility = Visibility.Collapsed;

            var appBar = currentPage.FindName("appBar") as AppBar;
            appBar.Visibility = Visibility.Visible;

            var mainPivot = currentPage.FindName("mainPivot") as Pivot;
            mainPivot.Visibility = Visibility.Visible;

            mainPivot.SelectedIndex = 1;
#else

#endif
        }

        public static BaseTaskControl GetTaskControlFromString(string taskControlName)
        {
            switch (taskControlName)
            {
                case "WeddingBudget":
                    return new WeddingBudget();
                case "WeddingStyle":
                    return new WeddingStyle();
                case "RegistryPlace":
                    return new RegistryPlace();
                case "ReligiousPlace":
                    return new ReligiousPlace();
                case "DocumentsRequired":
                    return new DocumentsRequired();
                case "Restaurant":
                    return new Restaurant();
                case "RestaurantFood":
                    return new RestaurantFood();
                case "BestMan_MaidOfHonour":

                    break;

                case "BridesmaidsGroomsmen":

                    break;

                case "Decoration":

                    break;

                case "FreshFlowers":

                    break;

                case "MusicLayout":

                    break;

                case "Photographer":

                    break;

                case "BrideAccessories":

                    break;

                case "BrideClothes":

                    break;

                case "GroomAccessories":

                    break;

                case "GroomClothes":

                    break;

                case "BMMOHAccessories":

                    break;

                case "BMMOHClothes":

                    break;

                case "BAGAccessories":

                    break;

                case "BAGClothes":

                    break;

                case "HoneymoonDestination":

                    break;

                case "GuestsList":
                    return new GuestsList();
                case "ForeignGuestsAccomodation":

                    break;

                case "HairdresserMakeupArtist":

                    break;

                case "Invitations":

                    break;

                case "PurchaseBrideAccessories":

                    break;

                case "PurchaseBrideClothes":

                    break;

                case "PurchaseGroomAccessories":

                    break;

                case "PurchaseGroomClothes":

                    break;

                case "PurchaseBMMOHAccessories":

                    break;

                case "PurchaseBMMOHClothes":

                    break;

                case "PurchaseBAGAccessories":

                    break;

                case "PurchaseBAGClothes":

                    break;

                case "PurchaseRestaurantFood":

                    break;

                case "PurchaseFreshFlowers":

                    break;

                case "PurchaseRings":

                    break;

                case "PurchaseCake":

                    break;

                case "BookMusicLayout":

                    break;

                case "BookPhotographer":

                    break;

                case "BookHoneymoonDestination":

                    break;

                case "BookGuestsAccomodation":

                    break;

                case "BookHairdresserMakeupArtistAppointments":

                    break;

                case "SendInvitations":

                    break;

                case "RestaurantAccomodationPlan":

                    break;
            }

            return null;
        }

        private static BaseTaskControl CreateWeddingBudgetControl(object value)
        {
            var intValue = Convert.ToInt32(value);
            if (intValue == -1)
            {
                return new WeddingBudget();
            }

            var weddingBudget = new WeddingBudget(intValue);
            return weddingBudget;
        }

        private static BaseTaskControl CreateWeddingStyleControl(object value)
        {
            var stringValue = value.ToString();
            if (stringValue == "-1")
            {
                return new WeddingStyle();
            }

            var weddingStyle = new WeddingStyle(stringValue);
            return weddingStyle;
        }

        private static BaseTaskControl CreateRegistryPlaceControl(object value)
        {
            var locationValue = value.ToString();
            if (locationValue == "-1")
            {
                return new RegistryPlace();
            }

            var registryPlace = new RegistryPlace(locationValue);
            return registryPlace;
        }


        private static BaseTaskControl CreateReligiousPlaceControl(object value)
        {
            var locationValue = value.ToString();
            if (locationValue == "-1")
            {
                return new ReligiousPlace();
            }

            var religiousPlace = new ReligiousPlace(locationValue);
            return religiousPlace;
        }

        private static BaseTaskControl CreateDocumentsRequiredControl(object value)
        {
            var documentsList = value as Dictionary<int, string>;
            if (documentsList == null)
            {
                return new DocumentsRequired();
            }

            var documentsRequired = new DocumentsRequired(documentsList);
            return documentsRequired;
        }

        private static BaseTaskControl CreateRestaurantControl(object value)
        {
            var parameters = value as Dictionary<string, string>;
            if (parameters == null)
            {
                return new Restaurant();
            }

            var restaurant = new Restaurant(parameters);
            return restaurant;
        }

        private static BaseTaskControl CreateRestaurantFoodControl(object value)
        {
            var dishesList = value as Dictionary<int, string>;
            if (dishesList == null)
            {
                return new RestaurantFood();
            }

            var restaurantFood = new RestaurantFood(dishesList);
            return restaurantFood;
        }

        private static BaseTaskControl CreateGuestsListControl(object value)
        {
            var contactsValue = value as List<Contact>;
            if (contactsValue == null)
            {
                return new GuestsList();
            }

            var guestsList = new GuestsList(contactsValue);
            return guestsList;
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
            "DocumentsRequired", "Restaurant", "RestaurantFood", "BestMan_MaidOfHonour",
            "BridesmaidsGroomsmen", "Decoration", "FreshFlowers", "MusicLayout",
            "Photographer", "BrideAccessories", "BrideClothes", "GroomAccessories",
            "GroomClothes", "BMMOHAccessories", "BMMOHClothes", "BAGAccessories",
            "BAGClothes", "HoneymoonDestination", "GuestsList", "ForeignGuestsAccomodation",
            "HairdresserMakeupArtist", "Invitations",
            // Purchase
            "PurchaseBrideAccessories", "PurchaseBrideClothes", "PurchaseGroomAccessories", "PurchaseGroomClothes", 
            "PurchaseBMMOHAccessories", "PurchaseBMMOHClothes", "PurchaseBAGAccessories", "PurchaseBAGClothes",
            "PurchaseRestaurantFood", "PurchaseFreshFlowers", "PurchaseRings", "PurchaseCake",
            // Bookings
            "BookMusicLayout", "BookPhotographer", "BookHoneymoonDestination", "BookGuestsAccomodation",
            "BookHairdresserMakeupArtistAppointments",
            "SendInvitations", "RestaurantAccomodationPlan"
        };
    }
}
