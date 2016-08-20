using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WedChecker.Infrastructure
{
    public class Currency
    {
        public string CultureString;
        public string CurrencySymbol;
        public string CurrencyLetters;

        public override string ToString()
        {
            return $"{CurrencyLetters} ({CurrencySymbol})";
        }
    }
}
