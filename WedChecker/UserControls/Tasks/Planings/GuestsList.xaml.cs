using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Contacts;
using WedChecker.Common;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Text;
using WedChecker.Extensions;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public class ContactCategory
    {
        public char? Title { get; set; }
        public List<ContactControl> ContactControls { get; set; }
    }

    public sealed partial class GuestsList : BaseTaskControl
    {
        public List<ContactControl> Guests
        {
            get
            {
                return contactsList;
            }
        }

        public override string TaskName
        {
            get
            {
                return "Guests list";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can select the guests for your weddings";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "These are the guests you have added so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return Business.Models.Enums.Tasks.GuestsList.ToString();
            }
        }

        public override bool HasOwnScrollViewer
        {
            get
            {
                return true;
            }
        }

        public GuestsList()
        {
            this.InitializeComponent();
            contactsList = new List<ContactControl>();

            mainGuestsGrid.Children.Remove(editPanel);
            editPanelTreeNode.Nodes = new List<UIElement> { editPanel };
        }


        List<ContactControl> contactsList;
        ObservableCollection<ContactCategory> contactCategoryList;

        private void GroupContacts()
        {
            contactItemsViewSource.Source = null;
            contactCategoryList = new ObservableCollection<ContactCategory>();

            var taskGroups = contactsList.OrderBy(c => c.StoredContact?.FullName.FirstLetter()).GroupBy(c => c.StoredContact?.FullName.FirstLetter());
            foreach (var item in taskGroups)
            {
                contactCategoryList.Add(new ContactCategory() { Title = item.Key.ToString().FirstLetter(), ContactControls = item.ToList<ContactControl>() });
            }

            contactItemsViewSource.Source = contactCategoryList;
            ZoomedOutGridView.ItemsSource = contactCategoryList;
        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (ZoomedOutGridView != null && ZoomedOutGridView.Items != null && tasksGridView != null && tasksGridView.Items != null)
            {
                tasksGridView.ScrollIntoView(tasksGridView.Items[0], ScrollIntoViewAlignment.Default);
                if (e.IsSourceZoomedInView == false)
                {
                    var index = 0;
                    var sourceItem = e.SourceItem.Item as ContactCategory;
                    var category = sourceItem.Title;
                    if (tasksGridView.Items != null)
                    {
                        var item = tasksGridView.Items[index] as ContactControl;
                        while (item == null || item.StoredContact.FullName.FirstLetter() != category)
                        {
                            index++;
                            item = tasksGridView.Items[index] as ContactControl;
                        }

                        e.DestinationItem.Item = item;
                    }
                }
            }
        }

        public override void DisplayValues()
        {
            editPanelTreeNode.Visibility = Visibility.Collapsed;

            foreach (var guestControl in contactsList)
            {
                guestControl.DisplayValues();
            }
        }

        public override void EditValues()
        {
            editPanelTreeNode.Visibility = Visibility.Visible;

            foreach (var guestControl in contactsList)
            {
                guestControl.EditValues();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Guests.Count);
            foreach (var guest in Guests)
            {
                guest.Serialize(writer);
            }

            AppData.Guests = Guests.Select(c => c.StoredContact).ToList();
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var records = reader.ReadInt32();

            for (long i = 0; i < records; i++)
            {
                var contactControl = new ContactControl(true, true, true);

                await contactControl.Deserialize(reader);
                contactControl.OnDelete = deleteButton_Click;

                contactsList.Add(contactControl);
            }

            tbGuestsAdded.Text = string.Format("{0} guests added", Guests.Count);

            GroupContacts();

            AppData.Guests = contactsList.Select(c => c.StoredContact).ToList();
        }

        private async void selectContacts_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contacts = await picker.PickContactsAsync();

            if (contacts == null || !contacts.Any())
            {
                return;
            }

            AddNewContacts(contacts);
           
            tbGuestsAdded.Text = string.Format("{0} guests added", Guests.Count);
        }

        protected override void SetLocalStorage()
        {
            var guestsContacts = Guests.Select(g => g.StoredContact).ToList();
            AppData.SetStorage("GuestsList", guestsContacts);
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = (sender as FrameworkElement).FindAncestorByType<ContactControl>();
            DeleteGuest(contactControl.StoredContact.Id);
        }

        private void DeleteGuest(string id)
        {
            var guestToRemove = Guests.FirstOrDefault(g => g.StoredContact.Id == id);
            if (guestToRemove == null)
            {
                return;
            }

            contactsList.Remove(guestToRemove);
            var matchingContactCategory = contactCategoryList.FirstOrDefault(cc => cc.ContactControls.Any(c => c.StoredContact.Id == guestToRemove.StoredContact.Id));
            if (matchingContactCategory != null)
            {
                matchingContactCategory.ContactControls.Remove(guestToRemove);
                GroupContacts();
            }
        }

        private void addNewContactButton_Click(object sender, RoutedEventArgs e)
        {
            var contactControl = new ContactControl(true, true, true);
            contactControl.IncludeName = true;
            contactControl.IncludePhones = true;
            contactControl.IncludeEmail = true;
            contactControl.IncludeAlongWith = true;
            contactControl.OnDelete = deleteButton_Click;

            AddNewContactControl(contactControl);
        }
        
        private void AddNewContactControl(ContactControl contactControl)
        {
            contactsList.Add(contactControl);

            GroupContacts();
        }

        private void AddNewContacts(IList<Contact> contacts)
        {
            var addedContactControls = new List<ContactControl>();
            foreach (var contact in contacts)
            {
                if (!Guests.Any(g => g.StoredContact.Id == contact.Id))
                {
                    var contactControl = new ContactControl(contact, null, true, true, true);
                    contactControl.OnDelete = deleteButton_Click;

                    contactsList.Add(contactControl);
                    addedContactControls.Add(contactControl);
                }
            }

            GroupContacts();
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            foreach (var guest in Guests)
            {
                var contactAsText = guest.GetDataAsText();
                sb.AppendLine(contactAsText);
            }
        }

        private void RecalculateContactWidth()
        {
            var parentWidth = tasksGridView.ActualWidth;
            contactsList.ForEach(c => c.Width = parentWidth);
        }

        private void tasksGridView_Loaded(object sender, RoutedEventArgs e)
        {
            RecalculateContactWidth();
        }

        private void tasksGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateContactWidth();
        }

        public override void SetOwnScrollViewerHeight(double maxHeight, double maxWidth)
        {
            maxHeight -= editPanelTreeNode.ActualHeight + tbGuestsAdded.ActualHeight + 10;

            semanticZoom.MaxHeight = maxHeight;
            semanticZoom.MaxWidth = maxWidth;
        }
    }
}
