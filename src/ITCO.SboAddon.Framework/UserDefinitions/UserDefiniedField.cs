namespace ITCO.SboAddon.Framework.UserDefinitions
{
    using System.Collections.Generic;
    using Helpers;
    using SAPbobsCOM;

    public class UserDefiniedField
    {
        public readonly string TableName;
        public readonly string FieldName;
        public readonly string FieldDescription;
        public readonly BoFieldTypes Type;
        public readonly int Size;
        public readonly BoFldSubTypes SubType;
        public readonly IDictionary<string, string> ValidValues;
        public readonly string DefaultValue;

        public UserDefiniedField(string tableName, string fieldName, string fieldDescription,
            BoFieldTypes type = BoFieldTypes.db_Alpha, int size = 50, BoFldSubTypes subType = BoFldSubTypes.st_None,
            IDictionary<string, string> validValues = null, string defaultValue = null)
        {
            TableName = tableName;
            FieldName = fieldName;
            FieldDescription = fieldDescription;
            Type = type;
            Size = size;
            SubType = subType;
            ValidValues = validValues;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Create field
        /// </summary>
        public void Create() => UserDefinedHelper.CreateField(TableName, FieldName, FieldDescription, Type, Size, SubType, ValidValues,DefaultValue);
        
        /// <summary>
        /// Get field name with U_ prefix
        /// </summary>
        public string DbField => $"U_{FieldName}";
    }
}
