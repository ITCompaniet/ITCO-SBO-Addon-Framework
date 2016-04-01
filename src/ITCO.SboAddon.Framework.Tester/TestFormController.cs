using System;
using ITCO.SboAddon.Framework.Constants;
using ITCO.SboAddon.Framework.Extensions;
using ITCO.SboAddon.Framework.Forms;
using ITCO.SboAddon.Framework.Setup;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Tester
{
    public class TestFormController : FormController, IFormMenuItem
    {
        public override void FormCreated()
        {
            var comboboxItem = Form.Items.Add("comboT01", BoFormItemTypes.it_COMBO_BOX);
            comboboxItem.Visible = true;

            var combobox = comboboxItem.Specific as ComboBox;

            combobox.AddComboBoxValues("SELECT CardCode, Cardname FROM OCRD");

            using (Form.FreezeEx())
            {
                
            }
            SetupManager.RunSetup(new TestSetup());
        }

        #region IFormMenuItem
        public string MenuItemTitle => "My Test Form";

        public string ParentMenuItemId => SboMenuItem.Inventory;

        #endregion
    }
}
