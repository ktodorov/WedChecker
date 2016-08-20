using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WedChecker.Infrastructure.Convertors
{
    public class DoubleToStringConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var convertedValue = string.Format("{0:# ### ###.00}", value);
            return convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var tempValue = 0.0;
            var convertedValue = value.ToString().Replace(",", ".");
            convertedValue = string.Format("{0:0.00}", convertedValue);
            if (double.TryParse(convertedValue, out tempValue))
            {
                return tempValue;
            }
            else
            {
                return 0;
            }
        }
    }
}
