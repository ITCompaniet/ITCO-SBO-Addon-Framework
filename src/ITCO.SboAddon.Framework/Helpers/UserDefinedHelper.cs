using ITCO.SboAddon.Framework.Queries;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using ITCO.SboAddon.Framework.Helpers;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// Helper for creating UD objects
    /// </summary>
    public static class UserDefinedHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, string> YesNoValiesValues => new Dictionary<string, string>
        {
            { "Y", "Yes"},
            { "N", "No" }
        };

        /// <summary>
        /// User defined table object
        /// </summary>
        public class UserDefinedTable
        {
            /// <summary>
            /// UserDefinedTable
            /// </summary>
            /// <param name="tableName"></param>
            public UserDefinedTable(string tableName)
            {
                TableName = tableName;
            }
            /// <summary>
            /// Get TableName
            /// </summary>
            public string TableName { get; set; }

            // Floud API
            /// <summary>
            /// Create UDF
            /// </summary>
            /// <param name="fieldName"></param>
            /// <param name="fieldDescription"></param>
            /// <param name="type"></param>
            /// <param name="size"></param>
            /// <param name="subType"></param>
            /// <param name="validValues"></param>
            /// <param name="defaultValue"></param>
            /// <param name="linkedTable"></param>
            /// <param name="editSize"></param>
            /// <returns></returns>
            public UserDefinedTable CreateUDF(string fieldName, string fieldDescription,
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None,
            IDictionary<string, string> validValues = null, string defaultValue = null,
            string linkedTable = null, int editSize = 0)
            {
                CreateField(TableName, fieldName, fieldDescription, type, size, subType, validValues, defaultValue, linkedTable, editSize);            
                return this;
            }
            // ColumnAliases should be without U_ according to https://answers.sap.com/questions/6567025/udt-key-creation-udf-index-creation.html
            public UserDefinedTable CreateKey(string keyName, string[] columnAliases, bool unique)
            {
                CreateKey(TableName, keyName, columnAliases, unique);
                return this;
            }
        }

        /// <summary>
        /// Create UDT (tableName of UserTables will allways become UPPERCASE in HANA)
        /// </summary>
        /// <param name="tableName">Table name eg: NS_MyTable (in UPPERCASE is recommended)</param>
        /// <param name="tableDescription"></param>
        /// <param name="tableType"></param>
        /// <returns>Success</returns>
        public static UserDefinedTable CreateTable(string tableName, string tableDescription, BoUTBTableType tableType = BoUTBTableType.bott_NoObject)
        {
            UserTablesMD userTablesMd = null;
            
            try
            {
                userTablesMd = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserTables) as UserTablesMD;

                if (userTablesMd == null)
                    throw new NullReferenceException("Failed to get UserTablesMD object");

                if (!userTablesMd.GetByKey(tableName))
                {
                    userTablesMd.TableName = tableName;
                    userTablesMd.TableDescription = tableDescription;
                    userTablesMd.TableType = tableType;
                    ErrorHelper.HandleErrorWithException(
                        userTablesMd.Add(),
                        $"Could not create UDT {tableName}");
                }
            }
            catch (Exception ex)
            {
                SboApp.Logger.Error($"UDT Create Error: {ex.Message}", ex);
                throw;                
            }
            finally
            {
                if (userTablesMd != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userTablesMd);
                userTablesMd = null;
                GC.Collect();
            }

            return new UserDefinedTable("@" + tableName);
        }

        /// <summary>
        /// Create UDF on UDT
        /// </summary>
        /// <param name="tableName">UDT Name without @</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldDescription"></param>
        /// <param name="type">BoFieldTypes type</param>
        /// <param name="size"></param>
        /// <param name="subType"></param>
        /// <param name="validValues"></param>
        /// <param name="defaultValue"></param>
        /// <param name="linkedTable"> use '@' with usertables</param>
        /// <param name="editSize"></param>
        /// <returns></returns>
        public static void CreateFieldOnUDT(string tableName, string fieldName, string fieldDescription, 
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None,
            IDictionary<string, string> validValues = null, string defaultValue = null,
            string linkedTable = null, int editSize = 0)
        {
            tableName = "@" + tableName;
            CreateField(tableName, fieldName, fieldDescription, type, size, subType, validValues, defaultValue, linkedTable, editSize);
        }

        /// <summary>
        /// Create field on table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDescription"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="editSize"></param>
        /// <param name="subType"></param>
        /// <param name="validValues">Dropdown values</param>
        /// <param name="defaultValue"></param>
        /// <param name="linkedTable"> use '@' with usertables</param>
        /// <returns></returns>
        public static void CreateField(string tableName, string fieldName, string fieldDescription, 
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None, 
            IDictionary<string, string> validValues = null, string defaultValue = null,
            string linkedTable = null, int editSize = 0)
        {
            UserFieldsMD userFieldsMd = null;

            try
            {
                userFieldsMd = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserFields) as UserFieldsMD;

                if (userFieldsMd == null)
                    throw new NullReferenceException("Failed to get UserFieldsMD object");

                var fieldId = GetFieldId(tableName, fieldName);
                if (fieldId != -1) return;

                userFieldsMd.TableName = tableName;
                userFieldsMd.Name = fieldName;
                userFieldsMd.Description = fieldDescription;
                userFieldsMd.Type = type;
                userFieldsMd.SubType = subType;
                userFieldsMd.Size = size;
                userFieldsMd.EditSize = editSize != 0 ? editSize : size;
                userFieldsMd.DefaultValue = defaultValue;

                if (validValues != null)
                {
                    foreach (var validValue in validValues)
                    {
                        userFieldsMd.ValidValues.Value = validValue.Key;
                        userFieldsMd.ValidValues.Description = validValue.Value;
                        userFieldsMd.ValidValues.Add();
                    }
                }

                if(linkedTable != null)
                {
                    if (DatabaseHelper.TableExists(linkedTable))
                        userFieldsMd.LinkedTable = linkedTable.Trim('@');
                    else
                        throw new Exception($"Linked table '{linkedTable}' could not be found");
                }

                ErrorHelper.HandleErrorWithException(userFieldsMd.Add(), "Could not create field");
            }
            catch (Exception ex)
            {
                SboApp.Logger.Error($"Create Field {tableName}.{fieldName} Error: {ex.Message}", ex);
                throw;
            }
            finally
            {
                if (userFieldsMd != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userFieldsMd);
                userFieldsMd = null;
                GC.Collect();
            }
        }

        public static void CreateKey(string tableName, string keyName, string[] columnAliases,
            bool unique)
        {
            UserKeysMD userKeysMd = null;
            try
            {
                userKeysMd = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserKeys) as UserKeysMD;
                userKeysMd.TableName = tableName;
                userKeysMd.KeyName = keyName;
                userKeysMd.Unique = unique ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                foreach (var columnAlias in columnAliases)
                {
                    userKeysMd.Elements.ColumnAlias = columnAlias;
                    userKeysMd.Elements.Add();
                }
            }
            catch (Exception e)
            {
                SboApp.Application.SetStatusBarMessage("Couldn't create Key {keyName}", BoMessageTime.bmt_Long, true);
                SboApp.Logger.Error($"Create Key {tableName}.{keyName}. Possible cause: Table must be Empty. Error: {e.Message}", e);
                throw;
            }
            finally
            {
                if (userKeysMd != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userKeysMd);
                userKeysMd = null;
                GC.Collect();
            }
        }

        /// <summary>
        /// Get Field Id
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldAlias"></param>
        /// <returns></returns>
        public static int GetFieldId(string tableName, string fieldAlias)
        {
            var recordSet = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

            if (recordSet == null)
            {
                throw new NullReferenceException("Failed to get Recordset object");
            }

            try
            {
                var sql = FrameworkQueries.Instance.GetFieldIdQuery(tableName, fieldAlias);
                recordSet.DoQuery(sql);
                if (recordSet.RecordCount == 1)
                {
                    var fieldId = recordSet.Fields.Item("FieldID").Value as int?;
                    return fieldId.Value;
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSet);
                recordSet = null;
                GC.Collect();
            }
            return -1;
        }

        /// <summary>
        /// Increase UserField Size
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldAlias"></param>
        /// <param name="size"></param>
        /// <param name="editSize"></param>
        /// <returns></returns>
        public static void IncreaseUserFieldSize(string tableName, string fieldAlias, int size, int editSize = 0)
        {
            UserFieldsMD userFieldsMd = null;
            try
            {
                userFieldsMd = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserFields) as UserFieldsMD;
                if (userFieldsMd == null) return;

                var fieldId = GetFieldId(tableName, fieldAlias);
                if (fieldId == -1) return;
                if (!userFieldsMd.GetByKey(tableName, fieldId)) return;

                bool changed = false;
                if(userFieldsMd.Size < size && userFieldsMd.EditSize < editSize)
                {
                    if (editSize == 0)
                        editSize = size;
                    userFieldsMd.Size = size;
                    userFieldsMd.EditSize = editSize;
                    changed = true;
                }
                else if (userFieldsMd.Size < size)
                {
                    userFieldsMd.Size = size;
                    changed = true;
                }
                else if (userFieldsMd.EditSize < editSize)
                {
                    userFieldsMd.EditSize = editSize;
                    changed = true;
                }
                if(changed)
                    userFieldsMd.Update();
            }
            catch (Exception ex)
            {
                SboApp.Logger.Error($"Increase User Field Size Error: {ex.Message}", ex);
                throw;
            }
            finally
            {
                if (userFieldsMd != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userFieldsMd);
                userFieldsMd = null;
                GC.Collect();
            }
        }

        public static void UpdateFieldSettings(string tableName, string fieldAlias, string fieldDescription, string linkedTable)
        {
            UserFieldsMD userFieldsMd = null;
            try
            {
                userFieldsMd = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserFields) as UserFieldsMD;
                if (userFieldsMd == null) return;

                var fieldId = GetFieldId(tableName, fieldAlias);
                if (fieldId == -1) return;
                if (!userFieldsMd.GetByKey(tableName, fieldId)) return;

                bool changed = false;

                if (linkedTable != null && linkedTable.Trim('@') != userFieldsMd.LinkedTable.Trim('@'))
                {
                    if (DatabaseHelper.TableExists(linkedTable))
                        userFieldsMd.LinkedTable = linkedTable.Trim('@');
                    else
                        throw new Exception($"Linked table '{linkedTable}' could not be found");
                    changed = true;
                }

                if (userFieldsMd.Description != fieldDescription)
                {
                    userFieldsMd.Description = fieldDescription;
                    changed = true;
                }

                if (changed)
                    userFieldsMd.Update();
            }
            catch (Exception ex)
            {
                SboApp.Logger.Error($"Failed updating field: {ex.Message}", ex);
                throw;
            }
            finally
            {
                if (userFieldsMd != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userFieldsMd);
                userFieldsMd = null;
                GC.Collect();
            }
        }
    }
}
