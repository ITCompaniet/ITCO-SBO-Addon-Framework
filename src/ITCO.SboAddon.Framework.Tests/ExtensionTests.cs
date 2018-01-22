namespace ITCO.SboAddon.Framework.Tests
{
    using Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void AddNewLine_Test()
        {
            Assert.AreEqual("NewLine\n", "".AddNewLine("NewLine"));
            Assert.AreEqual("ExistingText\nNewLine\n", "ExistingText".AddNewLine("NewLine"));
            Assert.AreEqual("ExistingText\nNewLine\n", "ExistingText\n".AddNewLine("NewLine"));
            Assert.AreEqual("ExistingText\nExistingText2\nNewLine\n", "ExistingText\nExistingText2".AddNewLine("NewLine"));
        }

        [TestMethod]
        public void ReturnParameterStyle_String_Test()
        {
            var userQuery = @"SELECT * FROM T0 WHERE C0 = [%0] AND C1 = '[%1]'";
            userQuery = UserQueryExtensions.ReturnParameterStyle(userQuery, ParameterFormat.String);

            Assert.AreEqual(@"SELECT * FROM T0 WHERE C0 = {0} AND C1 = '{1}'", userQuery);
        }

        [TestMethod]
        public void ReturnParameterStyle_Sql_Test()
        {
            var userQuery = @"SELECT * FROM T0 WHERE C0 = [%0] AND C1 = '[%1]'";
            userQuery = UserQueryExtensions.ReturnParameterStyle(userQuery, ParameterFormat.Database);

            Assert.AreEqual(@"SELECT * FROM T0 WHERE C0 = @p0 AND C1 = @p1", userQuery);
        }
    }
}
