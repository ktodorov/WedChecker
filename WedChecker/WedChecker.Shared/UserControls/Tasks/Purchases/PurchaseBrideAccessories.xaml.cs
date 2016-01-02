using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseBrideAccessories : BaseTaskControl
    {
        private List<string> _storedAccessories;

        public List<string> StoredAccessories
        {
            get
            {
                if (_storedAccessories == null)
                {
                    _storedAccessories = AppData.GetGlobalValues(TaskData.Tasks.BrideAccessories.ToString());
                }

                return _storedAccessories;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Bride accessories";
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

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.PurchaseBrideAccessories.ToString();
            }
        }

        public PurchaseBrideAccessories()
        {
            this.InitializeComponent();

            foreach (var accessory in StoredAccessories)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddToggle(toggle);
            }
        }

        public PurchaseBrideAccessories(Dictionary<string, bool> accessories)
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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskCode);

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
            await AppData.InsertGlobalValue(TaskCode);
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
