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
    public sealed partial class PurchaseBMMOHClothes : BaseTaskControl
    {
        private List<string> _storedClothes;
        public List<string> StoredClothes
        {
            get
            {
                if (_storedClothes == null)
                {
                    _storedClothes = AppData.GetGlobalValues(TaskData.Tasks.BestManMaidOfHonorClothes.ToString());
                }

                return _storedClothes;
            }
        }

        private List<string> _storedBestManClothes;
        public List<string> StoredBestManClothes
        {
            get
            {
                if (_storedBestManClothes == null)
                {
                    var startIndex = StoredClothes.IndexOf($"StartBestManClothes{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredClothes.IndexOf($"EndBestManClothes{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedBestManClothes = new List<string>();
                    _storedBestManClothes.AddRange(StoredClothes.Skip(startIndex).Take(endIndex));
                }

                return _storedBestManClothes;
            }
        }

        private List<string> _storedMaidOfHonorClothes;
        public List<string> StoredMaidOfHonorClothes
        {
            get
            {
                if (_storedMaidOfHonorClothes == null)
                {
                    var startIndex = StoredClothes.IndexOf($"StartMaidOfHonorClothes{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredClothes.IndexOf($"EndMaidOfHonorClothes{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedMaidOfHonorClothes = new List<string>();
                    _storedMaidOfHonorClothes.AddRange(StoredClothes.Skip(startIndex).Take(endIndex));
                }

                return _storedMaidOfHonorClothes;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Best man and maid of honor clothes";
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

        public PurchaseBMMOHClothes()
        {
            this.InitializeComponent();

            foreach (var accessory in StoredBestManClothes)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddBestManToggle(toggle);
            }

            foreach (var accessory in StoredMaidOfHonorClothes)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddMaidOfHonorToggle(toggle);
            }
        }

        public override void DisplayValues()
        {
            var bestManToggles = bestManPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in bestManToggles)
            {
                toggle.DisplayValues();
            }

            var maidOfHonorToggles = maidOfHonorPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in maidOfHonorToggles)
            {
                toggle.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var bestManToggles = bestManPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in bestManToggles)
            {
                toggle.EditValues();
            }

            var maidOfHonorToggles = maidOfHonorPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in maidOfHonorToggles)
            {
                toggle.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.PurchaseBMMOHClothes.ToString());

            var bestManToggles = bestManPanel.Children.OfType<ToggleControl>();
            writer.Write("BestMan");
            writer.Write(bestManToggles.Count());
            foreach (var toggle in bestManToggles)
            {
                toggle.Serialize(writer);
            }

            var maidOfHonorToggles = maidOfHonorPanel.Children.OfType<ToggleControl>();
            writer.Write("MaidOfHonor");
            writer.Write(maidOfHonorToggles.Count());
            foreach (var toggle in maidOfHonorToggles)
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

                    if (type == "BestMan")
                    {
                        AddBestManToggle(toggle);
                    }
                    else if (type == "MaidOfHonor")
                    {
                        AddMaidOfHonorToggle(toggle);
                    }
                }
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.PurchaseBMMOHClothes.ToString());
        }

        private void AddBestManToggle(ToggleControl toggle)
        {
            if (!StoredBestManClothes.Contains(toggle.Title))
            {
                return;
            }

            var allToggles = bestManPanel.Children.OfType<ToggleControl>();

            if (allToggles.Any(t => t.Title == toggle.Title))
            {
                bestManPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                return;
            }

            bestManPanel.Children.Add(toggle);
        }

        private void AddMaidOfHonorToggle(ToggleControl toggle)
        {
            if (!StoredMaidOfHonorClothes.Contains(toggle.Title))
            {
                return;
            }

            var allToggles = maidOfHonorPanel.Children.OfType<ToggleControl>();

            if (allToggles.Any(t => t.Title == toggle.Title))
            {
                maidOfHonorPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                return;
            }

            maidOfHonorPanel.Children.Add(toggle);
        }
    }
}
