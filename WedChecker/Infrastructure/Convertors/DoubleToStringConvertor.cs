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
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var tempValue = 0.0;
            if (double.TryParse(value.ToString(), out tempValue))
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
