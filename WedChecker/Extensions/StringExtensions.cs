using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WedChecker.Extensions
{
    public static class StringExtensions
    {
        public static string SeparateCamelCase(this string text)
        {
            var array = text.ToCharArray().ToList();

            var resultArray = new List<char>();

            var i = 0;
            foreach (var symbol in array)
            {
                if (char.IsUpper(symbol) && i != 0)
                {
                    resultArray.Add(' ');
                    resultArray.Add(char.ToLower(symbol));
                }
                else
                {
                    resultArray.Add(symbol);
                }
                i++;
            }

            var result = string.Join("", resultArray.ToArray());

            return result;
        }

        public static int ToInteger(this string text)
        {
            var result = 0;
            if (int.TryParse(text, out result))
            {
                return result;
            }

            return 0;
        }

        public static char? FirstLetter(this string text)
        {
            return string.IsNullOrEmpty(text) ? (char?)null : text[0];
        }
    }
}
