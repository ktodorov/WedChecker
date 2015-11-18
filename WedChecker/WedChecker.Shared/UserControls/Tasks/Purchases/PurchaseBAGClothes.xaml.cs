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
    public sealed partial class PurchaseBAGClothes : BaseTaskControl
    {
        private List<string> _storedClothes;
        public List<string> StoredClothes
        {
            get
            {
                if (_storedClothes == null)
                {
                    _storedClothes = AppData.GetGlobalValues(TaskData.Tasks.BridesmaidsGroomsmenClothes.ToString());
                }

                return _storedClothes;
            }
        }

        private List<string> _storedBridesmaidsClothes;
        public List<string> StoredBridesmaidsClothes
        {
            get
            {
                if (_storedBridesmaidsClothes == null)
                {
                    var startIndex = StoredClothes.IndexOf($"StartBridesmaidsClothes{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredClothes.IndexOf($"EndBridesmaidsClothes{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedBridesmaidsClothes = new List<string>();
                    _storedBridesmaidsClothes.AddRange(StoredClothes.Skip(startIndex).Take(endIndex));
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
                    var startIndex = StoredClothes.IndexOf($"StartGroomsmenClothes{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredClothes.IndexOf($"EndGroomsmenClothes{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedGroomsmenClothes = new List<string>();
                    _storedGroomsmenClothes.AddRange(StoredClothes.Skip(startIndex).Take(endIndex));
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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.PurchaseBAGClothes.ToString());

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
            await AppData.InsertGlobalValue(TaskData.Tasks.PurchaseBAGClothes.ToString());
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
