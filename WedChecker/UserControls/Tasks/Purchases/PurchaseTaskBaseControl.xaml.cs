using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Extensions;
using WedChecker.Interfaces;
using WedChecker.UserControls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public abstract partial class PurchaseTaskBaseControl : BaseTaskControl, ICompletableTask, IPurchaseableTask
    {
        public override string TaskName
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

        public List<PurchaseToggleControl> Toggles
        {
            get
            {
                var toggles = new List<PurchaseToggleControl>();
                if (hasCategories)
                {
                    var categoryPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();

                    foreach (var categoryPanel in categoryPanels)
                    {
                        toggles.AddRange(categoryPanel.Children.OfType<PurchaseToggleControl>().ToList());
                    }
                }
                else
                {
                    toggles = ItemsPanel.Children.OfType<PurchaseToggleControl>().ToList();
                }

                return toggles;
            }
        }

        public Dictionary<string, List<PurchaseToggleControl>> TogglesByCategories
        {
            get
            {
                var result = new Dictionary<string, List<PurchaseToggleControl>>();
                if (hasCategories)
                {
                    var categoryPanels = ItemsPanel.Children.OfType<StackPanel>().ToList();

                    foreach (var categoryPanel in categoryPanels)
                    {
                        var categoryNameBlock = categoryPanel.Children.OfType<TextBlock>().FirstOrDefault();
                        var categoryName = categoryNameBlock.Text;
                        var toggles = categoryPanel.Children.OfType<PurchaseToggleControl>().ToList();
                        if (!result.ContainsKey(categoryName))
                        {
                            result.Add(categoryName, toggles);
                        }
                        else
                        {
                            result[categoryName].AddRange(toggles);
                        }
                    }
                }
                else
                {
                    var toggles = ItemsPanel.Children.OfType<PurchaseToggleControl>().ToList();
                    result.Add(string.Empty, toggles);
                }

                return result;
            }
        }

        public PurchaseTaskBaseControl()
        {
            this.InitializeComponent();
            this.DataContext = this;

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
                    var toggle = new PurchaseToggleControl();
                    toggle.Title = item;
                    AddToggle(toggle, categoryPosition);
                }
            }
        }

        public override void DisplayValues()
        {
            var toggles = Toggles;
            foreach (var toggle in toggles)
            {
                toggle.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var toggles = Toggles;
            foreach (var toggle in toggles)
            {
                toggle.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
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

                    var toggles = categoryPanel.Children.OfType<PurchaseToggleControl>().ToList();
                    writer.Write(toggles.Count);
                    foreach (var toggle in toggles)
                    {
                        toggle.Serialize(writer);
                    }
                }
            }
            else
            {
                var toggles = ItemsPanel.Children.OfType<PurchaseToggleControl>();
                writer.Write(toggles.Count());
                foreach (var toggle in toggles)
                {
                    toggle.Serialize(writer);
                }
            }
        }

        public override async Task Deserialize(BinaryReader reader)
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
                        var toggle = new PurchaseToggleControl();
                        toggle.Deserialize(reader);
                        var categoryPosition = FindIndex(type);
                        AddToggle(toggle, categoryPosition);
                    }
                }
            }
            else
            {
                var count = reader.ReadInt32();

                for (var j = 0; j < count; j++)
                {
                    var toggle = new PurchaseToggleControl();
                        toggle.Deserialize(reader);
                    AddToggle(toggle);
                }
            }
        }

        public int FindIndex(string category)
        {
            var categories = CategoryNames.Select(c => c.SeparateCamelCase().ToLower()).ToList();
            var index = categories.IndexOf(category.SeparateCamelCase().ToLower());
            return index;
        }

        public int GetCompletedItems()
        {
            var toggles = Toggles;
            var completedToggles = toggles.Where(t => t.Toggled);
            return completedToggles.Count();
        }

        public int GetUncompletedItems()
        {
            var toggles = Toggles;
            var completedToggles = toggles.Where(t => !t.Toggled);
            return completedToggles.Count();
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

        protected void AddToggle(PurchaseToggleControl toggle, int? category = null)
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
                var allToggles = categoryPanel.Children.OfType<PurchaseToggleControl>();

                if (allToggles.Any(t => t.Title == toggle.Title))
                {
                    categoryPanel.Children.OfType<PurchaseToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                    categoryPanel.Children.OfType<PurchaseToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).PurchaseValue = toggle.PurchaseValue;
                    return;
                }

                categoryPanel.Children.Add(toggle);
            }
            else
            {
                var allToggles = ItemsPanel.Children.OfType<PurchaseToggleControl>();

                if (allToggles.Any(t => t.Title == toggle.Title))
                {
                    ItemsPanel.Children.OfType<PurchaseToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                    ItemsPanel.Children.OfType<PurchaseToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).PurchaseValue = toggle.PurchaseValue;
                    return;
                }

                ItemsPanel.Children.Add(toggle);
            }
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            var togglesByCategories = TogglesByCategories;
            foreach (var toggleCategory in togglesByCategories)
            {
                if (!string.IsNullOrEmpty(toggleCategory.Key))
                {
                    sb.AppendLine(toggleCategory.Key);
                }

                var toggles = toggleCategory.Value;
                foreach (var toggle in toggles)
                {
                    var toggleText = toggle.GetDataAsText();
                    sb.Append(toggleText);
                }
            }
        }

        public double GetPurchasedItemsValue()
        {
            var purchaseValue = Toggles.Where(t => t.Toggled && t.PurchaseValue.HasValue).Sum(t => t.PurchaseValue.Value);
            return purchaseValue;
        }
    }
}
