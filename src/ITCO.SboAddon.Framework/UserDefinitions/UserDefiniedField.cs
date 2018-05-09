namespace ITCO.SboAddon.Framework.UserDefinitions
{
    using System.Collections.Generic;
    using Helpers;
    using SAPbobsCOM;

    /// <summary>
    /// UserDefinedField
    /// </summary>
    public class UserDefiniedField
    {
        /// <summary>
        /// Get TableName
        /// </summary>
        public readonly string TableName;
        /// <summary>
        /// Get FieldName
        /// </summary>
        public readonly string FieldName;
        /// <summary>
        /// Get Field Description
        /// </summary>
        public readonly string FieldDescription;
        /// <summary>
        /// Get Type
        /// </summary>
        public readonly BoFieldTypes Type;
        /// <summary>
        /// Get Size
        /// </summary>
        public readonly int Size;
        /// <summary>
        /// Get SubType
        /// </summary>
        public readonly BoFldSubTypes SubType;
        /// <summary>
        /// Get ValidValues
        /// </summary>
        public readonly IDictionary<string, string> ValidValues;
        /// <summary>
        /// Get DefaultValue
        /// </summary>
        public readonly string DefaultValue;
        /// <summary>
        /// Get LinkedTable
        /// </summary>
        public readonly string LinkedTable;
        /// <summary>
        /// Get EditSize
        /// </summary>
        public readonly int EditSize;

        /// <summary>
        /// User Defined Field
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDescription"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="subType"></param>
        /// <param name="validValues"></param>
        /// <param name="defaultValue"></param>
        /// <param name="linkedTable"></param>
        /// <param name="editSize"></param>
        public UserDefiniedField(string tableName, string fieldName, string fieldDescription,
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None,
            IDictionary<string, string> validValues = null, string defaultValue = null, string linkedTable = null, int editSize = 0)
        {
            TableName = tableName;
            FieldName = fieldName;
            FieldDescription = fieldDescription;
            Type = type;
            Size = size;
            SubType = subType;
            ValidValues = validValues;
            DefaultValue = defaultValue;
            LinkedTable = linkedTable;
            EditSize = editSize;
        }

        /// <summary>
        /// Create field
        /// </summary>
        public void Create() => UserDefinedHelper.CreateField(TableName, FieldName, FieldDescription, Type, Size, SubType, ValidValues, DefaultValue, LinkedTable, EditSize);
        
        /// <summary>
        /// Get field name with U_ prefix
        /// </summary>
        public string DbField => $"U_{FieldName}";
    }
}
