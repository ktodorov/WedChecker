using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public abstract partial class BaseTaskControl : UserControl
    {
        public BaseTaskControl()
        {
            this.InitializeComponent();
        }

        public virtual string TaskName
        {
            get;
        }

        public virtual string EditHeader { get; }
        public virtual string DisplayHeader { get; }
        public virtual string TaskCode { get; }

        public string ErrorMessage { get; set; }


        public virtual void DisplayValues()
        {

        }

        public virtual void EditValues()
        {

        }

        public virtual void Serialize(BinaryWriter writer)
        {

        }

        public virtual void Deserialize(BinaryReader reader)
        {
        }

        public virtual async Task SubmitValues()
        {
        }
    }
}
