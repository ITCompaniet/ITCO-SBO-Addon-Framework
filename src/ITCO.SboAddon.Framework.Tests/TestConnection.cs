using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ITCO.SboAddon.Framework.Tests
{
    [TestClass]
    public class TestConnection
    {
        [TestMethod]
        public void DiConnect()
        {
            try
            {
                SboApp.DiConnect("KENNY-SUPERDELL", SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014, "SBODemoUS", "dbo", "dbopass", "manager", "password");
            }
            catch (Exception e)
            {
                throw new AssertFailedException(e.Message);
            }
        }
    }
}
