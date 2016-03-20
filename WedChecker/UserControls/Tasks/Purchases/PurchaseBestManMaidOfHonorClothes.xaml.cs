using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseBMMOHClothes : BaseTaskControl
    {
        private List<string> _storedBestManClothes;
        public List<string> StoredBestManClothes
        {
            get
            {
                if (_storedBestManClothes == null)
                {
                    _storedBestManClothes = AppData.GetStorage("BestMenClothes") as List<string>;
                }

                if ((_storedBestManClothes == null || !_storedBestManClothes.Any()) && (_storedMaidOfHonorClothes == null || !_storedMaidOfHonorClothes.Any()))
                {
                    throw new WedCheckerInvalidDataException("You must first add clothes for the best men or maids of honor in order to mark them purchased after that!");
                }

                if (_storedBestManClothes == null)
                {
                    _storedMaidOfHonorClothes = new List<string>();
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
                    _storedMaidOfHonorClothes = AppData.GetStorage("MaidsOfHonorClothes") as List<string>;
                }

                if ((_storedBestManClothes == null || !_storedBestManClothes.Any()) && (_storedMaidOfHonorClothes == null || !_storedMaidOfHonorClothes.Any()))
                {
                    throw new WedCheckerInvalidDataException("You must first add clothes for the best men or maids of honor in order to mark them purchased after that!");
                }

                if(_storedMaidOfHonorClothes == null)
                {
                    _storedMaidOfHonorClothes = new List<string>();
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

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.PurchaseBMMOHClothes.ToString();
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

        protected override void Serialize(BinaryWriter writer)
        {
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
