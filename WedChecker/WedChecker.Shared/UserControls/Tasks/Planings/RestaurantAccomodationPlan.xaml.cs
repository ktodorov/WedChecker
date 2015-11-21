using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class RestaurantAccomodationPlan : BaseTaskControl
    {

        private Dictionary<string, List<Contact>> GuestsPerTables = new Dictionary<string, List<Contact>>();
        private List<Contact> StoredGuests = new List<Contact>();
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

        public override string DisplayHeader
        {
            get
            {
                return "Here are your plannings about tables accomodation so far";
            }
        }

        public RestaurantAccomodationPlan()
        {
            this.InitializeComponent();

            InitializeStoredInfo();
        }

        private void InitializeStoredInfo()
        {
            var storedGuestsInfo = AppData.GetValue(TaskData.Tasks.GuestsList.ToString()).Split(new string[] { AppData.GLOBAL_SEPARATOR }, StringSplitOptions.None).ToList();
            for (var i = 0; i < storedGuestsInfo.Count / 4; i++)
            {
                var contact = new Contact();
                contact.Id = storedGuestsInfo[i * 4];
                contact.FirstName = storedGuestsInfo[i * 4 + 1];
                contact.LastName = storedGuestsInfo[i * 4 + 2];
                contact.Notes = storedGuestsInfo[i * 4 + 3];
                StoredGuests.Add(contact);
            }
        }

        private void InitializeTables()
        {
            StoredTables = new List<string>();

            for (int i = 1; i <= TablesCount; i++)
            {
                StoredTables.Add($"{TABLE_DISPLAY_NAME} {i}");
            }

            foreach (var table in StoredTables)
            {
                AddTable(table);
            }

            var currentTablesCount = GuestsPerTables.Keys.Count;
            if (currentTablesCount > TablesCount)
            {
                for (var i = TablesCount + 1; i <= currentTablesCount; i++)
                {
                    var tableToRemove = $"{TABLE_DISPLAY_NAME} {i}";
                    RemoveTable(tableToRemove);
                }
            }
        }

        private void AddChildButton_Click(object sender, RoutedEventArgs e)
        {
            var tableName = ((((sender as Button)?.Parent as Grid)?.Parent as Grid)?.Parent as TreeNodeControl)?.NodeName;
            if (tableName != null)
            {
                PopulateStoredGuests(tableName);
            }

            storedGuestsPanel.Visibility = Visibility.Visible;
        }

        private void PopulateStoredGuests(string tableName)
        {
            storedGuestsPanel.Children.Clear();

            // We only take those that weren't already chosen
            var freeGuests = StoredGuests.Where(sg => !GuestsPerTables.Any(gp => gp.Value.Any(g => g.Id == sg.Id))).ToList();
            foreach (var guest in freeGuests)
            {
                var guestButton = new Button();
                guestButton.Content = $"{guest.FirstName} {guest.LastName}";
                guestButton.Tag = $"{tableName}{AppData.GLOBAL_SEPARATOR}{guest.Id}";
                guestButton.Click += GuestButton_Click;

                object buttonStyle = Application.Current.Resources["WedCheckerTextButtonStyle"];
                if (buttonStyle != null && buttonStyle.GetType() == typeof(Style))
                {
                    guestButton.Style = (Style)buttonStyle;
                }

                storedGuestsPanel.Children.Add(guestButton);
            }

            if (freeGuests.Count == 0)
            {
                var tbNoGuests = new TextBlock();
                tbNoGuests.Text = "No free guests available!\nTry removing some and then try again.";
                tbNoGuests.HorizontalAlignment = HorizontalAlignment.Center;
                tbNoGuests.TextAlignment = TextAlignment.Center;
                tbNoGuests.FontSize = 15;

                storedGuestsPanel.Children.Add(tbNoGuests);
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            var tags = (sender as Button).Tag.ToString().Split(new string[] { AppData.GLOBAL_SEPARATOR }, StringSplitOptions.None);
            var tableName = tags[0];
            var id = tags[1];

            AddGuestForTable(tableName, StoredGuests.FirstOrDefault(g => g.Id == id));

            storedGuestsPanel.Visibility = Visibility.Collapsed;
        }

        public override void DisplayValues()
        {
            HideTablesCountFields();

            var nodeControls = spTables.Children.OfType<TreeNodeControl>();
            foreach (var nodeControl in nodeControls)
            {
                nodeControl.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var nodeControls = spTables.Children.OfType<TreeNodeControl>();
            foreach (var nodeControl in nodeControls)
            {
                nodeControl.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            ActualizeContactInformation();

            writer.Write(TaskData.Tasks.RestaurantAccomodationPlan.ToString());

            writer.Write(GuestsPerTables.Keys.Count);
            foreach (var table in GuestsPerTables)
            {
                writer.Write(table.Value.Count);

                foreach (var guest in table.Value)
                {
                    writer.Write(guest.Id);
                    writer.Write(guest.FirstName);
                    writer.Write(guest.LastName);
                    writer.Write(guest.Notes);
                }
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var tablesCount = reader.ReadInt32();

            TablesCount = tablesCount;

            for (int i = 1; i <= tablesCount; i++)
            {
                var tableName = $"{TABLE_DISPLAY_NAME} {i}";
                AddTable(tableName);

                var guestsCount = reader.ReadInt32();

                for (int j = 0; j < guestsCount; j++)
                {
                    var contact = new Contact();
                    contact.Id = reader.ReadString();
                    contact.FirstName = reader.ReadString();
                    contact.LastName = reader.ReadString();
                    contact.Notes = reader.ReadString();

                    AddGuestForTable(tableName, contact);
                }
            }

            DisplayValues();
        }

        private void ActualizeContactInformation()
        {
            var tableNodes = spTables.Children.OfType<TreeNodeControl>();
            foreach (var tableNode in tableNodes)
            {
                GuestsPerTables[tableNode.NodeName] = new List<Contact>();

                var contacts = tableNode.Nodes.OfType<ContactControl>();
                foreach (var contactControl in contacts)
                {
                    GuestsPerTables[tableNode.NodeName].Add(contactControl.StoredContact);
                }
            }
        }

        private void AddTable(string name)
        {
            if (!GuestsPerTables.ContainsKey(name))
            {
                var tableNode = new TreeNodeControl();
                tableNode.NodeName = name;
                tableNode.addChildButton.Click += AddChildButton_Click;
                spTables.Children.Add(tableNode);

                GuestsPerTables.Add(name, new List<Contact>());
            }
        }

        private void RemoveTable(string name)
        {
            GuestsPerTables.Remove(name);

            var tableNode = spTables.Children.OfType<TreeNodeControl>().FirstOrDefault(t => t.NodeName == name);
            if (tableNode != null)
            {
                spTables.Children.Remove(tableNode);
            }
        }

        private void AddGuestForTable(string table, Contact guest)
        {
            if (!GuestsPerTables.ContainsKey(table))
            {
                return;
            }

            if (!GuestsPerTables[table].Any(g => g.Id == guest.Id))
            {
                GuestsPerTables[table].Add(guest);

                var alongWith = 0;
                if (!int.TryParse(guest.Notes, out alongWith))
                {
                    alongWith = 0;
                }

                var guestControl = new ContactControl(guest, alongWith.ToString(), false);
                guestControl.deleteButton.Click += deleteGuestFromTableButton_Click;

                var tableNode = spTables.Children.OfType<TreeNodeControl>().Where(tn => tn.NodeName == table).FirstOrDefault();

                if (tableNode != null)
                {
                    tableNode.AddChildNode(guestControl);
                }
            }
        }

        public override async Task SubmitValues()
        {
            if (TablesCount <= 0)
            {
                return;
            }

            storedGuestsPanel.Visibility = Visibility.Collapsed;
            await AppData.InsertGlobalValue(TaskData.Tasks.RestaurantAccomodationPlan.ToString());
        }

        void deleteGuestFromTableButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGuestFromTable(contactControl.tbId.Text);
        }

        private async void DeleteGuestFromTable(string id)
        {
            var tableToUse = GuestsPerTables.FirstOrDefault(p => p.Value.Any(g => g.Id == id)).Key;

            if (tableToUse != null)
            {
                GuestsPerTables[tableToUse].RemoveAll(g => g.Id == id);

                spTables.Children.OfType<TreeNodeControl>().FirstOrDefault(tn => tn.NodeName == tableToUse)
                                                         .RemoveChildNode(c => (c is ContactControl) && (c as ContactControl).StoredContact.Id == id);
            }

            await AppData.SerializeData();
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
        }

        private void ShowTablesCountFields()
        {
            tbTablesCount.Text = TablesCount.ToString();
            spTablesCount.Visibility = Visibility.Visible;
            spTables.Visibility = Visibility.Collapsed;
        }
    }
}
