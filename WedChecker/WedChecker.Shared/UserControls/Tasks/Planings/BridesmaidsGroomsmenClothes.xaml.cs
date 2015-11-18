using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public partial class BridesmaidsGroomsmenClothes : BaseTaskControl
    {
        private Dictionary<int, string> BridesmaidsClothes { get; set; } = new Dictionary<int, string>();
        private Dictionary<int, string> GroomsmenClothes { get; set; } = new Dictionary<int, string>();

        private bool BridesmaidsClothesChanged = false;
        private bool GroomsmenClothesChanged = false;

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
                return "What are your planned clothes for the bridesmaids and groomsmen? You can add or remove them at any time";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "These are your clothes for the bridesmaids and groomsmen";
            }
        }

        public BridesmaidsGroomsmenClothes()
        {
            this.InitializeComponent();
        }

        public BridesmaidsGroomsmenClothes(Dictionary<string, Dictionary<int, string>> values)
        {
            this.InitializeComponent();
            BridesmaidsClothes = values.ContainsKey("BridesmaidsClothes") ? values["BridesmaidsClothes"] : new Dictionary<int, string>();
            BridesmaidsClothesChanged = false;
            GroomsmenClothes = values.ContainsKey("GroomsmenClothes") ? values["GroomsmenClothes"] : new Dictionary<int, string>();
            GroomsmenClothesChanged = false;
        }

        public override void DisplayValues()
        {
            var bridesmaidsElementChildren = spBridesmaidsClothes.Children.OfType<ElementControl>();
            foreach (var clothing in bridesmaidsElementChildren)
            {
                clothing.DisplayValues();
            }
            addBridesmaidsClothingButton.Visibility = Visibility.Collapsed;


            var groomsmenElementChildren = spGroomsmenClothes.Children.OfType<ElementControl>();
            foreach (var clothing in groomsmenElementChildren)
            {
                clothing.DisplayValues();
            }
            addGroomsmenClothingButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            var bridesmaidsElementChildren = spBridesmaidsClothes.Children.OfType<ElementControl>();
            foreach (var clothing in bridesmaidsElementChildren)
            {
                clothing.EditValues();
            }
            addBridesmaidsClothingButton.Visibility = Visibility.Visible;


            var groomsmenElementChildren = spGroomsmenClothes.Children.OfType<ElementControl>();
            foreach (var clothing in groomsmenElementChildren)
            {
                clothing.EditValues();
            }
            addGroomsmenClothingButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.BridesmaidsGroomsmenClothes.ToString());

            int count = BridesmaidsClothes.Any() ? (GroomsmenClothes.Any() ? 2 : 1) : (GroomsmenClothes.Any() ? 1 : 0);
            writer.Write(count);

            if (BridesmaidsClothes.Any())
            {
                writer.Write("BridesmaidsClothes");
                writer.Write(BridesmaidsClothes.Count);
                foreach (var clothing in BridesmaidsClothes)
                {
                    writer.Write(clothing.Value);
                }
                BridesmaidsClothesChanged = false;
            }

            if (GroomsmenClothes.Any())
            {
                writer.Write("GroomsmenClothes");
                writer.Write(GroomsmenClothes.Count);
                foreach (var clothing in GroomsmenClothes)
                {
                    writer.Write(clothing.Value);
                }
                GroomsmenClothesChanged = false;
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            BridesmaidsClothes = new Dictionary<int, string>();
            GroomsmenClothes = new Dictionary<int, string>();
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
                if (type == "BridesmaidsClothes")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var clothing = reader.ReadString();
                        AddBridesmaidClothing(i, clothing);
                    }
                }
                else if (type == "GroomsmenClothes")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var clothing = reader.ReadString();
                        AddGroomsmanClothing(i, clothing);
                    }
                }
            }

            DisplayValues();
        }

        private void AddGroomsmanClothing(int number, string title)
        {
            if (!GroomsmenClothes.ContainsKey(number) ||
                GroomsmenClothes[number] != title)
            {
                GroomsmenClothes[number] = title;
            }

            var newClothing = new ElementControl(number, title);
            newClothing.removeElementButton.Click += removeGroomsmenClothingButton_Click;
            spGroomsmenClothes.Children.Add(newClothing);
            GroomsmenClothesChanged = true;
        }

        private void AddBridesmaidClothing(int number, string title)
        {
            if (!BridesmaidsClothes.ContainsKey(number) ||
                BridesmaidsClothes[number] != title)
            {
                BridesmaidsClothes[number] = title;
            }

            var newClothing = new ElementControl(number, title);
            newClothing.removeElementButton.Click += removeBridesmaidsClothingButton_Click;
            spBridesmaidsClothes.Children.Add(newClothing);
            BridesmaidsClothesChanged = true;
        }

        public override async Task SubmitValues()
        {
            var bridesmaidsElementChildren = spBridesmaidsClothes.Children.OfType<ElementControl>();
            var groomsmenElementChildren = spGroomsmenClothes.Children.OfType<ElementControl>();

            foreach (var clothing in bridesmaidsElementChildren)
            {
                SaveBridesmaidsClothing(clothing);
            }

            foreach (var clothing in groomsmenElementChildren)
            {
                SaveGroomsmenClothing(clothing);
            }

            if (BridesmaidsClothesChanged || GroomsmenClothesChanged)
            {
                var allClothes = new List<string>();
                allClothes.Add($"StartBridesmaidsClothes{AppData.GLOBAL_SEPARATOR}");
                allClothes.AddRange(bridesmaidsElementChildren.Select(a => a.Title).ToList());
                allClothes.Add($"EndBridesmaidsClothes{AppData.GLOBAL_SEPARATOR}");
                allClothes.Add($"StartGroomsmenClothes{AppData.GLOBAL_SEPARATOR}");
                allClothes.AddRange(groomsmenElementChildren.Select(a => a.Title).ToList());
                allClothes.Add($"EndGroomsmenClothes{AppData.GLOBAL_SEPARATOR}");

                await AppData.InsertGlobalValues(TaskData.Tasks.BridesmaidsGroomsmenClothes.ToString(), allClothes);
            }
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

        private void SaveBridesmaidsClothing(ElementControl clothing)
        {
            if (!BridesmaidsClothes.ContainsKey(clothing.Number) ||
                BridesmaidsClothes[clothing.Number] != clothing.Title)
            {
                BridesmaidsClothes[clothing.Number] = clothing.Title;
                BridesmaidsClothesChanged = true;
            }
        }

        private void SaveGroomsmenClothing(ElementControl clothing)
        {
            if (!GroomsmenClothes.ContainsKey(clothing.Number) ||
                GroomsmenClothes[clothing.Number] != clothing.Title)
            {
                GroomsmenClothes[clothing.Number] = clothing.Title;
                GroomsmenClothesChanged = true;
            }
        }

        private void addBridesmaidsClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(BridesmaidsClothes);

            AddBridesmaidClothing(number, string.Empty);
        }

        private void addGroomsmenClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(GroomsmenClothes);

            AddGroomsmanClothing(number, string.Empty);
        }

        private void removeBridesmaidsClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var clothing = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (BridesmaidsClothes != null)
            {
                BridesmaidsClothes.Remove(clothing.Number);
            }

            spBridesmaidsClothes.Children.Remove(clothing);
        }

        private void removeGroomsmenClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var clothing = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (GroomsmenClothes != null)
            {
                GroomsmenClothes.Remove(clothing.Number);
            }

            spGroomsmenClothes.Children.Remove(clothing);
        }
    }
}
