using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WedChecker.Common;
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

namespace WedChecker.UserControls.Tasks.Purchases
{
    public sealed partial class PurchaseFreshFlowers : BaseTaskControl
    {

        public override string TaskName
        {
            get
            {
                return "Fresh flowers";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Have you purchased your flowers yet?";
            }
        }

        public override string DisplayHeader
        {
            get
            {
                return "This is what you have saved about the flowers so far";
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

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.PurchaseFreshFlowers.ToString());

            var toggles = mainPanel.Children.OfType<ToggleControl>();
            writer.Write(toggles.Count());
            flowersPurchasedToggle.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            flowersPurchasedToggle.Deserialize(reader);

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            await AppData.InsertGlobalValue(TaskData.Tasks.PurchaseFreshFlowers.ToString());
        }
    }
}
