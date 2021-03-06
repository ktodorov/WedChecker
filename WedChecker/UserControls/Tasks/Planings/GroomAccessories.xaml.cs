﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
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

        public static new string DisplayHeader
        {
            get
            {
                return "These are your groom accessories";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.GroomAccessories.ToString();
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
            foreach (var accessory in spGroomAccessories.Children.OfType<ElementControl>())
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
            var accessories = spGroomAccessories.Children.OfType<ElementControl>();
            foreach (var accessory in accessories)
            {
                SaveAccessory(accessory);
            }

            writer.Write(Accessories.Count);
            foreach (var accessory in Accessories)
            {
                writer.Write(accessory.Value);
            }
            AccessoriesChanged = false;
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            Accessories = new Dictionary<int, string>();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                var accessory = reader.ReadString();
                AddAccessory(i, accessory);
            }
        }

        private void AddAccessory(int number, string title)
        {
            if (!Accessories.ContainsKey(number) ||
                Accessories[number] != title)
            {
                Accessories[number] = title;
            }

            var accessoryControl = new ElementControl(number, title);
            accessoryControl.removeElementButton.Click += removeAccessoryButton_Click;
            spGroomAccessories.Children.Add(accessoryControl);

            AccessoriesChanged = true;
        }

        protected override void SetLocalStorage()
        {
            var accessories = spGroomAccessories.Children.OfType<ElementControl>();

            var accessoriesTitles = accessories.Select(a => a.Title).ToList();
            AppData.SetStorage("GroomAccessories", accessoriesTitles);
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
                   Accessories[accessory.Number] != accessory.Title)
            {
                Accessories[accessory.Number] = accessory.Title;
                AccessoriesChanged = true;
            }
        }

        private void addAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber();

            AddAccessory(number, string.Empty);
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

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            if (Accessories != null && Accessories.Any())
            {
                foreach (var accessory in Accessories)
                {
                    sb.Append(" - ");
                    sb.AppendLine(accessory.Value);
                }
            }
        }
    }
}
