using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            tbHeader.Text = "These are your planned dishes so far";
        }

        public override void EditValues()
        {
            foreach (var dish in spDishes.Children.OfType<DishControl>())
            {
                dish.EditValues();
            }
            addDishButton.Visibility = Visibility.Visible;
            tbHeader.Text = "You can add your planned foods for the restaurant here";
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.RestaurantFood.ToString());
            writer.Write(Dishes.Count);
            foreach (var dish in Dishes)
            {
                writer.Write(dish.Value);
            }
            DishesChanged = false;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Dishes = new Dictionary<int, string>();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                var dish = reader.ReadString();
                Dishes.Add(i, dish);
            }

            foreach (var dish in Dishes)
            {
                AddDish(new DishControl(dish.Key, dish.Value));
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            foreach (var dish in spDishes.Children.OfType<DishControl>())
            {
                if (dish.tbDishName.Visibility == Visibility.Visible) // Then its in edit mode
                {
                    SaveDish(dish);
                }
            }

            if (DishesChanged)
            {
                await AppData.InsertGlobalValue(TaskData.Tasks.RestaurantFood.ToString());
            }
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

            var newDish = new DishControl(number, string.Empty);

            AddDish(newDish);
            DishesChanged = true;
        }

        private void AddDish(DishControl newDish)
        {
            newDish.saveDishButton.Click += saveDishButton_Click;
            newDish.removeDishButton.Click += removeDishButton_Click;
            newDish.upDishButton.Click += upDishButton_Click;
            newDish.downDishButton.Click += downDishButton_Click;
            spDishes.Children.Insert(newDish.Number, newDish);
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

            Dishes.Add(dishNumber, previousDish.Value);
            Dishes.Add(previousDish.Key, dish.Title);
            AddDish(new DishControl(previousDish.Key, dish.Title));
            AddDish(new DishControl(dishNumber, previousDish.Value));

            DishesChanged = true;
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

            Dishes.Add(dishNumber, nextDish.Value);
            Dishes.Add(nextDish.Key, dish.Title);
            AddDish(new DishControl(dishNumber, nextDish.Value));
            AddDish(new DishControl(nextDish.Key, dish.Title));

            DishesChanged = true;
        }
    }
}
