using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class TaskTileControl : UserControl
    {
		public event TappedEventHandler TileTapped;

		public TaskCategories Category;
		public TaskCategories TaskCategory
		{
			get
			{
				return (TaskCategories)GetValue(TaskCategoryProperty);
			}
			set
			{
				SetValue(TaskCategoryProperty, value);
				//tbTaskTitle.Text = value; //set the textbox content
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
				SetValue(TaskTitleProperty, value);
				tbTaskTitle.Text = value; //set the textbox content
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
				//tbTaskName.Text = value; //set the textbox content
			}
		}
		public static DependencyProperty TaskNameProperty = DependencyProperty.Register("TaskName", typeof(string), typeof(TaskTileControl), new PropertyMetadata(""));
		//public string TaskTitle
		//{
		//	get; set;
		//}
		//{
		//    get
		//    {
		//        return tbTaskTitle.Text;
		//    }
		//    set
		//    {
		//        tbTaskTitle.Text = value.ToUpper();
		//    }
		//}

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
			var enabledBackgroundBrush = "SystemControlBackgroundBaseHighBrush";
			var disabledForegroundBrush = "ButtonDisabledForegroundThemeBrush";
			var enabledForegroundBrush = "SystemControlForegroundAccentBrush";
			var enabledBorderBrush = "SystemControlForegroundAccentBrush";
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
				var foregroundBrush = new SolidColorBrush((Application.Current.Resources[enabledForegroundBrush] as SolidColorBrush).Color);
				var borderBrush = new SolidColorBrush((Application.Current.Resources[enabledBorderBrush] as SolidColorBrush).Color);
				tileGrid.Background = backgroundBrush;
				tileBorder.BorderBrush = borderBrush;
				tbTaskTitle.Foreground = foregroundBrush;
			}
            else
			{
				var backgroundBrush = new SolidColorBrush((Application.Current.Resources[disabledBackgroundBrush] as SolidColorBrush).Color);
				var foregroundBrush = new SolidColorBrush((Application.Current.Resources[disabledForegroundBrush] as SolidColorBrush).Color);
				var borderBrush = new SolidColorBrush((Application.Current.Resources[disabledBorderBrush] as SolidColorBrush).Color);
				tileGrid.Background = backgroundBrush;
				tileBorder.BorderBrush = borderBrush;
				tbTaskTitle.Foreground = foregroundBrush;
			}
        }
    }
}
