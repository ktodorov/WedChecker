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
    public sealed partial class PurchaseGroomClothes : BaseTaskControl
    {
        private List<string> _storedClothes;

        public List<string> StoredClothes
        {
            get
            {
                if (_storedClothes == null)
                {
                    _storedClothes = AppData.GetGlobalValues(TaskData.Tasks.GroomClothes.ToString());
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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.PurchaseGroomClothes.ToString());

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
            await AppData.InsertGlobalValue(TaskData.Tasks.PurchaseGroomClothes.ToString());
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
