using ITCO.SboAddon.Framework.Dialogs;
using ITCO.SboAddon.Framework.Dialogs.Inputs;
using ITCO.SboAddon.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ITCO.SboAddon.Framework.Services
{
    public static class SettingService
    {
        private const string UDT_Settings = "ITCO_FW_Settings";
        private const string UDF_Setting_Value = "ITCO_FW_SValue";
        private static bool setupOk = false;

        public static bool Init()
        {
            if (setupOk)
                return true;
            
            try
            {
                UserDefinedHelper.CreateTable(UDT_Settings, "Settings")
                    .CreateUDF(UDF_Setting_Value, "Value");

                setupOk = true;
                SboApp.Application.StatusBar.SetText("SettingService Init [OK]", 
                    SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception e)
            {
                SboApp.Application.StatusBar.SetText($"SettingService Init [NOT OK] {e.Message}", 
                    SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                
                setupOk = false;
            }
                
            return setupOk;
        }
        /// <summary>
        /// Create Empty Setting if not exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="name"></param>
        public static void InitSetting<T>(string key, string name)
        {
            if (GetSettingByKey<T>(key) == null)
                SaveSetting<T>(key, name: name);
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
            Init();

            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var returnValue = defaultValue;
            var name = key;
            var notFound = true;

            try
            {
                var sqlKey = key.Trim().ToLowerInvariant();

                if (userCode != null)
                    sqlKey = $"{sqlKey}[{userCode}]";

                var sql = $"SELECT [U_{UDF_Setting_Value}], [Name] FROM [@{UDT_Settings}] WHERE [Code] = '{sqlKey}'";


                using (var query = new SboRecordsetQuery(sql))
                {
                    foreach (var setting in query.Result)
                    {
                        var value = setting.Item(0).Value as string;
                        if (value == "")
                            value = null;
                        else
                            notFound = false;

                        name = setting.Item(1).Value as string;
                        
                        returnValue = To<T>(value);
                    }
                }
            }
            catch (Exception e)
            {
                SboApp.Application.StatusBar.SetText($"SettingService Error: {e.Message}", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return returnValue;
            }

            if (notFound && askIfNotFound)
            {
                var inputTitle = $"Insert setting {name}";
                if (userCode != null)
                    inputTitle += $" for {userCode}";

                IDialogInput input = new TextDialogInput("setting", name, required: true);

                if (typeof(T) == typeof(bool))
                    input = new CheckboxDialogInput("setting", name);

                if (typeof(T) == typeof(DateTime))
                    input = new DateDialogInput("setting", name, required: true);

                if (typeof(T) == typeof(int))
                    input = new IntegerDialogInput("setting", name, required: true);

                if (typeof(T) == typeof(decimal))
                    input = new DecimalDialogInput("setting", name, required: true);

                var result = InputHelper.GetInputs(inputTitle, new List<IDialogInput>()
                            {
                                input
                            });
                var newSetting = result.First().Value;
                SaveSetting(key, newSetting, userCode);

                returnValue = To<T>(newSetting);
            }

            return returnValue;
        }

        /// <summary>
        /// Save Setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="userCode"></param>
        /// <param name="name"></param>
        public static void SaveSetting<T>(string key, T value = default(T), string userCode = null, string name = null)
        {
            Init();

            var sqlKey = key.Trim().ToLowerInvariant();

            if (userCode != null)
                sqlKey = $"{key}[{userCode}]";

            var sql = $"SELECT [U_{UDF_Setting_Value}], [Name] FROM [@{UDT_Settings}] WHERE [Code] = '{sqlKey}'";
            
            var exists = false;
            using (var query = new SboRecordsetQuery(sql))
            {
                exists = query.Count == 1;
            }

            var sqlValue = string.Format(CultureInfo.InvariantCulture, "'{0}'", value);
            if (value == null)
                sqlValue = "NULL";

            if (exists)
            {
                sql = $"UPDATE [@{UDT_Settings}] SET [U_{UDF_Setting_Value}] = {sqlValue} WHERE [Code] = '{sqlKey}'";
            }
            else
            {
                if (name == null)
                    name = sqlKey;

                sql =
                    $"INSERT INTO [@{UDT_Settings}] ([Code], [Name], [U_{UDF_Setting_Value}]) VALUES ('{sqlKey}', '{name}', {sqlValue})";
            }

            SboRecordset.NonQuery(sql);
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
