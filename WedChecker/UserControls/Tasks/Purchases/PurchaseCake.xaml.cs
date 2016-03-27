using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WedChecker.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseCake : BaseTaskControl
    {

        public static new string TaskName
        {
            get
            {
                return "Wedding cake";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you purchased your cake yet?";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the cake so far";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.PurchaseCake.ToString();
            }
        }

        public PurchaseCake()
        {
            this.InitializeComponent();
        }

        public PurchaseCake(bool purchased)
        {
            this.InitializeComponent();

            cakePurchasedToggle.Toggled = true;
        }

        public override void DisplayValues()
        {
            cakePurchasedToggle.DisplayValues();
        }

        public override void EditValues()
        {
            cakePurchasedToggle.EditValues();
        }

        protected override void Serialize(BinaryWriter writer)
        {
            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            cakePurchasedToggle.Serialize(writer);
        }

        protected override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            cakePurchasedToggle.Deserialize(reader);
        }
    }
}
