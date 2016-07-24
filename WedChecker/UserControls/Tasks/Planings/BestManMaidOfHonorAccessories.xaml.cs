using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public partial class BestManMaidOfHonorAccessories : BaseTaskControl
    {
        private Dictionary<int, string> BestManAccessories { get; set; } = new Dictionary<int, string>();
        private Dictionary<int, string> MaidOfHonorAccessories { get; set; } = new Dictionary<int, string>();

        private bool BestManAccessoriesChanged = false;
        private bool MaidOfHonorAccessoriesChanged = false;

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
                return "What are your planned accessories for the best man and maid of honor? You can add or remove them at any time";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "These are your accessories for the best man and maid of honor";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BestManMaidOfHonorAccessories.ToString();
            }
        }

        public BestManMaidOfHonorAccessories()
        {
            this.InitializeComponent();
        }

        public BestManMaidOfHonorAccessories(Dictionary<string, Dictionary<int, string>> values)
        {
            this.InitializeComponent();
            BestManAccessories = values.ContainsKey("BestManAcc") ? values["BestManAcc"] : new Dictionary<int, string>();
            BestManAccessoriesChanged = false;
            MaidOfHonorAccessories = values.ContainsKey("MaidOfHonorAcc") ? values["MaidOfHonorAcc"] : new Dictionary<int, string>();
            MaidOfHonorAccessoriesChanged = false;
        }

        public override void DisplayValues()
        {
            var bestManElementChildren = spBestManAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in bestManElementChildren)
            {
                accessory.DisplayValues();
            }
            addBestManAccessoryButton.Visibility = Visibility.Collapsed;


            var maidOfHonorElementChildren = spMaidOfHonorAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in maidOfHonorElementChildren)
            {
                accessory.DisplayValues();
            }
            addMaidOfHonorAccessoryButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            var bestManElementChildren = spBestManAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in bestManElementChildren)
            {
                accessory.EditValues();
            }
            addBestManAccessoryButton.Visibility = Visibility.Visible;


            var maidOfHonorElementChildren = spMaidOfHonorAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in maidOfHonorElementChildren)
            {
                accessory.EditValues();
            }
            addMaidOfHonorAccessoryButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            var bestManElementChildren = spBestManAccessories.Children.OfType<ElementControl>();
            var maidOfHonorElementChildren = spMaidOfHonorAccessories.Children.OfType<ElementControl>();

            foreach (var accessory in bestManElementChildren)
            {
                SaveBestManAccessory(accessory);
            }

            foreach (var accessory in maidOfHonorElementChildren)
            {
                SaveMaidOfHonorAccessory(accessory);
            }

            int count = BestManAccessories.Any() ? (MaidOfHonorAccessories.Any() ? 2 : 1) : (MaidOfHonorAccessories.Any() ? 1 : 0);
            writer.Write(count);

            if (BestManAccessories.Any())
            {
                writer.Write("BestManAcc");
                writer.Write(BestManAccessories.Count);
                foreach (var accessory in BestManAccessories)
                {
                    writer.Write(accessory.Value);
                }
                BestManAccessoriesChanged = false;
            }

            if (MaidOfHonorAccessories.Any())
            {
                writer.Write("MaidOfHonorAcc");
                writer.Write(MaidOfHonorAccessories.Count);
                foreach (var accessory in MaidOfHonorAccessories)
                {
                    writer.Write(accessory.Value);
                }
                MaidOfHonorAccessoriesChanged = false;
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            BestManAccessories = new Dictionary<int, string>();
            MaidOfHonorAccessories = new Dictionary<int, string>();
            var count = reader.ReadInt32();

            if (count == 0)
            {
                return;
            }

            while (count > 0)
            {
                count--;

                var type = reader.ReadString();
                var size = reader.ReadInt32();
                if (type == "BestManAcc")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var accessory = reader.ReadString();
                        AddBestManAccessory(i, accessory);
                    }
                }
                else if (type == "MaidOfHonorAcc")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var accessory = reader.ReadString();
                        AddMaidOfHonorAccessory(i, accessory);
                    }
                }
            }
        }

        private void AddMaidOfHonorAccessory(int number, string title)
        {
            if (!MaidOfHonorAccessories.ContainsKey(number) ||
                MaidOfHonorAccessories[number] != title)
            {
                MaidOfHonorAccessories[number] = title;
            }

            var newAccessory = new ElementControl(number, title);
            newAccessory.removeElementButton.Click += removeMaidOfHonorAccessoryButton_Click;
            spMaidOfHonorAccessories.Children.Add(newAccessory);
            MaidOfHonorAccessoriesChanged = true;
        }

        private void AddBestManAccessory(int number, string title)
        {
            if (!BestManAccessories.ContainsKey(number) ||
                BestManAccessories[number] != title)
            {
                BestManAccessories[number] = title;
            }

            var newAccessory = new ElementControl(number, title);
            newAccessory.removeElementButton.Click += removeBestManAccessoryButton_Click;
            spBestManAccessories.Children.Add(newAccessory);
            BestManAccessoriesChanged = true;
        }

        protected override void SetLocalStorage()
        {
            var bestManElementChildren = spBestManAccessories.Children.OfType<ElementControl>();
            var maidOfHonorElementChildren = spMaidOfHonorAccessories.Children.OfType<ElementControl>();

            var bestMenAccessories = bestManElementChildren?.Select(a => a.Title).ToList();
            AppData.SetStorage("BestMenAccessories", bestMenAccessories);

            var maidsOfHonorAccessories = maidOfHonorElementChildren?.Select(a => a.Title).ToList();
            AppData.SetStorage("MaidsOfHonorAccessories", maidsOfHonorAccessories);
        }

        private int FindFirstFreeNumber(Dictionary<int, string> dict)
        {
            var result = 0;

            if (dict != null)
            {
                while (dict.Keys.Any(k => k == result))
                {
                    result++;
                }
            }

            return result;
        }

        private void SaveBestManAccessory(ElementControl accessory)
        {
            if (!BestManAccessories.ContainsKey(accessory.Number) ||
                BestManAccessories[accessory.Number] != accessory.Title)
            {
                BestManAccessories[accessory.Number] = accessory.Title;
                BestManAccessoriesChanged = true;
            }
        }

        private void SaveMaidOfHonorAccessory(ElementControl accessory)
        {
            if (!MaidOfHonorAccessories.ContainsKey(accessory.Number) ||
                MaidOfHonorAccessories[accessory.Number] != accessory.Title)
            {
                MaidOfHonorAccessories[accessory.Number] = accessory.Title;
                MaidOfHonorAccessoriesChanged = true;
            }
        }

        private void addBestManAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(BestManAccessories);

            AddBestManAccessory(number, string.Empty);
        }

        private void addMaidOfHonorAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(MaidOfHonorAccessories);

            AddMaidOfHonorAccessory(number, string.Empty);
        }

        private void removeBestManAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var accessory = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (BestManAccessories != null)
            {
                BestManAccessories.Remove(accessory.Number);
            }

            spBestManAccessories.Children.Remove(accessory);
        }

        private void removeMaidOfHonorAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var accessory = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (MaidOfHonorAccessories != null)
            {
                MaidOfHonorAccessories.Remove(accessory.Number);
            }

            spMaidOfHonorAccessories.Children.Remove(accessory);
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            if (BestManAccessories != null && BestManAccessories.Any())
            {
                sb.AppendLine("Best man:");
                foreach (var accessory in BestManAccessories)
                {
                    sb.Append(" - ");
                    sb.AppendLine(accessory.Value);
                }
            }
            if (MaidOfHonorAccessories != null && MaidOfHonorAccessories.Any())
            {
                sb.AppendLine("Maid of honor:");
                foreach (var accessory in MaidOfHonorAccessories)
                {
                    sb.Append(" - ");
                    sb.AppendLine(accessory.Value);
                }
            }
        }
    }
}
