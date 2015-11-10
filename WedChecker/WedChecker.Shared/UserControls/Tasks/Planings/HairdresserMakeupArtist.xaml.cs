using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class HairdresserMakeupArtist : BaseTaskControl
    {

        public override string TaskName
        {
            get
            {
                return "Hairdresser and makeup artist";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Select the haidresser or the makeup artist or you know - both of them";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "Here is the info that you have provided about those two";
            }
        }

        public HairdresserMakeupArtist()
        {
            this.InitializeComponent();
        }

        public HairdresserMakeupArtist(Dictionary<string, Contact> values)
        {
            this.InitializeComponent();

            if (values.ContainsKey("hairdresser"))
            {
                ccHairdresser.StoreContact(values["hairdresser"]);
            }

            if (values.ContainsKey("makeupArtist"))
            {
                ccMakeupArtist.StoreContact(values["makeupArtist"]);
            }
        }

        public override void DisplayValues()
        {
            if (!ccHairdresser.IsStored)
            {
                hairdresserPanel.Visibility = Visibility.Collapsed;
            }

            if (!ccHairdresser.IsStored)
            {
                makeupArtistPanel.Visibility = Visibility.Collapsed;
            }

            selectHairdresser.Visibility = Visibility.Collapsed;
            selectMakeupArtist.Visibility = Visibility.Collapsed;
            ccHairdresser.DisplayValues();
            ccMakeupArtist.DisplayValues();
        }

        public override void EditValues()
        {
            hairdresserPanel.Visibility = Visibility.Visible;
            makeupArtistPanel.Visibility = Visibility.Visible;
            selectHairdresser.Visibility = Visibility.Visible;
            selectMakeupArtist.Visibility = Visibility.Visible;
            ccHairdresser.EditValues();
            ccMakeupArtist.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            if (!ccHairdresser.IsStored && !ccMakeupArtist.IsStored)
            {
                return;
            }

            writer.Write(TaskData.Tasks.HairdresserMakeupArtist.ToString());

            var storedContactsCount = 0;
            if (ccHairdresser.IsStored)
            {
                storedContactsCount++;
            }
            if (ccMakeupArtist.IsStored)
            {
                storedContactsCount++;
            }

            writer.Write(storedContactsCount);

            if (ccHairdresser.IsStored)
            {
                writer.Write("Hairdresser");
                ccHairdresser.SerializeContact(writer);
            }

            if (ccMakeupArtist.IsStored)
            {
                writer.Write("MakeupArtist");
                ccMakeupArtist.SerializeContact(writer);
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            var storedContactsCount = reader.ReadInt32();

            for (var i = 0; i < storedContactsCount; i++)
            {
                var contactType = reader.ReadString();

                if (contactType == "Hairdresser")
                {
                    ccHairdresser.DeserializeContact(reader);
                }
                else if (contactType == "MakeupArtist")
                {
                    ccMakeupArtist.DeserializeContact(reader);
                }
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.HairdresserMakeupArtist.ToString());
        }

        private async void selectHairdresser_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contact = await picker.PickContactAsync();

            if (contact == null)
            {
                return;
            }

            ccHairdresser.StoreContact(contact);
        }

        private async void selectMakeupArtist_Click(object sender, RoutedEventArgs e)
        {
            var picker = new ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);
            var contact = await picker.PickContactAsync();

            if (contact == null)
            {
                return;
            }

            ccMakeupArtist.StoreContact(contact);
        }
    }
}
