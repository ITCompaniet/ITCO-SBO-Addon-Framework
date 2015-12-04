using ITCO.SboAddon.Framework.Helpers;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ITCO.SboAddon.Framework.Services
{
    public static class SettingService
    {
        public const string UDT_Settings = "ITCO_FW_Settings";
        public const string UDF_Setting_Value = "ITCO_FW_SValue";

        public static T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            using (var query = new RecordsetQuery(string.Format("SELECT [U_{0}] FROM [@{1}] WHERE [Code] = '{3}'", UDF_Setting_Value, UDT_Settings, key)))
            {
                if (query.Count == 0)
                    return defaultValue;

                var setting = query.Result.First();
                var value = setting.Item("U_ " + UDF_Setting_Value).Value as string;

                return To<T>(value);
            }
        }

        public static object To(object value, Type destinationType)
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
        public static object To(object value, Type destinationType, CultureInfo culture)
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
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type);
        }
    }
}
