using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WedChecker.Common;
using WedChecker.UserControls.Tasks;
using WedChecker.UserControls.Tasks.Bookings;
using WedChecker.UserControls.Tasks.Planings;
using WedChecker.UserControls.Tasks.Purchases;

namespace WedChecker.Helpers
{
    public class WedCheckerTask
    {
        public bool Added;
        public bool Priority;
        public Type TaskControlType;
        public string TaskCode;

        public WedCheckerTask(bool added, bool priority, Type taskControlType, string taskCode)
        {
            Added = added;
            Priority = priority;
            TaskControlType = taskControlType;
            TaskCode = taskCode;
        }
    }

    public class AddedTasks
    {
        private List<WedCheckerTask> AddedTasksList = new List<WedCheckerTask>()
        {
            // Plans

            new WedCheckerTask(false, false, typeof(WeddingBudget), "WeddingBudget"),
            new WedCheckerTask(false, false, typeof(WeddingStyle), "WeddingStyle"),
            new WedCheckerTask(false, false, typeof(RegistryPlace), "RegistryPlace"),
            new WedCheckerTask(false, false, typeof(ReligiousPlace), "ReligiousPlace"),
            new WedCheckerTask(false, false, typeof(DocumentsRequired), "DocumentsRequired"),
            new WedCheckerTask(false, false, typeof(Restaurant), "Restaurant"),
            new WedCheckerTask(false, true,  typeof(RestaurantFood), "RestaurantFood"),
            new WedCheckerTask(false, false, typeof(BestManMaidOfHonor), "BestManMaidOfHonor"),
            new WedCheckerTask(false, false, typeof(BridesmaidsGroomsmen), "BridesmaidsGroomsmen"),
            new WedCheckerTask(false, false, typeof(Decoration), "Decoration"),
            new WedCheckerTask(false, false, typeof(FreshFlowers), "FreshFlowers"),
            new WedCheckerTask(false, false, typeof(MusicLayout), "MusicLayout"),
            new WedCheckerTask(false, false, typeof(Photographer), "Photographer"),
            new WedCheckerTask(false, true,  typeof(BrideAccessories), "BrideAccessories"),
            new WedCheckerTask(false, true,  typeof(BrideClothes), "BrideClothes"),
            new WedCheckerTask(false, true,  typeof(GroomAccessories), "GroomAccessories"),
            new WedCheckerTask(false, true,  typeof(GroomClothes), "GroomClothes"),
            new WedCheckerTask(false, true,  typeof(BestManMaidOfHonorAccessories), "BestManMaidOfHonorAccessories"),
            new WedCheckerTask(false, true,  typeof(BestManMaidOfHonorClothes), "BestManMaidOfHonorClothes"),
            new WedCheckerTask(false, true,  typeof(BridesmaidsGroomsmenAccessories), "BridesmaidsGroomsmenAccessories"),
            new WedCheckerTask(false, true,  typeof(BridesmaidsGroomsmenClothes), "BridesmaidsGroomsmenClothes"),
            new WedCheckerTask(false, false, typeof(HoneymoonDestination), "HoneymoonDestination"),
            new WedCheckerTask(false, true,  typeof(GuestsList), "GuestsList"),
            new WedCheckerTask(false, false, typeof(ForeignGuestsAccomodation), "ForeignGuestsAccomodation"),
            new WedCheckerTask(false, false, typeof(HairdresserMakeupArtist), "HairdresserMakeupArtist"),
            new WedCheckerTask(false, true,  typeof(AccomodationPlaces), "AccomodationPlaces"),

            // Purchase

            new WedCheckerTask(false, false, typeof(PurchaseInvitations), "PurchaseInvitations"),
            new WedCheckerTask(false, false, typeof(PurchaseBrideAccessories), "PurchaseBrideAccessories"),
            new WedCheckerTask(false, false, typeof(PurchaseBrideClothes), "PurchaseBrideClothes"),
            new WedCheckerTask(false, false, typeof(PurchaseGroomAccessories), "PurchaseGroomAccessories"),
            new WedCheckerTask(false, false, typeof(PurchaseGroomClothes), "PurchaseGroomClothes"),
            new WedCheckerTask(false, false, typeof(PurchaseBMMOHAccessories), "PurchaseBMMOHAccessories"),
            new WedCheckerTask(false, false, typeof(PurchaseBMMOHClothes), "PurchaseBMMOHClothes"),
            new WedCheckerTask(false, false, typeof(PurchaseBAGAccessories), "PurchaseBAGAccessories"),
            new WedCheckerTask(false, false, typeof(PurchaseBAGClothes), "PurchaseBAGClothes"),
            new WedCheckerTask(false, false, typeof(PurchaseRestaurantFood), "PurchaseRestaurantFood"),
            new WedCheckerTask(false, false, typeof(PurchaseFreshFlowers), "PurchaseFreshFlowers"),
            new WedCheckerTask(false, false, typeof(PurchaseRings), "PurchaseRings"),
            new WedCheckerTask(false, false, typeof(PurchaseCake), "PurchaseCake"),

            // Bookings

            new WedCheckerTask(false, false, typeof(BookMusicLayout), "BookMusicLayout"),
            new WedCheckerTask(false, false, typeof(BookPhotographer), "BookPhotographer"),
            new WedCheckerTask(false, false, typeof(BookHoneymoonDestination), "BookHoneymoonDestination"),
            new WedCheckerTask(false, false, typeof(BookGuestsAccomodation), "BookGuestsAccomodation"),
            new WedCheckerTask(false, false, typeof(BookHairdresserMakeupArtistAppointments), "BookHairdresserMakeupArtistAppointments"),
            new WedCheckerTask(false, false, typeof(SendInvitations), "SendInvitations"),
            new WedCheckerTask(false, false, typeof(RestaurantAccomodationPlan), "RestaurantAccomodationPlan")
        };

        public AddedTasks()
        {
            var taskCodes = AddedTasksList.Select(t => t.TaskCode).ToList();

            foreach (var taskCode in taskCodes)
            {
                var added = AppData.GetRoamingSetting<bool?>(taskCode);

                if (added.HasValue && added.Value)
                {
                    InsertTask(taskCode);
                }
            }
        }

        public BaseTaskControl GetTaskByTaskCode(string taskCode)
        {
            var task = AddedTasksList.FirstOrDefault(t => t.TaskCode == taskCode);

            return Activator.CreateInstance(task.TaskControlType) as BaseTaskControl;
        }

        public void InsertTask(string taskCode)
        {
            var task = AddedTasksList.FirstOrDefault(t => t.TaskCode == taskCode);

            AppData.InsertRoamingSetting(task.TaskCode, true);

            task.Added = true;
        }

        public void DeleteTask(string taskCode)
        {
            var task = AddedTasksList.FirstOrDefault(t => t.TaskCode == taskCode);

            AppData.InsertRoamingSetting(task.TaskCode, false);

            task.Added = false;
        }

        public List<string> GetAllPriorityTasks(bool added = true)
        {
            var tasksQuery = AddedTasksList.Where(t => t.Priority == true);
            if (added)
            {
                tasksQuery = tasksQuery.Where(t => t.Added == true);
            }
            var tasks = tasksQuery.Select(t => t.TaskCode).ToList();

            return tasks;
        }

        public List<string> GetAllNonPriorityTasks(bool added = true)
        {
            var tasksQuery = AddedTasksList.Where(t => t.Priority == false);
            if (added)
            {
                tasksQuery = tasksQuery.Where(t => t.Added == true);
            }
            var tasks = tasksQuery.Where(t => t.Priority == false).Select(t => t.TaskCode).ToList();

            return tasks;
        }

        public List<string> GetAllTasks(bool added = true)
        {
            var tasksQuery = AddedTasksList;
            if (added)
            {
                tasksQuery = tasksQuery.Where(t => t.Added == true).ToList();
            }
            var tasks = tasksQuery.Select(t => t.TaskCode).ToList();

            return tasks;
        }
    }
}
