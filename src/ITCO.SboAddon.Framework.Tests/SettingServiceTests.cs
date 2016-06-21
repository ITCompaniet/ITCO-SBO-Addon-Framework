using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCO.SboAddon.Framework;
using ITCO.SboAddon.Framework.Helpers;
using ITCO.SboAddon.Framework.Services;

namespace ITCO.SboAddon.Framework.Tests
{
    [TestClass]
    public class SettingServiceTests
    {
        [ClassInitialize]
        public static void Init(TestContext textContext)
        {
            SboApp.Connect(SboApp.DebugConnectionString);

            SboRecordset.NonQuery("DELETE FROM [@ITCO_FW_SETTINGS] WHERE [Code] LIKE 'test%'");
        }

        [TestMethod]
        public void TestNoSettings()
        {
            var stringValue = SettingService.GetSettingByKey<string>("test.string");
            var intValue = SettingService.GetSettingByKey<int>("test.int");
            var nullintValue = SettingService.GetSettingByKey<int?>("test.int");
            var decimalValue = SettingService.GetSettingByKey<decimal>("test.decimal");
            var datetimeValue = SettingService.GetSettingByKey<DateTime>("test.date");
            var boolValue = SettingService.GetSettingByKey<bool>("test.bool");
            var userStringValue = SettingService.GetCurrentUserSettingByKey<string>("test.user");

            Assert.AreEqual(null, stringValue);
            Assert.AreEqual(0, intValue);
            Assert.AreEqual(null, nullintValue);
            Assert.AreEqual(0m, decimalValue);
            Assert.AreEqual(new DateTime(), datetimeValue);
            Assert.AreEqual(false, boolValue);
            Assert.AreEqual(null, userStringValue);

        }
    }
}
