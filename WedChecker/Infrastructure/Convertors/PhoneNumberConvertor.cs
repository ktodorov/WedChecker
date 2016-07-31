using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml.Data;

namespace WedChecker.Infrastructure.Convertors
{
    public class PhoneNumberConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            var phoneNumber = value.ToString();
            var hyperlinkText = $"tel:{phoneNumber}";
            return hyperlinkText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
