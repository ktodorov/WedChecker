using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
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

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseRestaurantFood : BaseTaskControl
    {
        private List<string> _storedFood;

        public List<string> StoredFood
        {
            get
            {
                if (_storedFood == null)
                {
                    _storedFood = AppData.GetGlobalValues(TaskData.Tasks.RestaurantFood.ToString());
                }

                return _storedFood;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Restaurant food";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "What have you purchased so far?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have purchased so far";
            }
        }

        public PurchaseRestaurantFood()
        {
            this.InitializeComponent();

            foreach (var accessory in StoredFood)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddToggle(toggle);
            }
        }

        public PurchaseRestaurantFood(Dictionary<string, bool> food)
        {
            this.InitializeComponent();

            foreach (var accessory in food)
            {
                var toggle = new ToggleControl(accessory.Key, accessory.Value);
                AddToggle(toggle);
            }

            var remainingStoredFood = StoredFood.Where(sa => !food.Any(a => a.Key == sa));
            foreach (var accessory in remainingStoredFood)
            {
                var toggle = new ToggleControl(accessory);
                AddToggle(toggle);
            }
        }

        public override void DisplayValues()
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.PurchaseRestaurantFood.ToString());

            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            foreach (var toggle in toggles)
            {
                toggle.Serialize(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var toggle = new ToggleControl();
                toggle.Deserialize(reader);

                AddToggle(toggle);
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.PurchaseRestaurantFood.ToString());
        }

        private void AddToggle(ToggleControl toggle)
        {
            if (!StoredFood.Contains(toggle.Title))
            {
                return;
            }

            var allToggles = mainPanel.Children.OfType<ToggleControl>();

            if (allToggles.Any(t => t.Title == toggle.Title))
            {
                mainPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                return;
            }

            mainPanel.Children.Add(toggle);
        }
    }
}
