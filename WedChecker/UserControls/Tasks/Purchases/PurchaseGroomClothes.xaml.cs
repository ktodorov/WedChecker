using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseGroomClothes : BaseTaskControl
    {
        private List<string> _storedClothes;

        public List<string> StoredClothes
        {
            get
            {
                if (_storedClothes == null)
                {
                    _storedClothes = AppData.GetStorage("GroomClothes") as List<string>;
                }

                if (_storedClothes == null || !_storedClothes.Any())
                {
                    throw new WedCheckerInvalidDataException("You must first add groom clothes in order to mark them purchased after that!");
                }

                return _storedClothes;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Groom clothes";
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
                return TaskData.Tasks.PurchaseGroomClothes.ToString();
            }
        }

        public PurchaseGroomClothes()
        {
            this.InitializeComponent();

            foreach (var cloth in StoredClothes)
            {
                var toggle = new ToggleControl();
                toggle.Title = cloth;
                AddToggle(toggle);
            }
        }

        public PurchaseGroomClothes(Dictionary<string, bool> clothes)
        {
            this.InitializeComponent();

            foreach (var cloth in clothes)
            {
                var toggle = new ToggleControl(cloth.Key, cloth.Value);
                AddToggle(toggle);
            }

            var remainingStoredClothes = StoredClothes.Where(sa => !clothes.Any(a => a.Key == sa));
            foreach (var cloth in remainingStoredClothes)
            {
                var toggle = new ToggleControl(cloth);
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
            if (!StoredClothes.Contains(toggle.Title))
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
