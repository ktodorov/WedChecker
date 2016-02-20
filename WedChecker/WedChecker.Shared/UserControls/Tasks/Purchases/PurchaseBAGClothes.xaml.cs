using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseBAGClothes : BaseTaskControl
    {
        private List<string> _storedBridesmaidsClothes;
        public List<string> StoredBridesmaidsClothes
        {
            get
            {
                if (_storedBridesmaidsClothes == null)
                {
                    _storedBridesmaidsClothes = AppData.GetStorage("BridesmaidsClothes") as List<string>;
                }

                if ((_storedBridesmaidsClothes == null || !_storedBridesmaidsClothes.Any()) && (_storedGroomsmenClothes == null || !_storedGroomsmenClothes.Any()))
                {
                    throw new WedCheckerInvalidDataException("You must first add clothes for the bridesmaids or the groomsmen in order to mark them purchased after that!");
                }

                if (_storedBridesmaidsClothes == null)
                {
                    _storedBridesmaidsClothes = new List<string>();
                }

                return _storedBridesmaidsClothes;
            }
        }

        private List<string> _storedGroomsmenClothes;
        public List<string> StoredGroomsmenClothes
        {
            get
            {
                if (_storedGroomsmenClothes == null)
                {
                    _storedGroomsmenClothes = AppData.GetStorage("GroomsmenClothes") as List<string>;
                }

                if ((_storedBridesmaidsClothes == null || !_storedBridesmaidsClothes.Any()) && (_storedGroomsmenClothes == null || !_storedGroomsmenClothes.Any()))
                {
                    throw new WedCheckerInvalidDataException("You must first add clothes for the bridesmaids or the groomsmen in order to mark them purchased after that!");
                }

                if (_storedGroomsmenClothes == null)
                {
                    _storedGroomsmenClothes = new List<string>();
                }

                return _storedGroomsmenClothes;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Bridesmaids and groomsmen clothes";
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
                return TaskData.Tasks.PurchaseBAGClothes.ToString();
            }
        }

        public PurchaseBAGClothes()
        {
            this.InitializeComponent();

            foreach (var accessory in StoredBridesmaidsClothes)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddBridesmaidsToggle(toggle);
            }

            foreach (var accessory in StoredGroomsmenClothes)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddGroomsmenToggle(toggle);
            }
        }

        public override void DisplayValues()
        {
            var bridesmaidsToggles = bridesmaidsPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in bridesmaidsToggles)
            {
                toggle.DisplayValues();
            }

            var groomsmenToggles = groomsmenPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in groomsmenToggles)
            {
                toggle.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var bridesmaidsToggles = bridesmaidsPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in bridesmaidsToggles)
            {
                toggle.EditValues();
            }

            var groomsmenToggles = groomsmenPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in groomsmenToggles)
            {
                toggle.EditValues();
            }
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var bridesmaidsToggles = bridesmaidsPanel.Children.OfType<ToggleControl>();
            writer.Write("Bridesmaids");
            writer.Write(bridesmaidsToggles.Count());
            foreach (var toggle in bridesmaidsToggles)
            {
                toggle.Serialize(writer);
            }

            var groomsmenToggles = groomsmenPanel.Children.OfType<ToggleControl>();
            writer.Write("Groomsmen");
            writer.Write(groomsmenToggles.Count());
            foreach (var toggle in groomsmenToggles)
            {
                toggle.Serialize(writer);
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            for (int i = 0; i < 2; i++)
            {
                var type = reader.ReadString();
                var count = reader.ReadInt32();

                
                for (var j = 0; j < count; j++)
                {
                    var toggle = new ToggleControl();
                    toggle.Deserialize(reader);

                    if (type == "Bridesmaids")
                    {
                        AddBridesmaidsToggle(toggle);
                    }
                    else if (type == "Groomsmen")
                    {
                        AddGroomsmenToggle(toggle);
                    }
                }
            }
        }

        private void AddBridesmaidsToggle(ToggleControl toggle)
        {
            if (!StoredBridesmaidsClothes.Contains(toggle.Title))
            {
                return;
            }

            var allToggles = bridesmaidsPanel.Children.OfType<ToggleControl>();

            if (allToggles.Any(t => t.Title == toggle.Title))
            {
                bridesmaidsPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                return;
            }

            bridesmaidsPanel.Children.Add(toggle);
        }

        private void AddGroomsmenToggle(ToggleControl toggle)
        {
            if (!StoredGroomsmenClothes.Contains(toggle.Title))
            {
                return;
            }

            var allToggles = groomsmenPanel.Children.OfType<ToggleControl>();

            if (allToggles.Any(t => t.Title == toggle.Title))
            {
                groomsmenPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                return;
            }

            groomsmenPanel.Children.Add(toggle);
        }
    }
}
