using WedChecker.Common;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
	public sealed partial class TaskTileControl : UserControl
    {
		public event TappedEventHandler TileTapped;

		public TaskCategories TaskCategory
		{
			get
			{
				return (TaskCategories)GetValue(TaskCategoryProperty);
			}
			set
			{
				SetValue(TaskCategoryProperty, value);
			}
		}
		public static DependencyProperty TaskCategoryProperty = DependencyProperty.Register("TaskCategory", typeof(TaskCategories), typeof(TaskTileControl), new PropertyMetadata(TaskCategories.Home));

		public string TaskTitle
		{
			get
			{
				return (string)GetValue(TaskTitleProperty);
			}
			set
			{
				SetValue(TaskTitleProperty, value.ToUpper());
				tbTaskTitle.Text = value.ToUpper(); //set the textbox content
			}
		}
		public static DependencyProperty TaskTitleProperty = DependencyProperty.Register("TaskTitle", typeof(string), typeof(TaskTileControl), new PropertyMetadata(""));

		public string TaskName
		{
			get
			{
				return (string)GetValue(TaskNameProperty);
			}
			set
			{
				SetValue(TaskNameProperty, value);
			}
		}
		public static DependencyProperty TaskNameProperty = DependencyProperty.Register("TaskName", typeof(string), typeof(TaskTileControl), new PropertyMetadata(""));


		public string TaskImageUrl { get; set; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    EnabledChanged();
                }
            }
        }

        public TaskTileControl()
        {
            this.InitializeComponent();

			Tapped += TaskTileControl_Tapped;
        }

		private void TaskTileControl_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (IsEnabled && TileTapped != null)
			{
				TileTapped(this, e);
			}
		}

		private void EnabledChanged()
        {
			var disabledBackgroundBrush = "ButtonDisabledBackgroundThemeBrush";
			var enabledBackgroundBrush = "SystemControlBackgroundAccentBrush";
			var disabledForegroundBrush = "ButtonDisabledForegroundThemeBrush";
			var enabledForegroundBrush = "SystemControlForegroundBaseHighBrush";
			var enabledBorderBrush = "SystemControlForegroundBaseHighBrush";
			var disabledBorderBrush = "ButtonDisabledForegroundThemeBrush";

			if (!Application.Current.Resources.ContainsKey(disabledBackgroundBrush) || 
				!Application.Current.Resources.ContainsKey(enabledBackgroundBrush) || 
				!Application.Current.Resources.ContainsKey(disabledForegroundBrush) || 
				!Application.Current.Resources.ContainsKey(enabledForegroundBrush) ||
				!Application.Current.Resources.ContainsKey(enabledBorderBrush) ||
				!Application.Current.Resources.ContainsKey(disabledBorderBrush))
			{
				return;
			}

			if (IsEnabled)
            {
				var backgroundBrush = new SolidColorBrush((Application.Current.Resources[enabledBackgroundBrush] as SolidColorBrush).Color);
                //var foregroundBrush = new SolidColorBrush((Application.Current.Resources[enabledForegroundBrush] as SolidColorBrush).Color);
                var foregroundBrush = new SolidColorBrush(Windows.UI.Colors.White);
				var borderBrush = new SolidColorBrush((Application.Current.Resources[enabledBorderBrush] as SolidColorBrush).Color);
				tileGrid.Background = backgroundBrush;
				tileBorder.BorderBrush = borderBrush;
				tbTaskTitle.Foreground = foregroundBrush;
				tbTaskTitle.FontWeight = FontWeights.Bold;
			}
            else
			{
				var backgroundBrush = new SolidColorBrush((Application.Current.Resources[disabledBackgroundBrush] as SolidColorBrush).Color);
				var foregroundBrush = new SolidColorBrush((Application.Current.Resources[disabledForegroundBrush] as SolidColorBrush).Color);
				var borderBrush = new SolidColorBrush((Application.Current.Resources[disabledBorderBrush] as SolidColorBrush).Color);
				tileGrid.Background = backgroundBrush;
				tileBorder.BorderBrush = borderBrush;
				tbTaskTitle.Foreground = foregroundBrush;
				tbTaskTitle.FontWeight = FontWeights.SemiLight;
			}
		}
    }
}
