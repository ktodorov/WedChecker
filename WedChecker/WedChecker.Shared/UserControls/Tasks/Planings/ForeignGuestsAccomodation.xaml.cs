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
    public sealed partial class ForeignGuestsAccomodation : BaseTaskControl
    {

        private Dictionary<string, List<ContactControl>> GuestsPerPlaces = new Dictionary<string, List<ContactControl>>();
        private List<Contact> StoredGuests = new List<Contact>();
        private List<string> StoredPlaces = new List<string>();
        public override string TaskName
        {
            get
            {
                return "Foreign guests accomodation";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "You can choose where your foreign guests will sleep here";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "Here are your plannings about accomodation so far";
            }
        }

        public ForeignGuestsAccomodation()
        {
            this.InitializeComponent();
            
            InitializeStoredInfo();

            foreach (var place in StoredPlaces)
            {
                AddPlace(place);
            }
        }

        private void InitializeStoredInfo()
        {
            var storedGuests = AppData.GetValue(TaskData.Tasks.GuestsList.ToString());
            if (storedGuests == null)
            {
                throw new WedCheckerInvalidDataException("No guests added. You must first add them from the Guest List planning task.");
            }
            var storedGuestsInfo = storedGuests.Split(new string[] { AppData.GLOBAL_SEPARATOR }, StringSplitOptions.None).ToList();
            for (var i = 0; i < storedGuestsInfo.Count / 4; i++)
            {
                var contact = new Contact();
                contact.Id = storedGuestsInfo[i * 4];
                contact.FirstName = storedGuestsInfo[i * 4 + 1];
                contact.LastName = storedGuestsInfo[i * 4 + 2];
                contact.Notes = storedGuestsInfo[i * 4 + 3];
                StoredGuests.Add(contact);
            }

            StoredPlaces = AppData.GetGlobalValues(TaskData.Tasks.AccomodationPlaces.ToString());
        }

        private void AddChildButton_Click(object sender, RoutedEventArgs e)
        {
            var placeName = ((((sender as Button)?.Parent as Grid)?.Parent as Grid)?.Parent as TreeNodeControl)?.NodeName;
            if (placeName != null)
            {
                PopulateStoredGuests(placeName);
            }

            storedGuestsPanel.Visibility = Visibility.Visible;
            treeViewPanel.Visibility = Visibility.Collapsed;
        }

        private void PopulateStoredGuests(string placeName)
        {
            spGuests.Children.Clear();

            // We only take those that weren't already chosen
            var freeGuests = StoredGuests.Where(sg => !GuestsPerPlaces.Any(gp => gp.Value.Any(g => g.StoredContact.Id == sg.Id))).ToList();
            foreach (var guest in freeGuests)
            {
                var guestButton = new Button();
                guestButton.Content = $"{guest.FirstName} {guest.LastName}";
                guestButton.Tag = $"{placeName}{AppData.GLOBAL_SEPARATOR}{guest.Id}";
                guestButton.Click += GuestButton_Click;
                guestButton.HorizontalAlignment = HorizontalAlignment.Stretch;

                object buttonStyle = Application.Current.Resources["WedCheckerTextButtonStyle"];
                if (buttonStyle != null && buttonStyle.GetType() == typeof(Style))
                {
                    guestButton.Style = (Style)buttonStyle;
                }

                spGuests.Children.Add(guestButton);
            }

            if (freeGuests.Count == 0)
            {
                var tbNoGuests = new TextBlock();
                tbNoGuests.Text = "No free guests available!\nTry removing some and then try again.";
                tbNoGuests.HorizontalAlignment = HorizontalAlignment.Center;
                tbNoGuests.TextAlignment = TextAlignment.Center;
                tbNoGuests.FontSize = 15;

                spGuests.Children.Add(tbNoGuests);
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            var tags = (sender as Button).Tag.ToString().Split(new string[] { AppData.GLOBAL_SEPARATOR }, StringSplitOptions.None);
            var placeName = tags[0];
            var id = tags[1];

            var guest = StoredGuests.FirstOrDefault(g => g.Id == id);
            var contactControl = new ContactControl(guest, guest.Notes, true, false);
            
            AddGuestForPlace(placeName, contactControl);

            storedGuestsPanel.Visibility = Visibility.Collapsed;
            treeViewPanel.Visibility = Visibility.Visible;
        }

        public override void DisplayValues()
        {
            var nodeControls = treeViewPanel.Children.OfType<TreeNodeControl>();
            foreach (var nodeControl in nodeControls)
            {
                nodeControl.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var nodeControls = treeViewPanel.Children.OfType<TreeNodeControl>();
            foreach (var nodeControl in nodeControls)
            {
                nodeControl.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            ActualizeContactInformation();

            writer.Write(TaskData.Tasks.ForeignGuestsAccomodation.ToString());

            writer.Write(GuestsPerPlaces.Keys.Count);
            foreach (var place in GuestsPerPlaces)
            {
                writer.Write(place.Key); // Place name
                writer.Write(place.Value.Count);

                foreach (var guest in place.Value)
                {
                    guest.SerializeContact(writer);
                }
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var placesCount = reader.ReadInt32();

            for (int i = 0; i < placesCount; i++)
            {
                var place = reader.ReadString();
                AddPlace(place);

                var guestsCount = reader.ReadInt32();

                for (int j = 0; j < guestsCount; j++)
                {
                    var contact = new ContactControl();
                    contact.DeserializeContact(reader);

                    AddGuestForPlace(place, contact);
                }
            }

            DisplayValues();
        }

        private void ActualizeContactInformation()
        {
            var placeNodes = treeViewPanel.Children.OfType<TreeNodeControl>();
            foreach (var placeNode in placeNodes)
            {
                GuestsPerPlaces[placeNode.NodeName] = new List<ContactControl>();

                var contacts = placeNode.Nodes.OfType<ContactControl>();
                foreach (var contactControl in contacts)
                {
                    GuestsPerPlaces[placeNode.NodeName].Add(contactControl);
                }
            }
        }

        private void AddPlace(string name)
        {
            if (!GuestsPerPlaces.ContainsKey(name))
            {
                var placeNode = new TreeNodeControl();
                placeNode.NodeName = name;
                placeNode.addChildButton.Click += AddChildButton_Click;
                treeViewPanel.Children.Add(placeNode);

                GuestsPerPlaces.Add(name, new List<ContactControl>());
            }
        }

        private void AddGuestForPlace(string place, ContactControl contactControl)
        {
            if (!GuestsPerPlaces.ContainsKey(place))
            {
                return;
            }

            if (!GuestsPerPlaces[place].Any(g => g.StoredContact.Id == contactControl.StoredContact.Id))
            {
                GuestsPerPlaces[place].Add(contactControl);

                var alongWith = 0;
                if (!int.TryParse(contactControl.StoredContact.Notes, out alongWith))
                {
                    alongWith = 0;
                }

                contactControl.OnDelete = deleteGuestFromPlaceButton_Click;

                var placeNode = treeViewPanel.Children.OfType<TreeNodeControl>().Where(tn => tn.NodeName == place).FirstOrDefault();

                if (placeNode != null)
                {
                    placeNode.AddChildNode(contactControl);
                }
            }
        }

        public override async Task SubmitValues()
        {
            storedGuestsPanel.Visibility = Visibility.Collapsed;
            await AppData.InsertGlobalValue(TaskData.Tasks.ForeignGuestsAccomodation.ToString());
        }

        void deleteGuestFromPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = (sender as Button).FindAncestorByType(typeof(ContactControl)) as ContactControl;
            if (contactControl == null)
            {
                return;
            }
            //var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGuestFromPlace(contactControl.StoredContact.Id);
        }

        private async void DeleteGuestFromPlace(string id)
        {
            var placeToUse = GuestsPerPlaces.FirstOrDefault(p => p.Value.Any(g => g.StoredContact.Id == id)).Key;

            if (placeToUse != null)
            {
                GuestsPerPlaces[placeToUse].RemoveAll(g => g.StoredContact.Id == id);

                treeViewPanel.Children.OfType<TreeNodeControl>().FirstOrDefault(tn => tn.NodeName == placeToUse)
                                                         .RemoveChildNode(c => (c is ContactControl) && (c as ContactControl).StoredContact.Id == id);
            }

            await AppData.SerializeData();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            treeViewPanel.Visibility = Visibility.Visible;
            storedGuestsPanel.Visibility = Visibility.Collapsed;
        }
    }
}
