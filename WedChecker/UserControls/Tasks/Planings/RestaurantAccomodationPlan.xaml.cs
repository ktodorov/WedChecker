using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public sealed partial class RestaurantAccomodationPlan : BaseTaskControl
    {
        private List<string> StoredTables = new List<string>();
        private int TablesCount = 0;
        private const string TABLE_DISPLAY_NAME = "Table";

        public override string TaskName
        {
            get
            {
                return "Restaurant accomodation plan";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "You can plan the tables plan here";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "Here are your plannings about tables accomodation so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return Business.Models.Enums.Tasks.RestaurantAccomodationPlan.ToString();
            }
        }

        public RestaurantAccomodationPlan()
        {
            this.InitializeComponent();
        }

        private void InitializeTables()
        {
            StoredTables = new List<string>();

            for (int i = 1; i <= TablesCount; i++)
            {
                StoredTables.Add($"{TABLE_DISPLAY_NAME} {i}");
            }

            var currentTablesCount = guestsPerTable.StoredPlaces.Count;
            if (currentTablesCount > TablesCount)
            {
                for (var i = TablesCount + 1; i <= currentTablesCount; i++)
                {
                    var tableToRemove = $"{TABLE_DISPLAY_NAME} {i}";
                    StoredTables.Remove(tableToRemove);
                }
            }

            guestsPerTable.StoredPlaces = StoredTables;
        }

        public override void DisplayValues()
        {
            HideTablesCountFields();
            backToTablesCountButton.Visibility = Visibility.Collapsed;

            guestsPerTable.DisplayValues();
        }

        public override void EditValues()
        {
            backToTablesCountButton.Visibility = Visibility.Visible;

            guestsPerTable.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(2);

            writer.Write("TablesCount");
            writer.Write(TablesCount);

            writer.Write("GuestsPerTable");
            guestsPerTable.Serialize(writer);
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            for (var i = 0; i < objectsCount; i++)
            {
                var type = reader.ReadString();

                if (type == "TablesCount")
                {
                    TablesCount = reader.ReadInt32();
                }
                else if (type == "GuestsPerTable")
                {
                    guestsPerTable.Deserialize(reader);
                }
            }
        }

        private void confirmTablesCountButton_Click(object sender, RoutedEventArgs e)
        {
            var tablesCount = 0;
            if (!int.TryParse(tbTablesCount.Text, out tablesCount) || tablesCount <= 0)
            {
                return;
            }

            TablesCount = tablesCount;
            HideTablesCountFields();

            InitializeTables();
        }

        private void backToTablesCountButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTablesCountFields();
        }

        private void HideTablesCountFields()
        {
            spTablesCount.Visibility = Visibility.Collapsed;
            spTables.Visibility = Visibility.Visible;

            guestsPerTable.Visibility = Visibility.Visible;
        }

        private void ShowTablesCountFields()
        {
            tbTablesCount.Text = guestsPerTable.StoredPlaces.Count.ToString();
            spTablesCount.Visibility = Visibility.Visible;
            spTables.Visibility = Visibility.Collapsed;

            guestsPerTable.Visibility = Visibility.Visible;
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            sb.AppendLine($"Tables count: {TablesCount}");

            sb.AppendLine("Guests per table:");
            var guestsAccomodationAsText = guestsPerTable.GetDataAsText();
            sb.AppendLine(guestsAccomodationAsText);
        }
    }
}
