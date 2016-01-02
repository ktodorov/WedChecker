using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseBMMOHAccessories : BaseTaskControl
    {
        private List<string> _storedAccessories;
        public List<string> StoredAccessories
        {
            get
            {
                if (_storedAccessories == null)
                {
                    _storedAccessories = AppData.GetGlobalValues(TaskData.Tasks.BestManMaidOfHonorAccessories.ToString());
                }

                return _storedAccessories;
            }
        }

        private List<string> _storedBestManAccessories;
        public List<string> StoredBestManAccessories
        {
            get
            {
                if (_storedBestManAccessories == null)
                {
                    var startIndex = StoredAccessories.IndexOf($"StartBestManAccessories{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredAccessories.IndexOf($"EndBestManAccessories{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedBestManAccessories = new List<string>();
                    _storedBestManAccessories.AddRange(StoredAccessories.Skip(startIndex).Take(endIndex));
                }

                return _storedBestManAccessories;
            }
        }

        private List<string> _storedMaidOfHonorAccessories;
        public List<string> StoredMaidOfHonorAccessories
        {
            get
            {
                if (_storedMaidOfHonorAccessories == null)
                {
                    var startIndex = StoredAccessories.IndexOf($"StartMaidOfHonorAccessories{AppData.GLOBAL_SEPARATOR}") + 1;
                    var endIndex = StoredAccessories.IndexOf($"EndMaidOfHonorAccessories{AppData.GLOBAL_SEPARATOR}") - startIndex;

                    _storedMaidOfHonorAccessories = new List<string>();
                    _storedMaidOfHonorAccessories.AddRange(StoredAccessories.Skip(startIndex).Take(endIndex));
                }

                return _storedMaidOfHonorAccessories;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Best man and maid of honor accessories";
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
                return TaskData.Tasks.PurchaseBMMOHAccessories.ToString();
            }
        }

        public PurchaseBMMOHAccessories()
        {
            this.InitializeComponent();

            foreach (var accessory in StoredBestManAccessories)
            {
                var toggle = new ToggleControl();
                toggle.Title = accessory;
                AddBestManToggle(toggle);
            }

            foreach (var accessory in StoredMaidOfHonorAccessories)
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
            writer.Write(TaskCode);

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
            await AppData.InsertGlobalValue(TaskCode);
        }

        private void AddBestManToggle(ToggleControl toggle)
        {
            if (!StoredBestManAccessories.Contains(toggle.Title))
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
            if (!StoredMaidOfHonorAccessories.Contains(toggle.Title))
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
