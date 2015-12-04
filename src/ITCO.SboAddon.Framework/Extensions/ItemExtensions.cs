using SAPbouiCOM;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class ItemExtensions
    {
        /// <summary>
        /// Parse EditText value into decimal value and currency code
        /// </summary>
        /// <param name="editText"></param>
        /// <returns></returns>
        public static MoneyField GetMoney(this EditText editText)
        {
            var moneyString = editText.Value.Substring(0, editText.Value.Length - 4);
            var money = decimal.Parse(moneyString, NumberStyles.Any);
            var currencyCode = editText.Value.Substring(editText.Value.Length - 3);

            return new MoneyField
            {
                Money = money,
                CurrencyCode = currencyCode
            };
        }

        /// <summary>
        /// Parse weight field eg. 100kg into mg
        /// </summary>
        /// <param name="editText"></param>
        /// <returns>milligrams</returns>
        public static int GetWeightInMg(this EditText editText)
        {
            var weightInUnit = decimal.Parse(Regex.Match(editText.Value.Replace(".", string.Empty), @"[\d.]+").Value, NumberStyles.Any);
            var unit = Regex.Match(editText.Value, @"[A-Za-z]+").Value;

            switch (unit)
            {
                case "g":
                    return (int)weightInUnit * 1000;
                case "kg":
                    return (int)weightInUnit * 1000000;
            }

            return 0;
        }
    }

    public class MoneyField
    {
        public decimal Money { get; set; }
        public string CurrencyCode { get; set; }
    }
}
