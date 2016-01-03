using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WedChecker.Common;

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

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.ForeignGuestsAccomodation.ToString();
            }
        }

        public ForeignGuestsAccomodation()
        {
            try
            {
                this.InitializeComponent();

            }
            catch (Exception)
            {
                var storedGuests = AppData.GetValue(TaskData.Tasks.GuestsList.ToString());
                if (storedGuests == null)
                {
                    throw new Exception("No guests added. You must first add them from the Guest List planning task.");
                }
            }

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
            writer.Write(TaskCode);

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
            await AppData.InsertGlobalValue(TaskCode);
        }
    }
}
