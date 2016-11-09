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
    }
}
