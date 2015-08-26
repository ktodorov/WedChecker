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
    public partial class BridesmaidsGroomsmenAccessories : BaseTaskControl
    {
        private Dictionary<int, string> BridesmaidsAccessories { get; set; } = new Dictionary<int, string>();
        private Dictionary<int, string> GroomsmenAccessories { get; set; } = new Dictionary<int, string>();

        private bool BridesmaidsAccessoriesChanged = false;
        private bool GroomsmenAccessoriesChanged = false;

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
                return "What are your planned accessories for the bridesmaids and the groomsmen? You can add or remove them at any time";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "These are your accessories for the bridesmaids and the groomsmen";
            }
        }

        public BridesmaidsGroomsmenAccessories()
        {
            this.InitializeComponent();
        }

        public BridesmaidsGroomsmenAccessories(Dictionary<string, Dictionary<int, string>> values)
        {
            this.InitializeComponent();
            BridesmaidsAccessories = values.ContainsKey("BridesmaidsAcc") ? values["BridesmaidsAcc"] : new Dictionary<int, string>();
            BridesmaidsAccessoriesChanged = false;
            GroomsmenAccessories = values.ContainsKey("GroomsmenAcc") ? values["GroomsmenAcc"] : new Dictionary<int, string>();
            GroomsmenAccessoriesChanged = false;
        }

        public override void DisplayValues()
        {
            var bridesmaidsElementChildren = spBridesmaidsAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in bridesmaidsElementChildren)
            {
                accessory.DisplayValues();
            }
            addBridesmaidsAccessoryButton.Visibility = Visibility.Collapsed;


            var groomsmenElementChildren = spGroomsmenAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in groomsmenElementChildren)
            {
                accessory.DisplayValues();
            }
            addGroomsmenAccessoryButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            var bridesmaidsElementChildren = spBridesmaidsAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in bridesmaidsElementChildren)
            {
                accessory.EditValues();
            }
            addBridesmaidsAccessoryButton.Visibility = Visibility.Visible;


            var groomsmenElementChildren = spGroomsmenAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in groomsmenElementChildren)
            {
                accessory.EditValues();
            }
            addGroomsmenAccessoryButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.BridesmaidsGroomsmenAccessories.ToString());

            int count = BridesmaidsAccessories.Any() ? (GroomsmenAccessories.Any() ? 2 : 1) : (GroomsmenAccessories.Any() ? 1 : 0);
            writer.Write(count);

            if (BridesmaidsAccessories.Any())
            {
                writer.Write("BridesmaidsAcc");
                writer.Write(BridesmaidsAccessories.Count);
                foreach (var accessory in BridesmaidsAccessories)
                {
                    writer.Write(accessory.Value);
                }
                BridesmaidsAccessoriesChanged = false;
            }

            if (GroomsmenAccessories.Any())
            {
                writer.Write("GroomsmenAcc");
                writer.Write(GroomsmenAccessories.Count);
                foreach (var accessory in GroomsmenAccessories)
                {
                    writer.Write(accessory.Value);
                }
                GroomsmenAccessoriesChanged = false;
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            BridesmaidsAccessories = new Dictionary<int, string>();
            GroomsmenAccessories = new Dictionary<int, string>();
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
                if (type == "BridesmaidsAcc")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var accessory = reader.ReadString();
                        BridesmaidsAccessories.Add(i, accessory);
                    }

                    foreach (var accessory in BridesmaidsAccessories)
                    {
                        spBridesmaidsAccessories.Children.Add(new ElementControl(accessory.Key, accessory.Value));
                    }
                }
                else if (type == "GroomsmenAcc")
                {
                    for (int i = 0; i < size; i++)
                    {
                        var accessory = reader.ReadString();
                        GroomsmenAccessories.Add(i, accessory);
                    }

                    foreach (var accessory in GroomsmenAccessories)
                    {
                        spGroomsmenAccessories.Children.Add(new ElementControl(accessory.Key, accessory.Value));
                    }
                }
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            var bridesmaidsElementChildren = spBridesmaidsAccessories.Children.OfType<ElementControl>();
            var groomsmenElementChildren = spGroomsmenAccessories.Children.OfType<ElementControl>();

            foreach (var accessory in bridesmaidsElementChildren)
            {
                SaveBridesmaidsAccessory(accessory);
            }

            foreach (var accessory in groomsmenElementChildren)
            {
                SaveGroomsmenAccessory(accessory);
            }

            if (BridesmaidsAccessoriesChanged || GroomsmenAccessoriesChanged)
            {
                await AppData.InsertGlobalValue(TaskData.Tasks.BridesmaidsGroomsmenAccessories.ToString());
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

        private void SaveBridesmaidsAccessory(ElementControl accessory)
        {
            if (!BridesmaidsAccessories.ContainsKey(accessory.Number) ||
                BridesmaidsAccessories[accessory.Number] != accessory.tbElementName.Text)
            {
                BridesmaidsAccessories[accessory.Number] = accessory.tbElementName.Text;
                BridesmaidsAccessoriesChanged = true;
            }
        }

        private void SaveGroomsmenAccessory(ElementControl accessory)
        {
            if (!GroomsmenAccessories.ContainsKey(accessory.Number) ||
                GroomsmenAccessories[accessory.Number] != accessory.tbElementName.Text)
            {
                GroomsmenAccessories[accessory.Number] = accessory.tbElementName.Text;
                GroomsmenAccessoriesChanged = true;
            }
        }

        private void addBridesmaidsAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(BridesmaidsAccessories);

            var newAccessory = new ElementControl(number, string.Empty);
            BridesmaidsAccessories.Add(number, string.Empty);
            newAccessory.removeElementButton.Click += removeBridesmaidsAccessoryButton_Click;
            spBridesmaidsAccessories.Children.Add(newAccessory);
            BridesmaidsAccessoriesChanged = true;
        }

        private void addGroomsmenAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber(GroomsmenAccessories);

            var newAccessory = new ElementControl(number, string.Empty);
            GroomsmenAccessories.Add(number, string.Empty);
            newAccessory.removeElementButton.Click += removeGroomsmenAccessoryButton_Click;
            spGroomsmenAccessories.Children.Add(newAccessory);
            GroomsmenAccessoriesChanged = true;
        }

        private void removeBridesmaidsAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var accessory = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (BridesmaidsAccessories != null)
            {
                BridesmaidsAccessories.Remove(accessory.Number);
            }

            spBridesmaidsAccessories.Children.Remove(accessory);
        }

        private void removeGroomsmenAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var accessory = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (GroomsmenAccessories != null)
            {
                GroomsmenAccessories.Remove(accessory.Number);
            }

            spGroomsmenAccessories.Children.Remove(accessory);
        }
    }
}