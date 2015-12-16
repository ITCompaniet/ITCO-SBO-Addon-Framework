using SAPbobsCOM;
using System;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class UserDefinedHelper
    {
        /// <summary>
        /// Create UDT
        /// </summary>
        /// <param name="tableName">Table name eg: NS_MyTable</param>
        /// <param name="tableDescription"></param>
        /// <returns>Success</returns>
        public static bool CreateTable(string tableName, string tableDescription)
        {
            UserTablesMD userTablesMD = null;

            try
            {
                userTablesMD = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserTables) as UserTablesMD;

                if (!userTablesMD.GetByKey(tableName))
                {
                    userTablesMD.TableName = tableName;
                    userTablesMD.TableDescription = tableDescription;
                    ErrorHelper.HandleErrorWithException(
                        userTablesMD.Add(), 
                        string.Format("Could not create UDT {0}", tableName));
                }
            }
            catch (Exception ex)
            {
                SboApp.Application.MessageBox(ex.Message);
                return false;
            }
            finally
            {
                if (userTablesMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userTablesMD);
            }

            return true;
        }

        /// <summary>
        /// Create UDF on UDT
        /// </summary>
        /// <param name="tableName">UDT Name without @</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldDescription"></param>
        /// <param name="type">BoFieldTypes type</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool CreateFieldOnUDT(string tableName, string fieldName, string fieldDescription, BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50)
        {
            tableName = "@" + tableName;
            return CreateField(tableName, fieldName, fieldDescription, type, size);
        }

        /// <summary>
        /// Create field on table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDescription"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool CreateField(string tableName, string fieldName, string fieldDescription, BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50)
        {
            UserFieldsMD userFieldsMD = null;

            try
            {
                userFieldsMD = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserFields) as UserFieldsMD;

                var fieldId = GetFieldId(tableName, fieldName);
                if (fieldId == -1)
                {
                    userFieldsMD.TableName = tableName;
                    userFieldsMD.Name = fieldName;
                    userFieldsMD.Description = fieldDescription;
                    userFieldsMD.Type = type;
                    userFieldsMD.Size = size;
                    userFieldsMD.EditSize = size;
                    ErrorHelper.HandleErrorWithException(
                        userFieldsMD.Add(), 
                        string.Format("Could not create {0} on {1}", fieldName, tableName));
                }
            }
            catch (Exception ex)
            {
                SboApp.Application.MessageBox(ex.Message);
                return false;
            }
            finally
            {
                if (userFieldsMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userFieldsMD);
            }

            return true;
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

            try
            {
                recordSet.DoQuery(string.Format("SELECT FieldID FROM CUFD WHERE TableID='{0}' AND AliasID='{1}'", tableName, fieldAlias));

                if (recordSet.RecordCount == 1)
                {
                    var fieldId = recordSet.Fields.Item("FieldID").Value as int?;
                    return fieldId.Value;
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSet);
            }
            return -1;
        }
    }
}
