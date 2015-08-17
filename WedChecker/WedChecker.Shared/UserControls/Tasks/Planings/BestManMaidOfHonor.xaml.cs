using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
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

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class BestManMaidOfHonor : BaseTaskControl
    {

        private Contact BestMan;
        private Contact MaidOfHonor;


        public override string TaskName
        {
            get
            {
                return "Best man and maid of honor";
            }
        }

        public BestManMaidOfHonor()
        {
            this.InitializeComponent();
        }

        public BestManMaidOfHonor(Dictionary<string, Contact> contacts)
        {
            this.InitializeComponent();
            if (contacts.ContainsKey("BestMan"))
            {
                BestMan = contacts["BestMan"];
                var cBestMan = new ContactControl(BestMan.Id, BestMan.FirstName + " " + BestMan.LastName);
                cBestMan.deleteButton.Click += deleteBestManButton_Click;
                cBestMan.Visibility = Visibility.Visible;
                spMaidOfHonor.Children.Add(cBestMan);
            }

            if (contacts.ContainsKey("MaidOfHonor"))
            {
                MaidOfHonor = contacts["MaidOfHonor"];
                var cMaidOfHonor = new ContactControl(MaidOfHonor.Id, MaidOfHonor.FirstName + " " + MaidOfHonor.LastName);
                cMaidOfHonor.deleteButton.Click += deleteBestManButton_Click;
                cMaidOfHonor.Visibility = Visibility.Visible;
                spMaidOfHonor.Children.Add(cMaidOfHonor);
            }
        }

        public override void DisplayValues()
        {
            selectBestMan.Visibility = Visibility.Collapsed;
            selectMaidOfHonor.Visibility = Visibility.Collapsed;
            tbHeader.Text = "Here are your best man and maid of honor";
        }

        public override void EditValues()
        {
            selectBestMan.Visibility = Visibility.Visible;
            selectMaidOfHonor.Visibility = Visibility.Visible;
            tbHeader.Text = "You can delete or select your best man and maid of honor again ";
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.BestManMaidOfHonor.ToString());

            var count = 0;
            if (BestMan != null)
            {
                count++;
            }
            if (MaidOfHonor != null)
            {
                count++;
            }

            writer.Write(count);

            if (BestMan != null)
            {
                writer.Write("BestMan");
                writer.Write(BestMan.Id);
                writer.Write(BestMan.FirstName);
                writer.Write(BestMan.LastName);
            }
            if (MaidOfHonor != null)
            {
                writer.Write("MaidOfHonor");
                writer.Write(MaidOfHonor.Id);
                writer.Write(MaidOfHonor.FirstName);
                writer.Write(MaidOfHonor.LastName);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var records = reader.ReadInt32();

            for (int i = 0; i < records; i++)
            {
                var type = reader.ReadString();

                var guestId = reader.ReadString();

                var guestFirstName = reader.ReadString();
                var guestLastName = reader.ReadString();

                var contact = new Contact();
                contact.Id = guestId;
                contact.FirstName = guestFirstName;
                contact.LastName = guestLastName;

                if (type == "BestMan")
                {
                    DeleteBestMan();
                    BestMan = contact;
                    var cBestMan = new ContactControl(contact.Id, contact.FirstName + " " + contact.LastName);
                    cBestMan.deleteButton.Click += deleteBestManButton_Click;
                    cBestMan.Visibility = Visibility.Visible;
                    spBestMan.Children.Add(cBestMan);
                }
                else if (type == "MaidOfHonor")
                {
                    DeleteMaidOfHonor();
                    MaidOfHonor = contact;
                    var cMaidOfHonor = new ContactControl(contact.Id, contact.FirstName + " " + contact.LastName);
                    cMaidOfHonor.deleteButton.Click += deleteMaidOfHonorButton_Click;
                    cMaidOfHonor.Visibility = Visibility.Visible;
                    spMaidOfHonor.Children.Add(cMaidOfHonor);
                }
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.SerializeData();
        }

        async void deleteBestManButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteBestMan();
            await AppData.SerializeData();
        }

        async void deleteMaidOfHonorButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteMaidOfHonor();
            await AppData.SerializeData();
        }
        private void DeleteBestMan()
        {
            BestMan = null;
            var cBestMan = spBestMan.Children.OfType<ContactControl>().FirstOrDefault();
            spBestMan.Children.Remove(cBestMan);
        }

        private void DeleteMaidOfHonor()
        {
            MaidOfHonor = null;
            var cMaidOfHonor = spMaidOfHonor.Children.OfType<ContactControl>().FirstOrDefault();
            spMaidOfHonor.Children.Remove(cMaidOfHonor);
        }

        private async void selectBestMan_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contact = await picker.PickContactAsync();

            if (contact == null)
            {
                return;
            }
            DeleteBestMan();

            BestMan = contact;

            var contactControl = new ContactControl(contact.Id, contact.FirstName + " " + contact.LastName);
            contactControl.deleteButton.Click += deleteBestManButton_Click;
            contactControl.Visibility = Visibility.Visible;

            spBestMan.Children.Add(contactControl);
        }


        private async void selectMaidOfHonor_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contact = await picker.PickContactAsync();

            if (contact == null)
            {
                return;
            }

            DeleteMaidOfHonor();

            MaidOfHonor = contact;

            var contactControl = new ContactControl(contact.Id, contact.FirstName + " " + contact.LastName);
            contactControl.deleteButton.Click += deleteMaidOfHonorButton_Click;
            contactControl.Visibility = Visibility.Visible;

            DeleteMaidOfHonor();

            spBestMan.Children.Add(contactControl);
        }
    }
}
