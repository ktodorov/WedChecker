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
        private List<string> StoredAccomodationPlaces = new List<string>();

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
        }

        private void InitializeStoredInfo()
        {
            StoredAccomodationPlaces = AppData.GetGlobalValues(TaskData.Tasks.AccomodationPlaces.ToString());

            guestsPerHotel.StoredPlaces = StoredAccomodationPlaces;
        }

        public override void DisplayValues()
        {
            guestsPerHotel.DisplayValues();
        }

        public override void EditValues()
        {
            guestsPerHotel.EditValues();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.ForeignGuestsAccomodation.ToString());

            writer.Write(1);
            writer.Write("GuestsPerHotel");
            guestsPerHotel.SerializeData(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            for (var i = 0; i < objectsCount; i++)
            {
                var type = reader.ReadString();
                
                if (type == "GuestsPerHotel")
                {
                    guestsPerHotel.DeserializeData(reader);
                }
            }

            DisplayValues();
        }


        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.ForeignGuestsAccomodation.ToString());
        }
    }
}
