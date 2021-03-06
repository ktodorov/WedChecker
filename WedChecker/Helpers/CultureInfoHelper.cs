﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using WedChecker.Infrastructure;

namespace WedChecker.Helpers
{
    public static class CultureInfoHelper
    {
        private static List<string> allCultures = new List<string> {
        "ar-SA",
"bg-BG",
"ca-ES",
"zh-TW",
"cs-CZ",
"da-DK",
"de-DE",
"el-GR",
"en-US",
"fi-FI",
"fr-FR",
"he-IL",
"hu-HU",
"is-IS",
"it-IT",
"ja-JP",
"ko-KR",
"nl-NL",
"nb-NO",
"pl-PL",
"pt-BR",
"ro-RO",
"ru-RU",
"hr-HR",
"sk-SK",
"sq-AL",
"sv-SE",
"th-TH",
"tr-TR",
"ur-PK",
"id-ID",
"uk-UA",
"be-BY",
"sl-SI",
"et-EE",
"lv-LV",
"lt-LT",
"fa-IR",
"vi-VN",
"hy-AM",
"az-Latn-AZ",
"eu-ES",
"mk-MK",
"af-ZA",
"ka-GE",
"fo-FO",
"hi-IN",
"ms-MY",
"kk-KZ",
"ky-KG",
"sw-KE",
"uz-Latn-UZ",
"tt-RU",
"pa-IN",
"gu-IN",
"ta-IN",
"te-IN",
"kn-IN",
"mr-IN",
"sa-IN",
"mn-MN",
"gl-ES",
"kok-IN",
"syr-SY",
"dv-MV",
"ar-IQ",
"zh-CN",
"de-CH",
"en-GB",
"es-MX",
"fr-BE",
"it-CH",
"nl-BE",
"nn-NO",
"pt-PT",
"sr-Latn-CS",
"sv-FI",
"az-Cyrl-AZ",
"ms-BN",
"uz-Cyrl-UZ",
"ar-EG",
"zh-HK",
"de-AT",
"en-AU",
"es-ES",
"fr-CA",
"sr-Cyrl-CS",
"ar-LY",
"zh-SG",
"de-LU",
"en-CA",
"es-GT",
"fr-CH",
"ar-DZ",
"zh-MO",
"de-LI",
"en-NZ",
"es-CR",
"fr-LU",
"ar-MA",
"en-IE",
"es-PA",
"fr-MC",
"ar-TN",
"en-ZA",
"es-DO",
"ar-OM",
"en-JM",
"es-VE",
"ar-YE",
"en-029",
"es-CO",
"ar-SY",
"en-BZ",
"es-PE",
"ar-JO",
"en-TT",
"es-AR",
"ar-LB",
"en-ZW",
"es-EC",
"ar-KW",
"en-PH",
"es-CL",
"ar-AE",
"es-UY",
"ar-BH",
"es-PY",
"ar-QA",
"es-BO",
"es-SV",
"es-HN",
"es-NI",
"es-PR",
"sma-NO",
"sr-Cyrl-BA",
"zu-ZA",
"xh-ZA",
"fy-NL",
"tn-ZA",
"se-SE",
"sma-SE",
"fil-PH",
"smn-FI",
"quz-PE",
"se-FI",
"sms-FI",
"cy-GB",
"hr-BA",
"iu-Latn-CA",
"bs-Cyrl-BA",
"moh-CA",
"smj-NO",
"arn-CL",
"mi-NZ",
"quz-EC",
"ga-IE",
"rm-CH",
"sr-Latn-BA",
"smj-SE",
"lb-LU",
"ns-ZA",
"quz-BO",
"se-NO",
"mt-MT",
"bs-Latn-BA"
        };


        private static List<Currency> allCurrencies = GetAllCurrencies();

        public static List<Currency> GetAllCurrencies()
        {
            if (allCurrencies != null)
            {
                return allCurrencies;
            }

            var currencies = new List<Currency>();

            foreach (var culture in allCultures)
            {
                try
                {
                    var region = new RegionInfo(culture);
                    if (region == null)
                    {
                        continue;
                    }

                    var currency = new Currency()
                    {
                        CultureString = culture,
                        CurrencyLetters = region.ISOCurrencySymbol,
                        CurrencySymbol = region.CurrencySymbol
                    };

                    if (currencies.Any(c => c.CurrencyLetters == currency.CurrencyLetters || c.CurrencySymbol == currency.CurrencySymbol))
                    {
                        continue;
                    }

                    currencies.Add(currency);
                }
                catch
                { }
            }

            currencies = currencies.OrderBy(c => c.CurrencyLetters).ToList();

            allCurrencies = currencies;

            return currencies;
        }

        public static Currency GetCurrencyForCurrentCulture()
        {
            var currentCulture = CultureInfo.CurrentCulture.Name;
            var regionInfo = new RegionInfo(currentCulture);

            var currency = allCurrencies.FirstOrDefault(c => c.CurrencyLetters == regionInfo.ISOCurrencySymbol);
            return currency;
        }

        public static Currency GetCurrencyForCulture(string culture)
        {
            try
            {
                var regionInfo = new RegionInfo(culture);
                var currency = allCurrencies.FirstOrDefault(c => c.CurrencyLetters == regionInfo.ISOCurrencySymbol);
                return currency;
            }
            catch
            {
                return GetCurrencyForCurrentCulture();
            }
        }

        public static Currency GetStoredCurrency()
        {
            Currency selectedCurrency = null;

            var currencyCulture = AppData.GetRoamingSetting<string>("CurrencyCulture");
            if (!string.IsNullOrEmpty(currencyCulture))
            {
                selectedCurrency = CultureInfoHelper.GetCurrencyForCulture(currencyCulture);
            }
            else
            {
                selectedCurrency = CultureInfoHelper.GetCurrencyForCurrentCulture();
            }

            return selectedCurrency;
        }

        public static string GetCurrentCurrencyString()
        {
            var currentCurrency = CultureInfoHelper.GetStoredCurrency();
            if (currentCurrency == null)
            {
                return string.Empty;
            }

            return currentCurrency.CurrencySymbol;
        }
    }
}
