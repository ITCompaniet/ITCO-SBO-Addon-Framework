using ITCO.SboAddon.Framework.Constants;
using ITCO.SboAddon.Framework.Dialogs;
using ITCO.SboAddon.Framework.Dialogs.Inputs;
using ITCO.SboAddon.Framework.Helpers;
using ITCO.SboAddon.Framework.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace ITCO.SboAddon.Framework.Tester
{
    internal class TestContext : SboAddonContext
    {
        public TestContext()
            : base()
        {
            try
            {
                if (!SboApp.ApplicationConnected)
                    throw new Exception("SboApp not Connected");

                //Test_Form();
                //Test_MenuCreate();
                Test_GetString();
                //Test_Setting();
                //Test_SettingAsk();
   
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}\nExiting...");
                Application.Exit();
            }
        }

        private void Test_SettingAsk()
        {
            var setting_ask = SettingService.GetSettingByKey<string>("test.ask", askIfNotFound: true);
        }

        private void Test_Form()
        {
            var form = FormHelper.CreateFormFromResource("Forms.TestForm.srf", "fType1", "fId1");

            form.Visible = true;


            var form2 = FormHelper.CreateFormFromResource("Forms.TestForm.srf", "fType1", "fId3");

            form2.Visible = true;
        }

        private void Test_MenuCreate()
        {
            MenuHelper.AddFolder("Test folder 2015", "ITCO_FO12y", SboMenuItem.Finance)
                .AddItem("Test item 1", "ITCO_IT12y")
                .AddSeparator("s2")
                .AddItem("Test item 2", "ITCO_IT22y")
                .AddFolder("Folder SUb", "ITCO_Sub12y")
                    .AddItem("Test Sub item 2", "ITCO_sIT22y");

            //MenuHelper.LoadMenuItemsFromFormControllers(Assembly.GetExecutingAssembly());
        }

        private void Test_Setting()
        {
            // Skapar blankt setting värde med Titel
            SettingService.InitSetting<string>("test.init", "My Setting");
            SettingService.InitSetting<int>("test.init-int", "My Setting Int");

            // Hämta värde, om det är blank får man en fråga att fylla i
            var setting_init = SettingService.GetSettingByKey<string>("test.init", askIfNotFound: true);

            string setting_str = SettingService.GetSettingByKey<string>("test.string", askIfNotFound: true);
            int setting_int = SettingService.GetSettingByKey<int>("test.int", askIfNotFound: true);
            decimal setting_dec = SettingService.GetSettingByKey<decimal>("test.decimal", askIfNotFound: true);
            DateTime setting_date = SettingService.GetSettingByKey<DateTime>("test.date", askIfNotFound: true);
            bool setting_bool = SettingService.GetSettingByKey<bool>("test.bool", askIfNotFound: true);
            string setting_user = SettingService.GetCurrentUserSettingByKey<string>("test.user", askIfNotFound: true);
        }

        private void Test_GetString()
        {
            IDictionary<string, object> result = InputHelper.GetInputs("Test", new List<IDialogInput>() {
                new TextDialogInput("t1", "Title 1"),
                new TextDialogInput("t2", "Title 2", "def 2"),
                new TextDialogInput("t3", "Title 3 Req", null, true),
                new DateDialogInput("t4", "Date 1"),
                new CheckboxDialogInput("t5", "Check On", true),
                new CheckboxDialogInput("t6", "Check off", false),
                new DropdownDialogInput("t7", "Dropdown 1", new Dictionary<string, string>() {
                    { "o1", "Option 1" },
                    { "o2", "Option 2" }
                }, "o2", true)
            });
        }
    }
}
