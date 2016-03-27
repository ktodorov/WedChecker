using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseFreshFlowers : BaseTaskControl
    {

        public static new string TaskName
        {
            get
            {
                return "Flowers";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you purchased your flowers yet?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the flowers so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.PurchaseFreshFlowers.ToString();
            }
        }

        public PurchaseFreshFlowers()
        {
            this.InitializeComponent();
        }

        public PurchaseFreshFlowers(bool purchased)
        {
            this.InitializeComponent();

            flowersPurchasedToggle.Toggled = true;
        }

        public override void DisplayValues()
        {
            flowersPurchasedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            flowersPurchasedToggle.EditValues();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            flowersPurchasedToggle.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            flowersPurchasedToggle.Deserialize(reader);
        }
    }
}
