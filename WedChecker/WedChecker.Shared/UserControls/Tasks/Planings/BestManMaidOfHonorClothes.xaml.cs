﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public partial class BestManMaidOfHonorClothes : BaseTaskControl
    {
        private Dictionary<int, string> BestManClothes { get; set; } = new Dictionary<int, string>();
        private Dictionary<int, string> MaidOfHonorClothes { get; set; } = new Dictionary<int, string>();

        private bool BestManClothesChanged = false;
        private bool MaidOfHonorClothesChanged = false;

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
                return "What are your planned clothes for the best man and maid of honor? You can add or remove them at any time";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "These are your clothes for the best man and maid of honor";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BestManMaidOfHonorClothes.ToString();
            }
        }

        public BestManMaidOfHonorClothes()
        {
            this.InitializeComponent();
        }

        public BestManMaidOfHonorClothes(Dictionary<string, Dictionary<int, string>> values)
        {
            this.InitializeComponent();
            BestManClothes = values.ContainsKey("BestManClothes") ? values["BestManClothes"] : new Dictionary<int, string>();
            BestManClothesChanged = false;
            MaidOfHonorClothes = values.ContainsKey("MaidOfHonorClothes") ? values["MaidOfHonorClothes"] : new Dictionary<int, string>();
            MaidOfHonorClothesChanged = false;
        }

        public override void DisplayValues()
        {
            var bestManElementChildren = spBestManClothes.Children.OfType<ElementControl>();
            foreach (var clothing in bestManElementChildren)
            {
                clothing.DisplayValues();
            }
            addBestManClothingButton.Visibility = Visibility.Collapsed;


            var maidOfHonorElementChildren = spMaidOfHonorClothes.Children.OfType<ElementControl>();
            foreach (var clothing in maidOfHonorElementChildren)
            {
                clothing.DisplayValues();
            }
            addMaidOfHonorClothingButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            var bestManElementChildren = spBestManClothes.Children.OfType<ElementControl>();
            foreach (var clothing in bestManElementChildren)
            {
                clothing.EditValues();
            }
            addBestManClothingButton.Visibility = Visibility.Visible;


            var maidOfHonorElementChildren = spMaidOfHonorClothes.Children.OfType<ElementControl>();
            foreach (var clothing in maidOfHonorElementChildren)
            {
                clothing.EditValues();
            }
            addMaidOfHonorClothingButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskCode);

            int count = BestManClothes.Any() ? (MaidOfHonorClothes.Any() ? 2 : 1) : (MaidOfHonorClothes.Any() ? 1 : 0);
            writer.Write(count);

            if (BestManClothes.Any())
            {
                writer.Write("BestManClothes");
                writer.Write(BestManClothes.Count);
                foreach (var clothing in BestManClothes)
                {
                    writer.Write(clothing.Value);
                }
                BestManClothesChanged = false;
            }

            if (MaidOfHonorClothes.Any())
            {
                writer.Write("MaidOfHonorClothes");
                writer.Write(MaidOfHonorClothes.Count);
                foreach (var clothing in MaidOfHonorClothes)
                {
                    writer.Write(clothing.Value);
                }
                MaidOfHonorClothesChanged = false;
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            BestManClothes = new Dictionary<int, string>();
            MaidOfHonorClothes = new Dictionary<int, string>();
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
                if (type == "BestManClothes")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var clothing = reader.ReadString();
                        AddBestManClothing(i, clothing);
                    }
                }
                else if (type == "MaidOfHonorClothes")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var clothing = reader.ReadString();
                        AddMaidOfHonorClothing(i, clothing);
                    }
                }
            }

            DisplayValues();
        }

        private void AddMaidOfHonorClothing(int number, string title)
        {
            if (!MaidOfHonorClothes.ContainsKey(number) ||
                MaidOfHonorClothes[number] != title)
            {
                MaidOfHonorClothes[number] = title;
            }

            var newClothing = new ElementControl(number, title);
            newClothing.removeElementButton.Click += removeMaidOfHonorClothingButton_Click;
            spMaidOfHonorClothes.Children.Add(newClothing);
            MaidOfHonorClothesChanged = true;
        }

        private void AddBestManClothing(int number, string title)
        {
            if (!BestManClothes.ContainsKey(number) ||
                BestManClothes[number] != title)
            {
                BestManClothes[number] = title;
            }

            var newClothing = new ElementControl(number, title);
            newClothing.removeElementButton.Click += removeBestManClothingButton_Click;
            spBestManClothes.Children.Add(newClothing);
            BestManClothesChanged = true;
        }

        public override async Task SubmitValues()
        {
            var bestManElementChildren = spBestManClothes.Children.OfType<ElementControl>();
            var maidOfHonorElementChildren = spMaidOfHonorClothes.Children.OfType<ElementControl>();

            foreach (var clothing in bestManElementChildren)
            {
                SaveBestManClothing(clothing);
            }

            foreach (var clothing in maidOfHonorElementChildren)
            {
                SaveMaidOfHonorClothing(clothing);
            }

            if (BestManClothesChanged || MaidOfHonorClothesChanged)
            {
                var allClothes = new List<string>();
                allClothes.Add($"StartBestManClothes{AppData.GLOBAL_SEPARATOR}");
                allClothes.AddRange(bestManElementChildren.Select(a => a.Title).ToList());
                allClothes.Add($"EndBestManClothes{AppData.GLOBAL_SEPARATOR}");
                allClothes.Add($"StartMaidOfHonorClothes{AppData.GLOBAL_SEPARATOR}");
                allClothes.AddRange(maidOfHonorElementChildren.Select(a => a.Title).ToList());
                allClothes.Add($"EndMaidOfHonorClothes{AppData.GLOBAL_SEPARATOR}");

                await AppData.InsertGlobalValues(TaskCode, allClothes);
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

        private void SaveBestManClothing(ElementControl clothing)
        {
            if (!BestManClothes.ContainsKey(clothing.Number) ||
                BestManClothes[clothing.Number] != clothing.Title)
            {
                BestManClothes[clothing.Number] = clothing.Title;
                BestManClothesChanged = true;
            }
        }

        private void SaveMaidOfHonorClothing(ElementControl clothing)
        {
            if (!MaidOfHonorClothes.ContainsKey(clothing.Number) ||
                MaidOfHonorClothes[clothing.Number] != clothing.Title)
            {
                MaidOfHonorClothes[clothing.Number] = clothing.Title;
                MaidOfHonorClothesChanged = true;
            }
        }

        private void addBestManClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(BestManClothes);

            AddBestManClothing(number, string.Empty);
        }

        private void addMaidOfHonorClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(MaidOfHonorClothes);

            AddMaidOfHonorClothing(number, string.Empty);
        }

        private void removeBestManClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var clothing = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (BestManClothes != null)
            {
                BestManClothes.Remove(clothing.Number);
            }

            spBestManClothes.Children.Remove(clothing);
        }

        private void removeMaidOfHonorClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var clothing = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (MaidOfHonorClothes != null)
            {
                MaidOfHonorClothes.Remove(clothing.Number);
            }

            spMaidOfHonorClothes.Children.Remove(clothing);
        }
    }
}
