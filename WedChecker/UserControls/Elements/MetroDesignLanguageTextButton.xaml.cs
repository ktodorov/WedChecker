using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace WedChecker.UserControls.Elements
{
	public sealed partial class MetroDesignLanguageTextButton : UserControl
	{
		public string IconContent
		{
			get
			{
				return iconButton.Text;
			}
			set
			{
				iconButton.Text = value;
			}
		}

		public bool ShowIcon
		{
			set
			{
				iconButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public double IconSize
		{
			get
			{
				return iconButton.FontSize;
			}
			set
			{
				iconButton.FontSize = value;
			}
		}


		public string Text
		{
			get
			{
				return buttonText.Text;
			}
			set
			{
				buttonText.Text = value;
			}
		}

		public MetroDesignLanguageTextButton()
		{
			this.InitializeComponent();
		}
	}
}
