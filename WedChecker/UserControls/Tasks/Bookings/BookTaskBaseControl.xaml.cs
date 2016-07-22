using System.Collections.Generic;
using System.IO;
using System.Linq;
using WedChecker.Common;
using WedChecker.Exceptions;
using WedChecker.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Bookings
{
    public partial class BookTaskBaseControl : BaseTaskControl, ICompletableTask
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
                var toggle = new ToggleControl();
                toggle.Title = accomodationPlace;
                AddToggle(toggle);
            }
        }


        public override void DisplayValues()
        {
            var toggles = ItemsPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var toggles = ItemsPanel.Children.OfType<ToggleControl>();
            foreach (var toggle in toggles)
            {
                toggle.EditValues();
            }
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = ItemsPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            foreach (var toggle in toggles)
            {
                toggle.Serialize(writer);
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var toggle = new ToggleControl();
                toggle.Deserialize(reader);

                AddToggle(toggle);
            }
        }

        public int GetCompletedItems()
        {
            var completedToggles = ItemsPanel.Children.OfType<ToggleControl>().Where(t => t.Toggled);
            return completedToggles.Count();
        }

        public int GetUncompletedItems()
        {
            var uncompletedToggles = ItemsPanel.Children.OfType<ToggleControl>().Where(t => !t.Toggled);
            return uncompletedToggles.Count();
        }

        protected void AddToggle(ToggleControl toggle)
        {
            if (!StoredItems.Contains(toggle.Title))
            {
                return;
            }

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
