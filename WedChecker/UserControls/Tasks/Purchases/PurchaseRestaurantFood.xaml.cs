using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

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
                    _storedFood = AppData.GetStorage("RestaurantFood") as List<string>;
                }

                if (_storedFood == null || !_storedFood.Any())
                {
                    throw new WedCheckerInvalidDataException("You must first add restaurant food in order to mark it purchased after that!");
                }

                return _storedFood;
            }
        }

        public static new string TaskName
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

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have purchased so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.PurchaseRestaurantFood.ToString();
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

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            foreach (var toggle in toggles)
            {
                toggle.Serialize(writer);
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var toggle = new ToggleControl();
                toggle.Deserialize(reader);

                AddToggle(toggle);
            }
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
