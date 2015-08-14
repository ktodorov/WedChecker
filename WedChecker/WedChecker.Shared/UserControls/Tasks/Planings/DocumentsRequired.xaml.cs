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

namespace WedChecker.UserControls.Tasks.Planings
{
    public partial class DocumentsRequired : BaseTaskControl
    {
        private Dictionary<int, string> Documents
        {
            get;
            set;
        }

        private bool DocumentsChanged = false;

        public override string TaskName
        {
            get
            {
                return "Documents required";
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
            foreach(var document in spDocuments.Children.OfType<DocumentControl>())
            {
                document.DisplayValues();
            }
            addDocumentButton.Visibility = Visibility.Collapsed;
            tbHeader.Text = "These are your documents";
        }

        public override void EditValues()
        {
            foreach (var document in spDocuments.Children.OfType<DocumentControl>())
            {
                document.EditValues();
            }
            addDocumentButton.Visibility = Visibility.Visible;
            tbHeader.Text = "What are your required documents?\nYou can add or remove them at any time.";
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(TaskData.Tasks.DocumentsRequired.ToString());
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
                spDocuments.Children.Add(new DocumentControl(document.Key, document.Value));
            }

            DisplayValues();
        }

        public override async Task SubmitValues()
        {
            foreach (var document in spDocuments.Children.OfType<DocumentControl>())
            {
                if (document.tbDocumentName.Visibility == Visibility.Visible) // Then its in edit mode
                {
                    SaveDocument(document);
                }
            }

            if (DocumentsChanged)
            {
                AppData.SerializeData();
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

        private void SaveDocument(DocumentControl document)
        {
            if (Documents == null)
            {
                Documents = new Dictionary<int, string>();
            }

            if (!Documents.ContainsKey(document.Number) ||
                Documents[document.Number] != document.tbDocumentName.Text)
            {
                Documents[document.Number] = document.tbDocumentName.Text;
                DocumentsChanged = true;
            }
        }

        private void addDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var number = FindFirstFreeNumber();

            var newDocument = new DocumentControl(number, string.Empty);
            newDocument.saveDocumentButton.Click += saveDocumentButton_Click;
            newDocument.removeDocumentButton.Click += removeDocumentButton_Click;
            spDocuments.Children.Add(newDocument);
            DocumentsChanged = true;
        }

        private void saveDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var document = ((sender as Button).Parent as Grid).Parent as DocumentControl;
            SaveDocument(document);

            document.DisplayValues();
        }

        private void removeDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var document = ((sender as Button).Parent as Grid).Parent as DocumentControl;
            if (Documents != null)
            {
                Documents.Remove(document.Number);
            }

            spDocuments.Children.Remove(document);
        }
    }
}
