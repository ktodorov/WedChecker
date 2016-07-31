using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml.Data;

namespace WedChecker.Infrastructure.Convertors
{
    public class EmailAddressConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            var emailAddress = value.ToString();
            var hyperlinkText = $"mailto:{emailAddress}";
            return hyperlinkText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
