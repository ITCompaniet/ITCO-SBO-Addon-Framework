using ITCO.SboAddon.Framework.Helpers;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ITCO.SboAddon.Framework.Services
{
    public static class SettingService
    {
        private const string UDT_Settings = "ITCO_FW_Settings";
        private const string UDF_Setting_Value = "ITCO_FW_SValue";
        private const string UDF_Setting_Usercode = "ITCO_FW_User";
        private static bool setupOk = false;

        public static bool Init()
        {
            if (setupOk)
                return true;

            if (!UserDefinedHelper.CreateTable(UDT_Settings, "Settings"))
                setupOk = false;

            if (!UserDefinedHelper.CreateFieldOnUDT(UDT_Settings, UDF_Setting_Value, "Value"))
                setupOk = false;

            if (!UserDefinedHelper.CreateFieldOnUDT(UDT_Settings, UDF_Setting_Usercode, "Usercode"))
                setupOk = false;

            if (setupOk)
                SboApp.Application.StatusBar.SetText("SettingService Init [OK]", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

            return setupOk;
        }

        public static T GetCurrentUserSettingByKey<T>(string key, T defaultValue = default(T), bool askIfNotFound = false)
        {
            var userCode = SboApp.Company.UserName;
            return GetSettingByKey<T>(key, defaultValue , userCode, askIfNotFound);
        }
        /// <summary>
        /// Get Setting
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="key">Setting Key</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Setting Value</returns>
        public static T GetSettingByKey<T>(string key, T defaultValue = default(T), string userCode = null, bool askIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;
            try
            {
                key = key.Trim().ToLowerInvariant();

                var sql = string.Format("SELECT [U_{0}] FROM [@{1}] WHERE [Code] = '{2}'", UDF_Setting_Value, UDT_Settings, key);

                if (userCode != null)
                    sql += string.Format(" AND [U_{0}] = '{1}'", UDF_Setting_Usercode, userCode);

                using (var query = new SboRecordsetQuery(sql))
                {
                    foreach (var setting in query.Result)
                    {
                        var value = setting.Item(0).Value as string;
                        return To<T>(value);
                    }
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                SboApp.Application.StatusBar.SetText(string.Format("SettingService Error: {0}", e.Message), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return defaultValue;
            }
        }

        private static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        private static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                var destinationConverter = GetTypeConverter(destinationType);
                var sourceConverter = GetTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        private static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        private static TypeConverter GetTypeConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type);
        }
    }
}
