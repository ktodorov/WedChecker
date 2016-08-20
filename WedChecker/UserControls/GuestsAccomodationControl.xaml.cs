using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Infrastructure;
using WedChecker.Interfaces;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class GuestsAccomodationControl : UserControl, IStorableTask
    {
        private List<string> _storedPlaces;
        public List<string> StoredPlaces
        {
            get
            {
                if (_storedPlaces == null)
                {
                    _storedPlaces = new List<string>();
                }

                return _storedPlaces;
            }
            set
            {
                _storedPlaces = value;

                foreach (var place in _storedPlaces)
                {
                    AddPlace(place);
                }

                RemoveUnusedPlaces();
            }
        }

        private Dictionary<string, List<ContactControl>> GuestsPerPlaces = new Dictionary<string, List<ContactControl>>();
        private List<Contact> StoredGuests = new List<Contact>();


        public GuestsAccomodationControl()
        {
            this.InitializeComponent();

            InitializeStoredInfo();
        }

        private void InitializeStoredInfo()
        {
            var storedGuests = AppData.Guests;
            if (storedGuests == null)
            {
                throw new Exception("No guests added. You must first add them from the Guest List planning task.");
            }

            StoredGuests.AddRange(storedGuests.Select(c => c.ToContact()));
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
            var contactControl = new ContactControl(guest, guest.Notes, false, false, false);

            AddGuestForPlace(placeName, contactControl);

            storedGuestsPanel.Visibility = Visibility.Collapsed;
            treeViewPanel.Visibility = Visibility.Visible;
        }

        public void DisplayValues()
        {
            var nodeControls = treeViewPanel.Children.OfType<TreeNodeControl>();
            foreach (var nodeControl in nodeControls)
            {
                nodeControl.DisplayValues();
            }
        }

        public void EditValues()
        {
            var nodeControls = treeViewPanel.Children.OfType<TreeNodeControl>();
            foreach (var nodeControl in nodeControls)
            {
                nodeControl.EditValues();
            }
        }
        public void Serialize(BinaryWriter writer)
        {
            storedGuestsPanel.Visibility = Visibility.Collapsed;
            ActualizeContactInformation();

            writer.Write(GuestsPerPlaces.Keys.Count);
            foreach (var place in GuestsPerPlaces)
            {
                writer.Write(place.Key); // Place name
                writer.Write(place.Value.Count);

                foreach (var guest in place.Value)
                {
                    guest.Serialize(writer);
                }
            }
        }

        public async Task Deserialize(BinaryReader reader)
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
                    contact.Deserialize(reader);

                    AddGuestForPlace(place, contact);
                }
            }

            StoredPlaces = GuestsPerPlaces.Keys.ToList();
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

        void deleteGuestFromPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = (sender as Button).FindAncestorByType<ContactControl>();
            if (contactControl == null)
            {
                return;
            }

            DeleteGuestFromPlace(contactControl.StoredContact.Id);
        }

        private void DeleteGuestFromPlace(string id)
        {
            var placeToUse = GuestsPerPlaces.FirstOrDefault(p => p.Value.Any(g => g.StoredContact.Id == id)).Key;

            if (placeToUse != null)
            {
                GuestsPerPlaces[placeToUse].RemoveAll(g => g.StoredContact.Id == id);

                treeViewPanel.Children.OfType<TreeNodeControl>().FirstOrDefault(tn => tn.NodeName == placeToUse)
                                                         .RemoveChildNode(c => (c is ContactControl) && (c as ContactControl).StoredContact.Id == id);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            treeViewPanel.Visibility = Visibility.Visible;
            storedGuestsPanel.Visibility = Visibility.Collapsed;
        }

        private void RemoveUnusedPlaces()
        {
            var unusedPlaces = GuestsPerPlaces.Where(gpp => !StoredPlaces.Contains(gpp.Key)).Select(g => g.Key);

            foreach (var place in unusedPlaces)
            {
                var nodeToRemove = treeViewPanel.Children.OfType<TreeNodeControl>().FirstOrDefault(tn => tn.NodeName == place);
                treeViewPanel.Children.Remove(nodeToRemove);
            }
        }

        public string GetDataAsText()
        {
            ActualizeContactInformation();

            var sb = new StringBuilder();
            sb.AppendLine("Accomodation");

            foreach (var place in GuestsPerPlaces)
            {
                sb.Append(" - ");
                sb.AppendLine(place.Key); // Place name

                foreach (var guest in place.Value)
                {
                    var contactAsText = guest.GetDataAsText();
                    sb.AppendLine(contactAsText);
                }
            }

            return sb.ToString();
        }
    }
}
