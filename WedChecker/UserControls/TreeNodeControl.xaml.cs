using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class TreeNodeControl : UserControl
    {
        private string _nodeName;

        public string NodeName
        {
            get
            {
                return _nodeName;
            }
            set
            {
                _nodeName = value;
                tbNodeName.Text = value;
            }
        }

        public List<UIElement> Nodes
        {
            get
            {
                return childNodesPanel.Children.ToList();
            }
            set
            {
                childNodesPanel.Children.Clear();
                if (value == null)
                {
                    return;
                }

                foreach (var node in value)
                {
                    childNodesPanel.Children.Add(node);
                }
            }
        }

        private bool _isAddable = true;
        public bool IsAddable
        {
            get
            {
                return _isAddable;
            }
            set
            {
                if (_isAddable == value)
                {
                    return;
                }

                if (value)
                {
                    addChildButton.Visibility = Visibility.Visible;
                }
                else
                {
                    addChildButton.Visibility = Visibility.Collapsed;
                }

                _isAddable = value;
            }
        }

        private bool _opened = false;
        public bool Opened
        {
            get
            {
                return _opened;
            }
            set
            {
                if (_opened == value)
                {
                    return;
                }

                _opened = value;

                ToggleNodesVisibility(value);
            }
        }

        public TreeNodeControl()
        {
            this.InitializeComponent();
        }

        public void AddChildNode(UIElement element)
        {
            childNodesPanel.Children.Add(element);
        }

        public void RemoveChildNode(Func<UIElement, bool> match)
        {
            var childToRemove = childNodesPanel.Children.FirstOrDefault(match);
            if (childToRemove != null)
            {
                childNodesPanel.Children.Remove(childToRemove);
            }
        }

        private void collapseButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleNodesVisibility();
        }

        private void ToggleNodesVisibility(bool? value = null)
        {
            if ((!value.HasValue && childNodesPanel.Visibility == Visibility.Visible) || (value.HasValue && !value.Value))
            {
                tbNodeSymbol.Text = "\uE09B";
                childNodesPanel.Visibility = Visibility.Collapsed;
            }
            else if (!value.HasValue || value.Value)
            {
                tbNodeSymbol.Text = "\uE09D";
                childNodesPanel.Visibility = Visibility.Visible;
            }
        }

        public void DisplayValues(bool applyForChildren = true)
        {
            if (!Nodes.Any())
            {
				tbNodeSymbol.Opacity = 0;
			}
            addChildButton.Visibility = Visibility.Collapsed;
			collapseButton.SetValue(Grid.ColumnSpanProperty, 2);

			if (applyForChildren)
            {
                foreach (var node in Nodes)
                {
                    Type nodeType = node.GetType();
                    MethodInfo method = nodeType.GetRuntimeMethod("DisplayValues", new Type[] { });
                    if (method != null)
                    {
                        method.Invoke(node, new object[] { });
                    }
                }
            }
        }

        public void EditValues(bool applyForChildren = true)
        {
            tbNodeSymbol.Opacity = 1;
            if (IsAddable)
            {
                addChildButton.Visibility = Visibility.Visible;
            }
            collapseButton.SetValue(Grid.ColumnSpanProperty, 1);

            if (applyForChildren)
            {
                foreach (var node in Nodes)
                {
                    Type nodeType = node.GetType();
                    MethodInfo method = nodeType.GetRuntimeMethod("EditValues", new Type[] { });
                    if (method != null)
                    {
                        method.Invoke(node, new object[] { });
                    }
                }

            }
        }
    }
}
