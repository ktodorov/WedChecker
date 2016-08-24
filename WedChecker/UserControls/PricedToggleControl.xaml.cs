using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Extensions;
using WedChecker.Helpers;
using WedChecker.Interfaces;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls
{
    public sealed partial class PricedToggleControl : UserControl, IStorableTask, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        private double? _purchaseValue;
        public double? PurchaseValue
        {
            get
            {
                if (_purchaseValue == null)
                {
                    _purchaseValue = 0;
                }
                return _purchaseValue;
            }
            set
            {
                if (_purchaseValue == value)
                {
                    return;
                }
                _purchaseValue = value;
                RaiseProperty("PurchaseValue");
            }
        }

        public double? StoredPurchaseValue;

        public string Title
        {
            get
            {
                return mainToggle.Title;
            }
            set
            {
                mainToggle.Title = value;
            }
        }

        public bool Toggled
        {
            get
            {
                return mainToggle.Toggled;
            }
            set
            {
                mainToggle.Toggled = value;
            }
        }

        public PricedToggleControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            Loaded += PurchaseToggleControl_Loaded;
        }

        private void PurchaseToggleControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbCurrency.Text = CultureInfoHelper.GetCurrentCurrencyString();

            mainToggle.Checked += MainToggle_Checked;
            tbPurchaseValue.IsEnabled = mainToggle.Toggled;
        }

        private void MainToggle_Checked(object sender, EventArgs e)
        {
            tbPurchaseValue.IsEnabled = mainToggle.Toggled;
        }

        public void DisplayValues()
        {
            tbPurchaseValue.Visibility = Visibility.Collapsed;
            if (mainToggle.Toggled)
            {
                pricePanel.Visibility = Visibility.Visible;
                tbPurchaseValueDisplay.Visibility = Visibility.Visible;
                tbCurrency.Visibility = Visibility.Visible;
            }
            else
            {
                pricePanel.Visibility = Visibility.Collapsed;
                tbPurchaseValueDisplay.Visibility = Visibility.Collapsed;
                tbCurrency.Visibility = Visibility.Collapsed;
            }
            mainToggle.DisplayValues();
        }

        public void EditValues()
        {
            pricePanel.Visibility = Visibility.Visible;
            tbPurchaseValueDisplay.Visibility = Visibility.Collapsed;
            tbPurchaseValue.Visibility = Visibility.Visible;
            tbPurchaseValue.Text = tbPurchaseValue.Text.Replace(" ", string.Empty);
            tbCurrency.Visibility = Visibility.Collapsed;
            mainToggle.EditValues();
        }

        public void Serialize(BinaryWriter writer)
        {
            mainToggle.Serialize(writer);

            var purchaseValueText = tbPurchaseValue.Text.Replace(",", ".");
            if (!purchaseValueText.IsValidPrice())
            {
                Core.ShowErrorMessage("Please enter a valid number for the purchase value (number, higher than zero and less than 2 million)");
                PurchaseValue = StoredPurchaseValue;
            }

            writer.Write(1);
            writer.Write("PurchaseValue");
            writer.Write(PurchaseValue.Value);
        }

        public async Task Deserialize(BinaryReader reader)
        {
            await mainToggle.Deserialize(reader);
            if (reader.PeekChar() != -1)
            {
                var objectsCount = reader.ReadInt32();

                for (var i = 0; i < objectsCount; i++)
                {
                    reader.ReadString();
                    PurchaseValue = reader.ReadDouble();
                    StoredPurchaseValue = PurchaseValue;
                }
            }
        }

        public string GetDataAsText()
        {
            var sb = new StringBuilder();

            var toggleText = mainToggle.GetDataAsText();
            var currency = CultureInfoHelper.GetCurrentCurrencyString();
            var priceText = $"- for {PurchaseValue.RoundToString()}{currency}";

            sb.AppendLine($"{toggleText} {priceText}");

            return sb.ToString();
        }
    }
}
