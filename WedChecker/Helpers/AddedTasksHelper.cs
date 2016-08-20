using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WedChecker.Common;
using WedChecker.UserControls.Tasks;
using WedChecker.UserControls.Tasks.Bookings;
using WedChecker.UserControls.Tasks.Plannings;
using WedChecker.UserControls.Tasks.Purchases;

namespace WedChecker.Helpers
{
    public class WedCheckerTask
    {
        public bool Added;
        public bool Priority;
        public Type TaskControlType;
        public string TaskCode;
        public TaskCategories TaskCategory;

        public WedCheckerTask(bool added, bool priority, Type taskControlType, string taskCode, TaskCategories category)
        {
            Added = added;
            Priority = priority;
            TaskControlType = taskControlType;
            TaskCode = taskCode;
            TaskCategory = category;
        }
    }

    public class AddedTasks
    {
        private List<WedCheckerTask> AddedTasksList = new List<WedCheckerTask>()
        {
            // Plans

            new WedCheckerTask(false, true, typeof(WeddingBudget), "WeddingBudget", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(WeddingStyle), "WeddingStyle", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(RegistryPlace), "RegistryPlace", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(ReligiousPlace), "ReligiousPlace", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(DocumentsRequired), "DocumentsRequired", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(Restaurant), "Restaurant", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(RestaurantFood), "RestaurantFood", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(BestManMaidOfHonor), "BestManMaidOfHonor", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(BridesmaidsGroomsmen), "BridesmaidsGroomsmen", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(Decoration), "Decoration", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(FreshFlowers), "FreshFlowers", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(MusicLayout), "MusicLayout", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(Photographer), "Photographer", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(BrideAccessories), "BrideAccessories", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(BrideClothes), "BrideClothes", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(GroomAccessories), "GroomAccessories", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(GroomClothes), "GroomClothes", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(BestManMaidOfHonorAccessories), "BestManMaidOfHonorAccessories", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(BestManMaidOfHonorClothes), "BestManMaidOfHonorClothes", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(BridesmaidsGroomsmenAccessories), "BridesmaidsGroomsmenAccessories", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(BridesmaidsGroomsmenClothes), "BridesmaidsGroomsmenClothes", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(HoneymoonDestination), "HoneymoonDestination", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(GuestsList), "GuestsList", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(ForeignGuestsAccomodation), "ForeignGuestsAccomodation", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(HairdresserMakeupArtist), "HairdresserMakeupArtist", TaskCategories.Planning),
            new WedCheckerTask(false, true,  typeof(AccomodationPlaces), "AccomodationPlaces", TaskCategories.Planning),
            new WedCheckerTask(false, false, typeof(RestaurantAccomodationPlan), "RestaurantAccomodationPlan", TaskCategories.Planning),

            // Purchase

            new WedCheckerTask(false, false, typeof(PurchaseInvitations), "PurchaseInvitations", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseBrideAccessories), "PurchaseBrideAccessories", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseBrideClothes), "PurchaseBrideClothes", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseGroomAccessories), "PurchaseGroomAccessories", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseGroomClothes), "PurchaseGroomClothes", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseBMMOHAccessories), "PurchaseBMMOHAccessories", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseBMMOHClothes), "PurchaseBMMOHClothes", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseBAGAccessories), "PurchaseBAGAccessories", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseBAGClothes), "PurchaseBAGClothes", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseRestaurantFood), "PurchaseRestaurantFood", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseFreshFlowers), "PurchaseFreshFlowers", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseRings), "PurchaseRings", TaskCategories.Purchase),
            new WedCheckerTask(false, false, typeof(PurchaseCake), "PurchaseCake", TaskCategories.Purchase),

            // Bookings

            new WedCheckerTask(false, false, typeof(BookMusicLayout), "BookMusicLayout", TaskCategories.Booking),
            new WedCheckerTask(false, false, typeof(BookPhotographer), "BookPhotographer", TaskCategories.Booking),
            new WedCheckerTask(false, false, typeof(BookHoneymoonDestination), "BookHoneymoonDestination", TaskCategories.Booking),
            new WedCheckerTask(false, false, typeof(BookGuestsAccomodation), "BookGuestsAccomodation", TaskCategories.Booking),
            new WedCheckerTask(false, false, typeof(BookHairdresserMakeupArtistAppointments), "BookHairdresserMakeupArtistAppointments", TaskCategories.Booking),
            new WedCheckerTask(false, false, typeof(SendInvitations), "SendInvitations", TaskCategories.Booking)
        };

        public AddedTasks()
        {
            var taskCodes = AddedTasksList.Select(t => t.TaskCode).ToList();

            foreach (var taskCode in taskCodes)
            {
                var added = AppData.GetRoamingSetting<bool?>(taskCode);

                if (added.HasValue && added.Value)
                {
                    InsertTask(taskCode, false);
                }
            }
        }

        public BaseTaskControl GetTaskByTaskCode(string taskCode)
        {
            var task = AddedTasksList.FirstOrDefault(t => t.TaskCode == taskCode);

            return Activator.CreateInstance(task.TaskControlType) as BaseTaskControl;
        }

        public void InsertTask(string taskCode, bool saveToRoaming = true)
        {
            var task = AddedTasksList.FirstOrDefault(t => t.TaskCode == taskCode);

            if (saveToRoaming)
            {
                AppData.InsertRoamingSetting(task.TaskCode, true);
            }

            task.Added = true;
        }

        public void DeleteTask(string taskCode)
        {
            var task = AddedTasksList.FirstOrDefault(t => t.TaskCode == taskCode);

            AppData.InsertRoamingSetting(task.TaskCode, false);

            task.Added = false;
        }

        public List<string> GetAllPriorityTasks(TaskCategories? category = null, bool added = true)
        {
            var tasksQuery = AddedTasksList.Where(t => t.Priority == true);
            if (added)
            {
                tasksQuery = tasksQuery.Where(t => t.Added == true);
            }
            if (category.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.TaskCategory == category.Value);
            }
            var tasks = tasksQuery.Select(t => t.TaskCode).ToList();

            return tasks;
        }

        public List<string> GetAllNonPriorityTasks(TaskCategories? category = null, bool added = true)
        {
            var tasksQuery = AddedTasksList.Where(t => t.Priority == false);
            if (added)
            {
                tasksQuery = tasksQuery.Where(t => t.Added == true);
            }
            if (category.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.TaskCategory == category.Value);
            }
            var tasks = tasksQuery.Where(t => t.Priority == false).Select(t => t.TaskCode).ToList();

            return tasks;
        }

        public List<string> GetAllTasks(TaskCategories? category = null, bool added = true)
        {
            var tasksQuery = AddedTasksList;
            if (added)
            {
                tasksQuery = tasksQuery.Where(t => t.Added == true).ToList();
            }
            if (category.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.TaskCategory == category.Value).ToList();
            }
            var tasks = tasksQuery.Select(t => t.TaskCode).ToList();

            return tasks;
        }
    }
}
