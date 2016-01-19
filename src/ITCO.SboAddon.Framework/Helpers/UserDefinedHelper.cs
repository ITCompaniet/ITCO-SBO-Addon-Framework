using SAPbobsCOM;
using System;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class UserDefinedHelper
    {
        public class UserDefinedTable
        {
            public UserDefinedTable(string tableName)
            {
                TableName = tableName;
            }
            public string TableName { get; set; }

            // Floud API
            public UserDefinedTable CreateUDF(string fieldName, string fieldDescription,
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None)
            {
                CreateField(TableName, fieldName, fieldDescription, type, size, subType);            
                return this;
            }
        }

        /// <summary>
        /// Create UDT
        /// </summary>
        /// <param name="tableName">Table name eg: NS_MyTable</param>
        /// <param name="tableDescription"></param>
        /// <param name="tableType"></param>
        /// <returns>Success</returns>
        public static UserDefinedTable CreateTable(string tableName, string tableDescription, BoUTBTableType tableType = BoUTBTableType.bott_NoObject)
        {
            UserTablesMD userTablesMD = null;

            try
            {
                userTablesMD = SboApp.Company.GetBusinessObject(BoObjectTypes.oUserTables) as UserTablesMD;

                if (!userTablesMD.GetByKey(tableName))
                {
                    userTablesMD.TableName = tableName;
                    userTablesMD.TableDescription = tableDescription;
                    userTablesMD.TableType = tableType;

                    ErrorHelper.HandleErrorWithException(
                        userTablesMD.Add(), 
                        string.Format("Could not create UDT {0}", tableName));
                }
            }
            catch (Exception ex)
            {
                SboApp.Application.MessageBox(ex.Message);
                throw ex;                
            }
            finally
            {
                if (userTablesMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userTablesMD);
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
        /// <returns></returns>
        public static void CreateFieldOnUDT(string tableName, string fieldName, string fieldDescription, 
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None)
        {
            tableName = "@" + tableName;
            CreateField(tableName, fieldName, fieldDescription, type, size, subType);
        }

        /// <summary>
        /// Create field on table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDescription"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="subType"></param>
        /// <returns></returns>
        public static void CreateField(string tableName, string fieldName, string fieldDescription, 
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None)
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
                    userFieldsMD.SubType = subType;
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
                throw ex;
            }
            finally
            {
                if (userFieldsMD != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userFieldsMD);
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
