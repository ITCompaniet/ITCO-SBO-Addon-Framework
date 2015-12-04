using SAPbouiCOM;
using System;
using System.Globalization;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class DataExtensions
    {
        /// <summary>
        /// Parse date from 
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        /// <example>
        /// var date = form.DataSources.UserDataSources.Item("Date").ToDate();
        /// </example>
        public static DateTime? ToDate(this UserDataSource dataSource)
        {
            if (string.IsNullOrEmpty(dataSource.ValueEx))
                return null;

            return DateTime.ParseExact(dataSource.ValueEx, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}
