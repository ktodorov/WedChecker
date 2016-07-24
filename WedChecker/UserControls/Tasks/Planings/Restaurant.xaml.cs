using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Planings
{
    public sealed partial class Restaurant : BaseTaskControl
    {
        public string RestaurantName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;


        public override string TaskName
        {
            get
            {
                return "Restaurant";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "You can add all kind of restaurant information here";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "Here is the restaurant info you have saved";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.Restaurant.ToString();
            }
        }

        public Restaurant()
        {
            this.InitializeComponent();
        }

        public Restaurant(Dictionary<string, string> parameters)
        {
            this.InitializeComponent();
            PopulateParameters(parameters);
            PopulateControls();
        }

        public override void DisplayValues()
        {
            DisplayPopulatedFields();
        }

        public override void EditValues()
        {
            EditAllFields();
        }


        public override void Serialize(BinaryWriter writer)
        {
            RestaurantName = tbName.Text;
            tbNameDisplay.Text = RestaurantName;

            Address = tbAddress.Text;
            tbAddressDisplay.Text = Address;

            Phone = tbPhone.Text;
            tbPhoneDisplay.Text = Phone;

            Notes = tbNotes.Text;
            tbNotesDisplay.Text = Notes;

            writer.Write(RestaurantName);
            writer.Write(Address);
            writer.Write(Phone);
            writer.Write(Notes);
        }

        public override void Deserialize(BinaryReader reader)
        {
            RestaurantName = reader.ReadString();
            Address = reader.ReadString();
            Phone = reader.ReadString();
            Notes = reader.ReadString();

            PopulateControls();
        }

        private void PopulateParameters(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("RestaurantName"))
            {
                RestaurantName = parameters["RestaurantName"];
            }
            if (parameters.ContainsKey("Address"))
            {
                Address = parameters["Address"];
            }
            if (parameters.ContainsKey("Phone"))
            {
                Phone = parameters["Phone"];
            }
            if (parameters.ContainsKey("Notes"))
            {
                Notes = parameters["Notes"];
            }
        }

        private void PopulateControls()
        {
            tbName.Text = RestaurantName;
            tbNameDisplay.Text = RestaurantName;

            tbAddress.Text = Address;
            tbAddressDisplay.Text = Address;

            tbPhone.Text = Phone;
            tbPhoneDisplay.Text = Phone;

            tbNotes.Text = Notes;
            tbNotesDisplay.Text = Notes;
        }

        private void DisplayPopulatedFields()
        {
            tbName.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(RestaurantName))
            {
                tbNameDisplay.Visibility = Visibility.Visible;
                tbNameHeader.Visibility = Visibility.Visible;
            }
            else
            {
                tbNameHeader.Visibility = Visibility.Collapsed;
            }

            tbAddress.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(Address))
            {
                tbAddressDisplay.Visibility = Visibility.Visible;
                tbAddressHeader.Visibility = Visibility.Visible;
            }
            else
            {
                tbAddressHeader.Visibility = Visibility.Collapsed;
            }

            tbPhone.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(Phone))
            {
                tbPhoneDisplay.Visibility = Visibility.Visible;
                tbPhoneHeader.Visibility = Visibility.Visible;
            }
            else
            {
                tbPhoneHeader.Visibility = Visibility.Collapsed;
            }

            tbNotes.Visibility = Visibility.Collapsed;
            if (!string.IsNullOrEmpty(Notes))
            {
                tbNotesDisplay.Visibility = Visibility.Visible;
                tbNotesHeader.Visibility = Visibility.Visible;
            }
            else
            {
                tbNotesHeader.Visibility = Visibility.Collapsed;
            }
        }

        public void EditAllFields()
        {
            tbName.Visibility = Visibility.Visible;
            tbNameDisplay.Visibility = Visibility.Collapsed;
            tbNameHeader.Visibility = Visibility.Visible;

            tbAddress.Visibility = Visibility.Visible;
            tbAddressDisplay.Visibility = Visibility.Collapsed;
            tbAddressHeader.Visibility = Visibility.Visible;

            tbPhone.Visibility = Visibility.Visible;
            tbPhoneDisplay.Visibility = Visibility.Collapsed;
            tbPhoneHeader.Visibility = Visibility.Visible;

            tbNotes.Visibility = Visibility.Visible;
            tbNotesDisplay.Visibility = Visibility.Collapsed;
            tbNotesHeader.Visibility = Visibility.Visible;
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            RestaurantName = tbName.Text;
            Address = tbAddress.Text;
            Phone = tbPhone.Text;
            Notes = tbNotes.Text;

            sb.AppendLine($"Name: {RestaurantName}");
            sb.AppendLine($"Address: {Address}");
            sb.AppendLine($"Phone: {Phone}");
            sb.AppendLine($"Notes: {Notes}");
        }
    }
}
