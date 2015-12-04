using SAPbobsCOM;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class UserDefiniedHelper
    {
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
