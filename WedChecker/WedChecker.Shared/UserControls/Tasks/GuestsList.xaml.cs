using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Contacts;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public sealed partial class GuestsList : BaseTaskControl
    {
        private List<string> Guests
        {
            get;
            set;
        }

        public GuestsList()
        {
            this.InitializeComponent();
        }

        public GuestsList(int value)
        {
            this.InitializeComponent();
        }

        public override void DisplayValues(int value)
        {
           
        }

        public override void EditValues()
        {
           
        }

        private async void selectContacts_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.ApplicationModel.Contacts.ContactPicker();
            picker.DesiredFieldsWithContactFieldType.Add(Windows.ApplicationModel.Contacts.ContactFieldType.PhoneNumber);
            var contacts = await picker.PickContactsAsync();

            if (!contacts.Any())
            {
                return;
            }
            
            //Core.LocalSettings.Values["GuestsList"]   = contacts;
            //Core.RoamingSettings.Values["GuestsList"] = contacts;

            if (Guests == null)
            {
                Guests = new List<string>();
            }

            foreach (var contact in contacts)
            {
                Guests.Add(string.Format("{0} {1}", contact.FirstName, contact.LastName));
            }
        }


    }
}
