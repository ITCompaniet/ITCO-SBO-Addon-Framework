using ITCO.SboAddon.Framework.Constants;
using ITCO.SboAddon.Framework.Helpers;
using ITCO.SboAddon.Framework.Services;
using ITCO.SboAddon.Framework.Setup;
using SAPbobsCOM;

namespace ITCO.SboAddon.Framework.Tester
{
    using UserDefinitions;

    public class TestSetup : ISetup
    {
        public int Version => 5;
        
        public void Run()
        {
            // Fluid API Style
            UserDefinedHelper.CreateTable("ITCO_MyTbl", "My Table")
                .CreateUDF("ITCO_MyField1", "My Field 1", BoFieldTypes.db_Alpha, 30)
                .CreateUDF("ITCO_MyField2", "My Field 2", BoFieldTypes.db_Numeric, 10)
                .CreateUDF("ITCO_MyField2", "My Field 2", BoFieldTypes.db_Numeric, 10);

            UserDefinedHelper.CreateField(
                SboTable.BusinessPartner, "ITCO_YesNo", "Yes or no?", 
                BoFieldTypes.db_Alpha, 1, validValues: UserDefinedHelper.YesNoValiesValues);

            

            // Init Setting
            SettingService.Instance.InitSetting("itco.savepath", "Save path", @"c:\temp\");
        }
    }
    /*
    public class MyTable1 : UserDefinedTable
    {
        public override string TableName => "ITCO_MyTbl1";
        public override string TableDescription => "My Table 1";

        public override UserDefiniedField[] UserDefiniedFields => new[] { Exported, MyCustomField1 };

        public static UserDefiniedField Exported = new UserDefiniedField(DbTableName, "ITCO_Exported", "Exported Date", BoFieldTypes.db_Date, 10);
        public static UserDefiniedField MyCustomField1 = new UserDefiniedField(DbTableName, "ITCO_C1", "Custom Field 1");
    }
    */
}
