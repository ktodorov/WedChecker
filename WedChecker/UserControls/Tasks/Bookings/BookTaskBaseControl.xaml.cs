using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public partial class BookTaskBaseControl : BaseTaskControl, ICompletableTask, IPricedTask
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

        protected virtual string ItemsAppDataName
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

        protected virtual List<string> PredefinedItems
        {
            get;
        }

        private List<string> _storedItems;

        public List<string> StoredItems
        {
            get
            {
                if (_storedItems == null)
                {
                    if (PredefinedItems != null)
                    {
                        _storedItems = PredefinedItems;
                    }
                    else
                    {
                        _storedItems = AppData.GetStorage(ItemsAppDataName) as List<string>;
                    }
                }

                if (_storedItems == null || !_storedItems.Any())
                {
                    throw new WedCheckerInvalidDataException(ItemsMissingExceptionText);
                }

                return _storedItems;
            }
        }

        public BookTaskBaseControl()
        {
            this.InitializeComponent();

            foreach (var accomodationPlace in StoredItems)
            {
                var toggle = new PricedToggleControl();
                toggle.Title = accomodationPlace;
                AddToggle(toggle);
            }
        }


        public override void DisplayValues()
        {
            var toggles = ItemsPanel.Children.OfType<PricedToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var toggles = ItemsPanel.Children.OfType<PricedToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            var toggles = ItemsPanel.Children.OfType<PricedToggleControl>();
            writer.Write(toggles.Count());
            foreach (var toggle in toggles)
            {
                toggle.Serialize(writer);
            }
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var toggle = new PricedToggleControl();
                await toggle.Deserialize(reader);

                AddToggle(toggle);
            }
        }

        public int GetCompletedItems()
        {
            var completedToggles = ItemsPanel.Children.OfType<PricedToggleControl>().Where(t => t.Toggled);
            return completedToggles.Count();
        }

        public int GetUncompletedItems()
        {
            var uncompletedToggles = ItemsPanel.Children.OfType<PricedToggleControl>().Where(t => !t.Toggled);
            return uncompletedToggles.Count();
        }

        protected void AddToggle(PricedToggleControl toggle)
        {
            if (!StoredItems.Contains(toggle.Title))
            {
                return;
            }

            var allToggles = ItemsPanel.Children.OfType<PricedToggleControl>();

            if (allToggles.Any(t => t.Title == toggle.Title))
            {
                ItemsPanel.Children.OfType<PricedToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).Toggled = toggle.Toggled;
                ItemsPanel.Children.OfType<PricedToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).PurchaseValue = toggle.PurchaseValue;
                ItemsPanel.Children.OfType<PricedToggleControl>().FirstOrDefault(t => t.Title == toggle.Title).StoredPurchaseValue = toggle.StoredPurchaseValue;
                return;
            }

            ItemsPanel.Children.Add(toggle);
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            var toggles = ItemsPanel.Children.OfType<PricedToggleControl>().ToList();
            foreach (var toggle in toggles)
            {
                var toggleText = toggle.GetDataAsText();
                sb.AppendLine(toggleText);
            }
        }

        public double GetPurchasedItemsValue()
        {
            var toggles = ItemsPanel.Children.OfType<PricedToggleControl>().ToList();
            var purchaseValue = toggles.Where(t => t.Toggled && t.PurchaseValue.HasValue).Sum(t => t.PurchaseValue.Value);
            return purchaseValue;
        }
    }
}
