using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public partial class DocumentsRequired : BaseTaskControl
    {
        private Dictionary<int, string> Documents { get; set; } = new Dictionary<int, string>();

        private bool DocumentsChanged = false;

        public override string TaskName
        {
            get
            {
                return "Documents required";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "What are your required documents? You can add or remove them at any time";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "These are your documents";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.DocumentsRequired.ToString();
            }
        }

        public DocumentsRequired()
        {
            this.InitializeComponent();
        }

        public DocumentsRequired(Dictionary<int, string> values)
        {
            this.InitializeComponent();
            Documents = values;
            DocumentsChanged = false;
        }
        public override void DisplayValues()
        {
            foreach (var document in spDocuments.Children.OfType<ElementControl>())
            {
                document.DisplayValues();
            }
            addDocumentButton.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            foreach (var document in spDocuments.Children.OfType<ElementControl>())
            {
                document.EditValues();
            }
            addDocumentButton.Visibility = Visibility.Visible;
        }

        public override void Serialize(BinaryWriter writer)
        {
            foreach (var document in spDocuments.Children.OfType<ElementControl>())
            {
                SaveDocument(document);
            }

            writer.Write(Documents.Count);
            foreach (var document in Documents)
            {
                writer.Write(document.Value);
            }
            DocumentsChanged = false;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Documents = new Dictionary<int, string>();
            var size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                var document = reader.ReadString();
                Documents.Add(i, document);
            }

            foreach (var document in Documents)
            {
                spDocuments.Children.Add(new ElementControl(document.Key, document.Value));
            }
        }

        private int FindFirstFreeNumber()
        {
            var result = 0;

            if (Documents != null)
            {
                while (Documents.Keys.Any(k => k == result))
                {
                    result++;
                }
            }

            return result;
        }

        private void SaveDocument(ElementControl document)
        {
            if (!Documents.ContainsKey(document.Number) ||
                Documents[document.Number] != document.Title)
            {
                Documents[document.Number] = document.Title;
                DocumentsChanged = true;
            }
        }

        private void addDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber();

            var newDocument = new ElementControl(number, string.Empty);
            newDocument.removeElementButton.Click += removeDocumentButton_Click;
            Documents.Add(number, string.Empty);
            spDocuments.Children.Add(newDocument);
            DocumentsChanged = true;
        }

        private void removeDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var document = ((sender as Button).Parent as Grid).Parent as ElementControl;
            if (Documents != null)
            {
                Documents.Remove(document.Number);
            }

            spDocuments.Children.Remove(document);
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            foreach (var document in spDocuments.Children.OfType<ElementControl>())
            {
                SaveDocument(document);
            }

            foreach (var document in Documents)
            {
                sb.AppendLine(document.Value);
            }
        }
    }
}
