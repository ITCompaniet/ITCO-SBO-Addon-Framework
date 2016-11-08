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
        public abstract string TableName { get; }
        public string DbTableName => "@" + TableName;
        public abstract string TableDescription { get; }
        public virtual BoUTBTableType TableType => BoUTBTableType.bott_NoObject;
        public virtual UserDefiniedField[] UserDefiniedFields => new UserDefiniedField[0];

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
