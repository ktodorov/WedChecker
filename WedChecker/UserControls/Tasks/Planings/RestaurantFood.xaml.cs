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
    public sealed partial class RestaurantFood : BaseTaskControl
    {
        private Dictionary<int, string> Dishes { get; set; } = new Dictionary<int, string>();

        private bool DishesChanged = false;

        public override string TaskName
        {
            get
            {
                return "Restaurant food";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "You can add your planned foods for the restaurant here";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "These are your planned dishes so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.RestaurantFood.ToString();
            }
        }

        public RestaurantFood()
        {
            this.InitializeComponent();
        }

        public RestaurantFood(Dictionary<int, string> values)
        {
            this.InitializeComponent();
            Dishes = values;
            DishesChanged = false;
        }
        public override void DisplayValues()
        {
            foreach (var dish in spDishes.Children.OfType<DishControl>())
            {
                dish.DisplayValues();
            }
            addDishButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            foreach (var dish in spDishes.Children.OfType<DishControl>())
            {
                dish.EditValues();
            }
            addDishButton.Visibility = Visibility.Visible;
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var dishes = spDishes.Children.OfType<DishControl>();
            foreach (var dish in dishes)
            {
                SaveDish(dish);
            }

            writer.Write(Dishes.Count);
            foreach (var dish in Dishes)
            {
                writer.Write(dish.Value);
            }
            DishesChanged = false;
        }

        protected override void Deserialize(BinaryReader reader)
        {
            Dishes = new Dictionary<int, string>();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                var dish = reader.ReadString();
                AddDish(i, dish);
            }
        }

        protected override void SetLocalStorage()
        {
            var dishes = spDishes.Children.OfType<DishControl>();

            var foodTitles = dishes.Select(a => a.Title).ToList();
            AppData.SetStorage("RestaurantFood", foodTitles);
        }

        private int FindFirstFreeNumber()
        {
            var result = 0;

            if (Dishes != null)
            {
                while (Dishes.Keys.Any(k => k == result))
                {
                    result++;
                }
            }

            return result;
        }

        private void SaveDish(DishControl dish)
        {
            if (!Dishes.ContainsKey(dish.Number) ||
                Dishes[dish.Number] != dish.tbDishName.Text)
            {
                Dishes[dish.Number] = dish.tbDishName.Text;
                DishesChanged = true;
            }
        }

        private void addDishButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber();

            AddDish(number, string.Empty);
        }

        private void AddDish(int number, string title)
        {
            if (!Dishes.ContainsKey(number) || Dishes[number] != title)
            {
                Dishes[number] = title;
            }

            var newDish = new DishControl(number, title);
            newDish.saveDishButton.Click += saveDishButton_Click;
            newDish.removeDishButton.Click += removeDishButton_Click;
            newDish.upDishButton.Click += upDishButton_Click;
            newDish.downDishButton.Click += downDishButton_Click;
            spDishes.Children.Insert(newDish.Number, newDish);

            DishesChanged = true;
        }

        private void saveDishButton_Click(object sender, RoutedEventArgs e)
        {
            var dish = (((sender as Button).Parent as Grid).Parent as Grid).Parent as DishControl;
            SaveDish(dish);

            dish.DisplayValues();
        }

        private void removeDishButton_Click(object sender, RoutedEventArgs e)
        {
            var dish = (((sender as Button).Parent as Grid).Parent as Grid).Parent as DishControl;
            if (Dishes != null)
            {
                Dishes.Remove(dish.Number);
            }

            spDishes.Children.Remove(dish);

            UpdateDishesNumbers();
        }

        private void UpdateDishesNumbers()
        {
            var dishControls = spDishes.Children.OfType<DishControl>();
            var i = -1;
            foreach (var dishControl in dishControls)
            {
                i++;
                if (dishControl.Number != i)
                {
                    spDishes.Children.RemoveAt(i);
                    dishControl.Number = i;
                    spDishes.Children.Insert(i, dishControl);
                }
            }
        }

        private void upDishButton_Click(object sender, RoutedEventArgs e)
        {
            var dish = (((sender as Button).Parent as Grid).Parent as Grid).Parent as DishControl;
            var dishNumber = dish.Number;

            if (Dishes.Count < 2 || !Dishes.Any(d => d.Key < dishNumber))
            {
                return;
            }

            Dishes.OrderBy(d => d.Key);
            var previousDish = Dishes.Last(d => d.Key < dish.Number);

            Dishes.Remove(dishNumber);
            Dishes.Remove(previousDish.Key);
            spDishes.Children.Remove(spDishes.Children.OfType<DishControl>().FirstOrDefault(d => d.Number == dish.Number));
            spDishes.Children.Remove(spDishes.Children.OfType<DishControl>().FirstOrDefault(d => d.Number == previousDish.Key));

            AddDish(previousDish.Key, dish.Title);
            AddDish(dishNumber, previousDish.Value);
        }

        private void downDishButton_Click(object sender, RoutedEventArgs e)
        {
            var dish = (((sender as Button).Parent as Grid).Parent as Grid).Parent as DishControl;
            var dishNumber = dish.Number;

            if (Dishes.Count < 2 || !Dishes.Any(d => d.Key > dishNumber))
            {
                return;
            }

            Dishes.OrderBy(d => d.Key);
            var nextDish = Dishes.FirstOrDefault(d => d.Key > dish.Number);

            Dishes.Remove(dishNumber);
            Dishes.Remove(nextDish.Key);
            spDishes.Children.Remove(spDishes.Children.OfType<DishControl>().FirstOrDefault(d => d.Number == dish.Number));
            spDishes.Children.Remove(spDishes.Children.OfType<DishControl>().FirstOrDefault(d => d.Number == nextDish.Key));

            AddDish(dishNumber, nextDish.Value);
            AddDish(nextDish.Key, dish.Title);
        }
    }
}
