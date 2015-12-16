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
        private const string UDF_Setting_Usercode = "ITCO_FW_User";
        private static bool setupOk = false;

        public static bool Init()
        {
            if (setupOk)
                return true;

            setupOk = true;

            if (!UserDefinedHelper.CreateTable(UDT_Settings, "Settings"))
                setupOk = false;

            if (!UserDefinedHelper.CreateFieldOnUDT(UDT_Settings, UDF_Setting_Value, "Value"))
                setupOk = false;

            //if (!UserDefinedHelper.CreateFieldOnUDT(UDT_Settings, UDF_Setting_Usercode, "Usercode"))
            //    setupOk = false;

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
            Init();

            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var returnValue = defaultValue;
            var name = key;

            try
            {
                var sqlKey = key.Trim().ToLowerInvariant();

                if (userCode != null)
                    sqlKey = string.Format("{0}[{1}]", sqlKey, userCode);

                var sql = string.Format("SELECT [U_{0}], [Name] FROM [@{1}] WHERE [Code] = '{2}'", UDF_Setting_Value, UDT_Settings, sqlKey);


                using (var query = new SboRecordsetQuery(sql))
                {
                    foreach (var setting in query.Result)
                    {
                        var value = setting.Item(0).Value as string;
                        name = setting.Item(1).Value as string;
                        
                        returnValue = To<T>(value);
                    }
                }
            }
            catch (Exception e)
            {
                SboApp.Application.StatusBar.SetText(string.Format("SettingService Error: {0}", e.Message), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return returnValue;
            }

            if (returnValue == null && askIfNotFound)
            {
                var inputTitle = string.Format("Insert setting {0}", key);
                if (userCode != null)
                    inputTitle += string.Format(" for {0}", userCode);

                var result = InputHelper.GetInputs(inputTitle, new List<IDialogInput>()
                            {
                                new TextDialogInput("setting", name, required: true)
                            });
                var newSetting = result.First().Value as string;
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
        public static void SaveSetting<T>(string key, T value, string userCode = null, string name = null)
        {
            Init();

            var sqlKey = key.Trim().ToLowerInvariant();

            if (userCode != null)
                sqlKey = string.Format("{0}[{1}]", key, userCode);

            var sql = string.Format("SELECT [U_{0}], [Name] FROM [@{1}] WHERE [Code] = '{2}'", UDF_Setting_Value, UDT_Settings, sqlKey);
            
            var exists = false;
            using (var query = new SboRecordsetQuery(sql))
            {
                exists = query.Count == 1;
            }

            if (exists)
            {
                sql = string.Format("UPDATE [@{0}] SET [U_{1}] = '{2}' WHERE [Code] = '{3}'", UDT_Settings, UDF_Setting_Value, value, sqlKey);
            }
            else
            {
                if (name == null)
                    name = sqlKey;

                sql = string.Format("INSERT INTO [@{0}] ([Code], [Name], [U_{1}]) VALUES ('{2}', '{3}', '{4}')", 
                    UDT_Settings, UDF_Setting_Value, sqlKey, name, value);
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
