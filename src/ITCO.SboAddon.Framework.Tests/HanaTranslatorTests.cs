using ITCO.SboAddon.Framework.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ITCO.SboAddon.Framework.Tests
{
    [TestClass]
    public class HanaTranslatorTests
    {
        [TestMethod]
        public void ConvertSqlToHana_Test()
        {
            var hanaQuery = SimpleHanaTranslator.ConvertSqlToHana("SELECT ISNULL([ABC], '') + '-' + [DEF], [IntVal] + 5 AS [IntValPlus], GETDATE() AS [DateTime] FROM [T0]");
            
            Assert.AreEqual("SELECT IFNULL(\"ABC\", '') || '-' || \"DEF\", \"IntVal\" + 5 AS \"IntValPlus\", NOW() AS \"DateTime\" FROM \"T0\"", hanaQuery);
        }
    }
}
