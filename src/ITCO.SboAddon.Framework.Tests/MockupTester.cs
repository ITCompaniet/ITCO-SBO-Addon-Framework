namespace ITCO.SboAddon.Framework.Tests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Services;
    using Services.Mockups;

    [TestClass]
    public class MockupTester
    {
        [TestMethod]
        public void MockupSettingService_Tester()
        {
            var settings = new Dictionary<string, object>
            {
                {"key1", "value-1"},
                {"key1[manager]", "value-manager-1"}
            };

            var settingService = new MockupSettingService(settings, "manager");
            settingService.SaveSetting("key2", "value-2");
            settingService.SaveCurrentUserSetting("key2", "value-manager-2");

            Assert.AreEqual("value-1", settingService.GetSettingByKey<string>("key1"));
            Assert.AreEqual("value-manager-1", settingService.GetCurrentUserSettingByKey<string>("key1"));

            Assert.AreEqual("value-2", settingService.GetSettingByKey<string>("key2"));
            Assert.AreEqual("value-manager-2", settingService.GetCurrentUserSettingByKey<string>("key2"));
        }
    }
}
