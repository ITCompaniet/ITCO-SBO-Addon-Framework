using ITCO.SboAddon.Framework.Dialogs.Inputs;
using SAPbouiCOM;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ITCO.SboAddon.Framework.Dialogs
{
    public class InputHelper
    {
        /// <summary>
        /// Generate SBO Dialog
        /// </summary>
        /// <param name="title">Form Title</param>
        /// <param name="dialogInputs">Inputs</param>
        /// <returns>Result</returns>
        public static IDictionary<string, object> GetInputs(string title, ICollection<IDialogInput> dialogInputs)
        {
            var formCreator = SboApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams) as FormCreationParams;
            formCreator.FormType = "ITCO_FW_GetString";
            var form = SboApp.Application.Forms.AddEx(formCreator);
            form.Title = title;
            form.Height = 100 + (dialogInputs.Count() * 15);
            form.Width = 250;

            var top = 5;

            foreach (var dialogInput in dialogInputs)
            {
                top += 15;
                // Caption
                var titleText = form.Items.Add(string.Format("T{0}", dialogInput.Id), BoFormItemTypes.it_STATIC).Specific as StaticText;
                titleText.Item.Top = top;
                titleText.Item.Left = 10;
                titleText.Item.Width = 100;
                titleText.Caption = dialogInput.Title;

                // Datasource
                form.DataSources.UserDataSources.Add(dialogInput.Id, dialogInput.DataType, dialogInput.Length);

                // Input
                var item = form.Items.Add(dialogInput.Id, dialogInput.ItemType);
                item.Top = top;
                item.Left = 100;
                dialogInput.Item = item;
            }

            top += 20;

            var okButton = form.Items.Add("okButton", BoFormItemTypes.it_BUTTON).Specific as Button;
            okButton.Caption = "Ok";
            okButton.Item.Top = top;
            okButton.Item.Left = 100;

            form.DefButton = "okButton";
            form.Visible = true;

            var wait = new ManualResetEvent(false);
            okButton.PressedAfter += (o, e) => { wait.Set(); };

            var validated = false;
            while (!validated)
            {
                wait.WaitOne();
                wait.Reset();
                validated = dialogInputs.Count(d => !d.Validated) == 0;
            }

            var resultDict = new Dictionary<string, object>();
            foreach (var dialogInput in dialogInputs)
            {
                resultDict.Add(dialogInput.Id, dialogInput.GetValue());
            }

            form.Close();

            return resultDict;
        }

    }
}
