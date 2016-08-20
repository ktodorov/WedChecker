using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Helpers;

namespace WedChecker.Extensions
{
    public static class DoubleExtensions
    {
        public static string RoundToString(this double number)
        {
            var convertedValue = string.Format("{0:# ### ###.00}", number);
            return convertedValue;
        }

        public static string RoundToString(this double? number)
        {
            if (!number.HasValue)
            {
                return string.Empty;
            }

            return number.Value.RoundToString();
        }

        public static string ToCurrency(this double? number)
        {
            if (!number.HasValue)
            {
                return string.Empty;
            }

            return number.Value.ToCurrency();
        }

        public static string ToCurrency(this double number)
        {
            var currency = number.RoundToString();
            var currencyType = CultureInfoHelper.GetCurrentCurrencyString();

            var currencyString = $"{currency}{currencyType}";
            return currencyString;
        }

        public static bool IsValidPrice(this string numberText)
        {
            double tempValue = 0;
            if (!double.TryParse(numberText, out tempValue) || tempValue > 2000000 || tempValue < 0)
            {
                return false;
            }

            return true;
        }
    }
}
