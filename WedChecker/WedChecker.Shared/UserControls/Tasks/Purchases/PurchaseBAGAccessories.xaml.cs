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
    public sealed partial class PurchaseBAGAccessories : BaseTaskControl
    {
        private List<string> _storedAccessories;
        public List<string> StoredAccessories
        {
            get
            {
                if (_storedAccessories == null)
                {
                    _storedAccessories = AppData.GetGlobalValues(TaskData.Tasks.BridesmaidsGroomsmenAccessories.ToString());
                }

                return _storedAccessories;
            }
        }

        private List<string> _storedBridesmaidsAccessories;
        public List<string> StoredBridesmaidsAccessories
        {
            get
            {
                if (_storedBridesmaidsAccessories == null)
                {
                    var startIndex = StoredAccessories.IndexOf($"StartBridesmaidsAccessories{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredAccessories.IndexOf($"EndBridesmaidsAccessories{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedBridesmaidsAccessories = new List<string>();
                    _storedBridesmaidsAccessories.AddRange(StoredAccessories.Skip(startIndex).Take(endIndex));
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
                    var startIndex = StoredAccessories.IndexOf($"StartGroomsmenAccessories{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredAccessories.IndexOf($"EndGroomsmenAccessories{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedGroomsmenAccessories = new List<string>();
                    _storedGroomsmenAccessories.AddRange(StoredAccessories.Skip(startIndex).Take(endIndex));
                }

                return _storedGroomsmenAccessories;
            }
        }

        public override string TaskName
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

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have purchased so far";
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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.PurchaseBAGAccessories.ToString());

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

        public override void Deserialize(BinaryReader reader)
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

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.PurchaseBAGAccessories.ToString());
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
