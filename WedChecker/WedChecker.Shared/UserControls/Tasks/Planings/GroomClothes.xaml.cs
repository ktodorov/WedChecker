using System.Collections;
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
    public partial class GroomClothes : BaseTaskControl
    {
        private Dictionary<int, string> Clothes { get; set; } = new Dictionary<int, string>();

        private bool ClothesChanged = false;

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
                return "What are your planned clothes for the groom? You can add or remove them at any time";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "These are your groom clothes";
            }
        }

        public GroomClothes()
        {
            this.InitializeComponent();
        }

        public GroomClothes(Dictionary<int, string> values)
        {
            this.InitializeComponent();
            Clothes = values;
            ClothesChanged = false;
        }

        public override void DisplayValues()
        {
            foreach (var clothing in spGroomClothes.Children.OfType<ElementControl>())
            {
                clothing.DisplayValues();
            }
            addClothingButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            foreach (var clothing in spGroomClothes.Children.OfType<ElementControl>())
            {
                clothing.EditValues();
            }
            addClothingButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.GroomClothes.ToString());
            writer.Write(Clothes.Count);
            foreach (var clothing in Clothes)
            {
                writer.Write(clothing.Value);
            }
            ClothesChanged = false;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Clothes = new Dictionary<int, string>();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                var clothing = reader.ReadString();
                Clothes.Add(i, clothing);
            }

            foreach (var clothing in Clothes)
            {
                spGroomClothes.Children.Add(new ElementControl(clothing.Key, clothing.Value));
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            var childrenClothes = spGroomClothes.Children.OfType<ElementControl>();
            foreach (var clothing in childrenClothes)
            {
                SaveClothing(clothing);
            }

            if (ClothesChanged)
            {
                await AppData.InsertGlobalValue(TaskData.Tasks.GroomClothes.ToString());
            }
        }

        private int FindFirstFreeNumber()
        {
            var result = 0;

            if (Clothes != null)
            {
                while (Clothes.Keys.Any(k => k == result))
                {
                    result++;
                }
            }

            return result;
        }

        private void SaveClothing(ElementControl clothing)
        {
            if (!Clothes.ContainsKey(clothing.Number) ||
                Clothes[clothing.Number] != clothing.tbElementName.Text)
            {
                Clothes[clothing.Number] = clothing.tbElementName.Text;
                ClothesChanged = true;
            }
        }

        private void addClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber();

            var newClothing = new ElementControl(number, string.Empty);
            Clothes.Add(number, string.Empty);
            newClothing.removeElementButton.Click += removeClothingButton_Click;
            spGroomClothes.Children.Add(newClothing);
            ClothesChanged = true;
        }

        private void removeClothingButton_Click(object sender, RoutedEventArgs e)
        {
            var clothing = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (Clothes != null)
            {
                Clothes.Remove(clothing.Number);
            }

            spGroomClothes.Children.Remove(clothing);
        }
    }
}
