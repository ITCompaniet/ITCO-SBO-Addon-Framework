using ITCO.SboAddon.Framework.Constants;
using ITCO.SboAddon.Framework.Helpers;
using ITCO.SboAddon.Framework.Services;
using ITCO.SboAddon.Framework.Setup;
using SAPbobsCOM;

namespace ITCO.SboAddon.Framework.Tester
{
    public class TestSetup : ISetup
    {
        public int Version => 3;

        public void Run()
        {
            // Fluid API Style
            UserDefinedHelper.CreateTable("ITCO_MyTbl", "My Table")
                .CreateUDF("ITCO_MyField1", "My Field 1", BoFieldTypes.db_Alpha, 30)
                .CreateUDF("ITCO_MyField2", "My Field 2", BoFieldTypes.db_Numeric, 10);

            UserDefinedHelper.CreateField(
                SboTable.BusinessPartner, "ITCO_YesNo", "Yes or no?", 
                BoFieldTypes.db_Alpha, 1, validValues: UserDefinedHelper.YesNoValiesValues);

            UserDefinedHelper.CreateField(
                SboTable.Invoice, "ITCO_Exported", "Exported Date", BoFieldTypes.db_Date, 10);
            
            // Init Setting
            SettingService.InitSetting("itco.savepath", "Save path", @"c:\temp\");
        }
    }
}
