using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCO.SboAddon.Framework.Helpers;

namespace ITCO.SboAddon.Framework.Tests
{
    [TestClass]
    public class TestDialogs
    {
        [TestMethod]
        public void TestMethod1()
        {
            var save = DialogHelper.SaveFile("Text files (*.txt)|*.txt" , "c:\\temp\\falk.txt");
            var open = DialogHelper.OpenFile("Text files (*.txt)|*.txt", "c:\\temp\\falk.txt");
            var folder = DialogHelper.FolderBrowser("c:\\temp\\");
        }
    }
}
