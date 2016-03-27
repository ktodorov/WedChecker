using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseBAGAccessories : BaseTaskControl
    {
        private List<string> _storedBridesmaidsAccessories;
        public List<string> StoredBridesmaidsAccessories
        {
            get
            {
                if (_storedBridesmaidsAccessories == null)
                {
                    _storedBridesmaidsAccessories = AppData.GetStorage("BridesmaidsAccessories") as List<string>;
                }

                if ((_storedBridesmaidsAccessories == null || !_storedBridesmaidsAccessories.Any()) && (_storedGroomsmenAccessories == null || !_storedGroomsmenAccessories.Any()))
                {
                    throw new WedCheckerInvalidDataException("You must first add accessories for the bridesmaids or the groomsmen in order to mark them purchased after that!");
                }

                if (_storedBridesmaidsAccessories == null)
                {
                    _storedBridesmaidsAccessories = new List<string>();
                }

                return _storedBridesmaidsAccessories;
            }
        }

        private List<string> _storedGroomsmenAccessories;
        public List<string> StoredGroomsmenAccessories
        {
            get
            {
                if (_storedGroomsmenAccessories == null)
                {
                    _storedGroomsmenAccessories = AppData.GetStorage("GroomsmenAccessories") as List<string>;
                }

                if ((_storedBridesmaidsAccessories == null || !_storedBridesmaidsAccessories.Any()) && (_storedGroomsmenAccessories == null || !_storedGroomsmenAccessories.Any()))
                {
                    throw new WedCheckerInvalidDataException("You must first add accessories for the bridesmaids or the groomsmen in order to mark them purchased after that!");
                }

                if (_storedGroomsmenAccessories == null)
                {
                    _storedGroomsmenAccessories = new List<string>();
                }

                return _storedGroomsmenAccessories;
            }
        }

        public static new string TaskName
        {
            get
            {
                return "Bridesmaids and groomsmen accessories";
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
                return TaskData.Tasks.PurchaseBAGAccessories.ToString();
            }
        }

        public PurchaseBAGAccessories()
        {
            this.InitializeComponent();

            foreach (var accessory in StoredBridesmaidsAccessories)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddBridesmaidsToggle(toggle);
            }

            foreach (var accessory in StoredGroomsmenAccessories)
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
            if (!StoredBridesmaidsAccessories.Contains(toggle.Title))
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
            if (!StoredGroomsmenAccessories.Contains(toggle.Title))
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
