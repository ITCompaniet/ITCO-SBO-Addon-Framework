using ITCO.SboAddon.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ITCO.SboAddon.Framework.Tester
{
    internal class TestContext : ApplicationContext
    {
        public TestContext()
        {
            var connectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";

            try
            {
                SboApp.Connect(connectionString);

                Test_GetString();       
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Error: {0}\nExiting...", e.Message));
                Application.Exit();
            }
        }

        private void Test_GetString()
        {
            var s = DialogHelper.GetInputs("Test", new List<IDialogInput>() {
                new TextDialogInput("t1", "Title 1"),
                new TextDialogInput("t2", "Title 2", "def 2"),
                new TextDialogInput("t3", "Title 3 Req", null, true),
                new TextDialogInput("t4", "Title 4")
            });
        }
    }
}
