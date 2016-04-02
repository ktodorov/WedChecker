using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseGroomAccessories : BaseTaskControl
    {
        private List<string> _storedAccessories;

        public List<string> StoredAccessories
        {
            get
            {
                if (_storedAccessories == null)
                {
                    _storedAccessories = AppData.GetStorage("GroomAccessories") as List<string>;
                }

                if (_storedAccessories == null || !_storedAccessories.Any())
                {
                    throw new WedCheckerInvalidDataException("You must first add groom accessories in order to mark them purchased after that!");
                }

                return _storedAccessories;
            }
        }

        public static new string TaskName
        {
            get
            {
                return "Groom accessories";
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
                return TaskData.Tasks.PurchaseGroomAccessories.ToString();
            }
        }

        public PurchaseGroomAccessories()
        {
            this.InitializeComponent();

            foreach (var accessory in StoredAccessories)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddToggle(toggle);
            }
        }

        public PurchaseGroomAccessories(Dictionary<string, bool> accessories)
        {
            this.InitializeComponent();

            foreach (var accessory in accessories)
            {
                var toggle = new ToggleControl(accessory.Key, accessory.Value);
                AddToggle(toggle);
            }

            var remainingStoredAccessories = StoredAccessories.Where(sa => !accessories.Any(a => a.Key == sa));
            foreach (var accessory in remainingStoredAccessories)
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
            if (!StoredAccessories.Contains(toggle.Title))
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
