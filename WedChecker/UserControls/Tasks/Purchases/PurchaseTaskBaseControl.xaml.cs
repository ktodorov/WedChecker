using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Extensions;
using WedChecker.Interfaces;
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

namespace WedChecker.UserControls.Tasks.Purchases
{
    public abstract partial class PurchaseTaskBaseControl : BaseTaskControl, ICompletableTask
    {
        public static new string TaskName
        {
            get
            {
                return string.Empty;
            }
        }

        public override string EditHeader
        {
            get
            {
                return string.Empty;
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return string.Empty;
            }
        }

        public override string TaskCode
        {
            get
            {
                return string.Empty;
            }
        }

        protected virtual List<string> ItemsAppDataName
        {
            get;
        }

        protected virtual string ItemsMissingExceptionText
        {
            get;
        }

        protected StackPanel _itemsPanel;

        protected StackPanel ItemsPanel
        {
            get
            {
                if (_itemsPanel == null)
                {
                    _itemsPanel = this.FindName("mainPanel") as StackPanel;
                }

                return _itemsPanel;
            }
        }

        protected virtual List<string> CategoryNames
        {
            get;
        }

        protected virtual List<List<string>> PredefinedItems
        {
            get;
        }

        private List<List<string>> _storedItems;

        public List<List<string>> StoredItems
        {
            get
            {
                if (_storedItems == null || !_storedItems.Any() || _storedItems.Any(s => s == null))
                {
                    if (PredefinedItems != null)
                    {
                        _storedItems = PredefinedItems;
                    }
                    else
                    {
                        _storedItems = new List<List<string>>();
                        foreach (var itemAppDataName in ItemsAppDataName)
                        {
                            var items = AppData.GetStorage(itemAppDataName) as List<string>;
                            _storedItems.Add(items);
                        }
                    }
                }

                if (_storedItems == null || !_storedItems.Any() || _storedItems.Any(s => s == null))
                {
                    throw new WedCheckerInvalidDataException(ItemsMissingExceptionText);
                }

                return _storedItems;
            }
        }

        private bool hasCategories
        {
            get
            {
                return (CategoryNames != null && CategoryNames.Any());
            }
        }

        public PurchaseTaskBaseControl()
        {
            this.InitializeComponent();

            var category = -1;
            foreach (var itemsList in StoredItems)
            {
                if (itemsList == null)
                {
                    continue;
                }

                category++;
                int? categoryPosition = null;
                if (hasCategories)
                {
                    var categoryName = CategoryNames.ElementAt(category);
                    AddCategory(categoryName);
                    categoryPosition = category;
                }

                foreach (var item in itemsList)
                {
                    var toggle = new ToggleControl();
                    toggle.Title = item;
                    AddToggle(toggle, categoryPosition);
                }
            }
        }

        public override void DisplayValues()
        {
            if (hasCategories)
            {
                var categoryPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();

                foreach (var categoryPanel in categoryPanels)
                {
                    var toggles = categoryPanel.Children.OfType<ToggleControl>();
                    foreach (var toggle in toggles)
                    {
                        toggle.DisplayValues();
                    }
                }
            }
            else
            {
                var toggles = ItemsPanel.Children.OfType<ToggleControl>();
                foreach (var toggle in toggles)
                {
                    toggle.DisplayValues();
                }
            }
        }

        public override void EditValues()
        {
            if (hasCategories)
            {
                var categoryPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();

                foreach (var categoryPanel in categoryPanels)
                {
                    var toggles = categoryPanel.Children.OfType<ToggleControl>();
                    foreach (var toggle in toggles)
                    {
                        toggle.EditValues();
                    }
                }
            }
            else
            {
                var toggles = ItemsPanel.Children.OfType<ToggleControl>();
                foreach (var toggle in toggles)
                {
                    toggle.EditValues();
                }
            }
        }

        protected override void Serialize(BinaryWriter writer)
        {
            if (hasCategories)
            {
                var categoryPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();

                foreach (var categoryPanel in categoryPanels)
                {
                    var categoryNameBlock = categoryPanel.Children.OfType<TextBlock>().FirstOrDefault();
                    if (categoryNameBlock != null)
                    {
                        var categoryName = categoryNameBlock.Text;
                        writer.Write(categoryName);
                    }
                    else
                    {
                        writer.Write("Category");
                    }

                    var toggles = categoryPanel.Children.OfType<ToggleControl>().ToList();
                    writer.Write(toggles.Count);
                    foreach (var toggle in toggles)
                    {
                        toggle.Serialize(writer);
                    }
                }
            }
            else
            {
                var toggles = ItemsPanel.Children.OfType<ToggleControl>();
                writer.Write(toggles.Count());
                foreach (var toggle in toggles)
                {
                    toggle.Serialize(writer);
                }
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            if (hasCategories)
            {
                var categoriesCount = CategoryNames.Count;
                for (int i = 0; i < categoriesCount; i++)
                {
                    var type = reader.ReadString();
                    var count = reader.ReadInt32();

                    for (var j = 0; j < count; j++)
                    {
                        var toggle = new ToggleControl();
                        toggle.Deserialize(reader);

                        var categoryPosition = CategoryNames.IndexOf(type);
                        AddToggle(toggle, categoryPosition);
                    }
                }
            }
            else
            {
                var count = reader.ReadInt32();

                for (var j = 0; j < count; j++)
                {
                    var toggle = new ToggleControl();
                    toggle.Deserialize(reader);
                    AddToggle(toggle);
                }
            }
        }

        public int GetCompletedItems()
        {
            if (hasCategories)
            {
                var allPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();
                var completedTogglesCount = 0;

                foreach (var categoryPanel in allPanels)
                {
                    var completedToggles = categoryPanel.Children.OfType<ToggleControl>().Where(t => t.Toggled);
                    completedTogglesCount += completedToggles.Count();
                }

                return completedTogglesCount;
            }
            else
            {
                var completedToggles = ItemsPanel.Children.OfType<ToggleControl>().Where(t => t.Toggled);
                return completedToggles.Count();
            }
        }

        public int GetUncompletedItems()
        {
            if (hasCategories)
            {
                var allPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();
                var uncompletedTogglesCount = 0;

                foreach (var categoryPanel in allPanels)
                {
                    var uncompletedToggles = categoryPanel.Children.OfType<ToggleControl>().Where(t => !t.Toggled);
                    uncompletedTogglesCount += uncompletedToggles.Count();
                }

                return uncompletedTogglesCount;
            }
            else
            {
                var uncompletedToggles = ItemsPanel.Children.OfType<ToggleControl>().Where(t => !t.Toggled);
                return uncompletedToggles.Count();
            }
        }

        protected void AddCategory(string categoryName)
        {
            var categoryPanel = new StackPanel();
            categoryPanel.Orientation = Orientation.Vertical;
            categoryPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            categoryPanel.Margin = new Thickness(0, 0, 0, 10);

            var categoryTitle = new TextBlock();
            categoryTitle.Text = categoryName.SeparateCamelCase();
            var categoryStyle = Application.Current.Resources["WedCheckerDisplayTextBlockStyle"] as Style;
            categoryTitle.Style = categoryStyle;

            categoryPanel.Children.Add(categoryTitle);
            ItemsPanel.Children.Add(categoryPanel);
        }

        protected void AddToggle(ToggleControl toggle, int? category = null)
        {
            if (!StoredItems.Any(il => il.Contains(toggle.Title)))
            {
                return;
            }

            if (category.HasValue)
            {
                var allPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();
                if (allPanels.Count <= category.Value)
                {
                    return;
                }

                var categoryPanel = allPanels.ElementAt(category.Value);
                var allToggles = categoryPanel.Children.OfType<ToggleControl>();

                if (allToggles.Any(t => t.Title == toggle.Title))
                {
                    categoryPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                    return;
                }

                categoryPanel.Children.Add(toggle);
            }
            else
            {
                var allToggles = ItemsPanel.Children.OfType<ToggleControl>();

                if (allToggles.Any(t => t.Title == toggle.Title))
                {
                    ItemsPanel.Children.OfType<ToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                    return;
                }

                ItemsPanel.Children.Add(toggle);
            }
        }
    }
}
