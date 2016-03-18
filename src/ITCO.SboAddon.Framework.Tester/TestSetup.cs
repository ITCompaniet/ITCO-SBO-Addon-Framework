using System.Collections.Generic;
using ITCO.SboAddon.Framework.Constants;
using ITCO.SboAddon.Framework.Helpers;
using ITCO.SboAddon.Framework.Setup;
using SAPbobsCOM;

namespace ITCO.SboAddon.Framework.Tester
{
    public class TestSetup : ISetup
    {
        public int Version => 3;

        public void Run()
        {
            var validValues = new Dictionary<string, string>
            {
                {"1", "Test 1"},
                {"2", "Test 2"}
            };

            UserDefinedHelper.CreateField(SboTable.BusinessPartner, "ITCO_VV_Test2", "ITCO_VV_Test2", BoFieldTypes.db_Alpha, 1, validValues: validValues);
        }
    }
}
