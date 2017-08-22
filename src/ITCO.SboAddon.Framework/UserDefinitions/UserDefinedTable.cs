namespace ITCO.SboAddon.Framework.UserDefinitions
{
    using System.Linq;
    using Helpers;
    using SAPbobsCOM;

    /// <summary>
    /// User defined table object
    /// </summary>
    public abstract class UserDefinedTable
    {
        /// <summary>
        /// Get TableName
        /// </summary>
        public abstract string TableName { get; }
        /// <summary>
        /// Get Database TableName
        /// </summary>
        public string DbTableName => "@" + TableName;
        /// <summary>
        /// Get TableDescription
        /// </summary>
        public abstract string TableDescription { get; }
        /// <summary>
        /// Get TableType
        /// </summary>
        public virtual BoUTBTableType TableType => BoUTBTableType.bott_NoObject;
        /// <summary>
        /// Get UserDefinded Fields
        /// </summary>
        public virtual UserDefiniedField[] UserDefiniedFields => new UserDefiniedField[0];

        /// <summary>
        /// Create Table
        /// </summary>
        public void Create()
        {
            UserDefinedHelper.CreateTable(TableName, TableDescription, TableType);
            UserDefiniedFields.ToList().ForEach(udf =>
            {
                udf.Create();
            });
        }
    }
}
