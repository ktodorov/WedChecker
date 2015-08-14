using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class DishControl : UserControl
    {
        public int Number { get; set; }
        public string Title { get; set; }

        public DishControl()
        {
            this.InitializeComponent();
        }

        public DishControl(int number, string name)
        {
            this.InitializeComponent();
            Number = number;
            Title = name;
            tbDishName.Text = name;
            tbDisplayDishName.Text = name;
        }

        public void DisplayValues()
        {
            Title = tbDishName.Text;
            tbDisplayDishName.Text = tbDishName.Text;
            tbDishName.Visibility = Visibility.Collapsed;
            tbDisplayDishName.Visibility = Visibility.Visible;
            removeDishButton.Visibility = Visibility.Collapsed;
            saveDishButton.Visibility = Visibility.Collapsed;
            upDishButton.Visibility = Visibility.Collapsed;
            downDishButton.Visibility = Visibility.Collapsed;
        }

        public void EditValues()
        {
            Title = tbDisplayDishName.Text;
            tbDishName.Text = tbDisplayDishName.Text;
            tbDishName.Visibility = Visibility.Visible;
            tbDisplayDishName.Visibility = Visibility.Collapsed;
            removeDishButton.Visibility = Visibility.Visible;
            saveDishButton.Visibility = Visibility.Visible;
            upDishButton.Visibility = Visibility.Visible;
            downDishButton.Visibility = Visibility.Visible;
        }
    }
}
