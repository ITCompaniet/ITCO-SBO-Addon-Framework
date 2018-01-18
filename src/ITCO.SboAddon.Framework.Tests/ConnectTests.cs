namespace ITCO.SboAddon.Framework.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConnectTests
    {
        [TestMethod]
        public void DiConnectFromAppConfig_Test()
        {
            SboApp.DiConnectFromAppConfig();
        }
    }
}
