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
    public sealed partial class BestManMaidOfHonor : BaseTaskControl
    {

        private Contact BestMan
        {
            get
            {
                return ccBestMan.StoredContact;
            }
        }
        private Contact MaidOfHonor
        {
            get
            {
                return ccMaidOfHonor.StoredContact;
            }
        }


        public static new string TaskName
        {
            get
            {
                return "Best man and maid of honor";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "You can delete or select your best man and maid of honor again";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "Here are your best man and maid of honor";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.BestManMaidOfHonor.ToString();
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
                ccBestMan.StoreContact(contacts["BestMan"]);
            }

            if (contacts.ContainsKey("MaidOfHonor"))
            {
                ccMaidOfHonor.StoreContact(contacts["MaidOfHonor"]);
            }
        }

        public override void DisplayValues()
        {
            ccBestMan.DisplayValues();
            ccMaidOfHonor.DisplayValues();
        }

        public override void EditValues()
        {
            ccBestMan.EditValues();
            ccMaidOfHonor.EditValues();
        }

        protected override void Serialize(BinaryWriter writer)
        {
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
                ccBestMan.SerializeContact(writer);
            }
            if (MaidOfHonor != null)
            {
                writer.Write("MaidOfHonor");
                ccMaidOfHonor.SerializeContact(writer);
            }
        }

        protected override void Deserialize(BinaryReader reader)
        {
            //Read in the number of records
            var records = reader.ReadInt32();

            for (int i = 0; i < records; i++)
            {
                var type = reader.ReadString();

                if (type == "BestMan")
                {
                    ccBestMan.DeserializeContact(reader);
                }
                else if (type == "MaidOfHonor")
                {
                    ccMaidOfHonor.DeserializeContact(reader);
                }
            }
        }
    }
}
