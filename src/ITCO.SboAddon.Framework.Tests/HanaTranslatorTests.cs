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
            var hanaQuery = SimpleHanaTranslator.ConvertSqlToHana("SELECT [ABC] FROM [T0]");
            
            Assert.AreEqual("SELECT \"ABC\" FROM \"T0\"", hanaQuery);
        }
    }
}
