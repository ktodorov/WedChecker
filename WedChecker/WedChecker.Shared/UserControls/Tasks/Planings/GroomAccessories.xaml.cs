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
    public partial class GroomAccessories : BaseTaskControl
    {
        private Dictionary<int, string> Accessories { get; set; } = new Dictionary<int, string>();

        private bool AccessoriesChanged = false;

        public override string TaskName
        {
            get
            {
                return "Groom accessories";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "What are your planned accessories for the groom? You can add or remove them at any time";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "These are your groom accessories";
            }
        }

        public GroomAccessories()
        {
            this.InitializeComponent();
        }

        public GroomAccessories(Dictionary<int, string> values)
        {
            this.InitializeComponent();
            Accessories = values;
            AccessoriesChanged = false;
        }

        public override void DisplayValues()
        {
            foreach(var accessory in spGroomAccessories.Children.OfType<ElementControl>())
            {
                accessory.DisplayValues();
            }
            addAccessoryButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            foreach (var accessory in spGroomAccessories.Children.OfType<ElementControl>())
            {
                accessory.EditValues();
            }
            addAccessoryButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.GroomAccessories.ToString());
            writer.Write(Accessories.Count);
            foreach (var accessory in Accessories)
            {
                writer.Write(accessory.Value);
            }
            AccessoriesChanged = false;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Accessories = new Dictionary<int, string>();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                var accessory = reader.ReadString();
                Accessories.Add(i, accessory);
            }

            foreach (var accessory in Accessories)
            {
                spGroomAccessories.Children.Add(new ElementControl(accessory.Key, accessory.Value));
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            foreach (var accessory in spGroomAccessories.Children.OfType<ElementControl>())
            {
                if (accessory.tbElementName.Visibility == Visibility.Visible) // Then its in edit mode
                {
                    SaveAccessory(accessory);
                }
            }

            if (AccessoriesChanged)
            {
                await AppData.InsertGlobalValue(TaskData.Tasks.GroomAccessories.ToString());
            }
        }

        private int FindFirstFreeNumber()
        {
            var result = 0;

            if (Accessories != null)
            {
                while (Accessories.Keys.Any(k => k == result))
                {
                    result++;
                }
            }

            return result;
        }

        private void SaveAccessory(ElementControl accessory)
        {
            if (!Accessories.ContainsKey(accessory.Number) ||
                Accessories[accessory.Number] != accessory.tbElementName.Text)
            {
                Accessories[accessory.Number] = accessory.tbElementName.Text;
                AccessoriesChanged = true;
            }
        }

        private void addAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber();

            var newAccessory = new ElementControl(number, string.Empty);
            Accessories.Add(number, string.Empty);
            newAccessory.removeElementButton.Click += removeAccessoryButton_Click;
            spGroomAccessories.Children.Add(newAccessory);
            AccessoriesChanged = true;
        }

        private void removeAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var accessory = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (Accessories != null)
            {
                Accessories.Remove(accessory.Number);
            }

            spGroomAccessories.Children.Remove(accessory);
        }
    }
}