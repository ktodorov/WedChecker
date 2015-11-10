﻿using System;
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

        private Dictionary<string, List<Contact>> GuestsPerPlaces = new Dictionary<string, List<Contact>>();
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

        public ForeignGuestsAccomodation(List<KeyValuePair<string, List<Contact>>> guests)
        {
            this.InitializeComponent();

            InitializeStoredInfo();

            if (guests != null)
            {
                foreach (var place in guests)
                {
                    // If the saved place is no longer chosen from AccomodationPlaces task, we don't display it here
                    if (!StoredPlaces.Any(p => p == place.Key))
                    {
                        continue;
                    }
                    AddPlace(place.Key);

                    foreach (var guest in place.Value)
                    {
                        // If the saved guest is no longer chosen from GuestsList task, we don't display it here
                        if (!StoredGuests.Any(g => g.Id == guest.Id))
                        {
                            continue;
                        }
                        AddGuestForPlace(place.Key, guest);
                    }
                }
            }
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
        }

        private void PopulateStoredGuests(string placeName)
        {
            storedGuestsPanel.Children.Clear();

            // We only take those that weren't already chosen
            var freeGuests = StoredGuests.Where(sg => !GuestsPerPlaces.Any(gp => gp.Value.Any(g => g.Id == sg.Id))).ToList();
            foreach (var guest in freeGuests)
            {
                var guestButton = new Button();
                guestButton.Content = $"{guest.FirstName} {guest.LastName}";
                guestButton.Tag = $"{placeName}{AppData.GLOBAL_SEPARATOR}{guest.Id}";
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
            var placeName = tags[0];
            var id = tags[1];

            AddGuestForPlace(placeName, StoredGuests.FirstOrDefault(g => g.Id == id));

            storedGuestsPanel.Visibility = Visibility.Collapsed;
        }

        public override void DisplayValues()
        {
            var nodeControls = spMain.Children.OfType<TreeNodeControl>();
            foreach (var nodeControl in nodeControls)
            {
                nodeControl.DisplayValues();
            }
        }

        public override void EditValues()
        {
            var nodeControls = spMain.Children.OfType<TreeNodeControl>();
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
            var placesCount = reader.ReadInt32();

            for (int i = 0; i < placesCount; i++)
            {
                var place = reader.ReadString();
                AddPlace(place);

                var guestsCount = reader.ReadInt32();

                for (int j = 0; j < guestsCount; j++)
                {
                    var contact = new Contact();
                    contact.Id = reader.ReadString();
                    contact.FirstName = reader.ReadString();
                    contact.LastName = reader.ReadString();
                    contact.Notes = reader.ReadString();

                    AddGuestForPlace(place, contact);
                }
            }

            DisplayValues();
        }

        private void ActualizeContactInformation()
        {
            var placeNodes = spMain.Children.OfType<TreeNodeControl>();
            foreach (var placeNode in placeNodes)
            {
                GuestsPerPlaces[placeNode.NodeName] = new List<Contact>();

                var contacts = placeNode.Nodes.OfType<ContactControl>();
                foreach (var contactControl in contacts)
                {
                    GuestsPerPlaces[placeNode.NodeName].Add(contactControl.StoredContact);
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
                spMain.Children.Add(placeNode);

                GuestsPerPlaces.Add(name, new List<Contact>());
            }
        }

        private void AddGuestForPlace(string place, Contact guest)
        {
            if (!GuestsPerPlaces.ContainsKey(place))
            {
                return;
            }

            if (!GuestsPerPlaces[place].Any(g => g.Id == guest.Id))
            {
                GuestsPerPlaces[place].Add(guest);

                var alongWith = 0;
                if (!int.TryParse(guest.Notes, out alongWith))
                {
                    alongWith = 0;
                }

                var guestControl = new ContactControl(guest, alongWith.ToString(), false);
                guestControl.deleteButton.Click += deleteGuestFromPlaceButton_Click;

                var placeNode = spMain.Children.OfType<TreeNodeControl>().Where(tn => tn.NodeName == place).FirstOrDefault();

                if (placeNode != null)
                {
                    placeNode.AddChildNode(guestControl);
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
            var contactControl = ((sender as Button).Parent as Grid).Parent as ContactControl;
            DeleteGuestFromPlace(contactControl.tbId.Text);
        }

        private async void DeleteGuestFromPlace(string id)
        {
            var placeToUse = GuestsPerPlaces.FirstOrDefault(p => p.Value.Any(g => g.Id == id)).Key;

            if (placeToUse != null)
            {
                GuestsPerPlaces[placeToUse].RemoveAll(g => g.Id == id);

                spMain.Children.OfType<TreeNodeControl>().FirstOrDefault(tn => tn.NodeName == placeToUse)
                                                         .RemoveChildNode(c => (c is ContactControl) && (c as ContactControl).StoredContact.Id == id);
            }

            await AppData.SerializeData();
        }
    }
}
