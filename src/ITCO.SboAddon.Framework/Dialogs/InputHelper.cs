using ITCO.SboAddon.Framework.Dialogs.Inputs;
using SAPbouiCOM;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ITCO.SboAddon.Framework.Dialogs
{
    /// <summary>
    /// Generate SBO Input Dialog
    /// </summary>
    public class InputHelper
    {
        private static Form _form;
        private static int _yPos;
        private readonly IList<IDialogInput> _dialogInputs = new List<IDialogInput>();
        
        public static InputHelper GetInputs(string title)
        {
            var formCreator = SboApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams) as FormCreationParams;
            formCreator.FormType = "ITCO_FW_Dialog";
            _form = SboApp.Application.Forms.AddEx(formCreator);
            _form.Title = title;
            _form.Height = 300;
            _form.Width = 250;

            _yPos = 5;

            return new InputHelper();
        }
        
        public InputHelper AddInput(IDialogInput input)
        {
            _dialogInputs.Add(input);
            return this;
        }

        public IDictionary<string, object> Result()
        {
            _form.Height = 100 + (_dialogInputs.Count() * 15);

            foreach (var dialogInput in _dialogInputs)
            {
                _yPos += 15;
                // Caption
                var titleText = _form.Items.Add($"T{dialogInput.Id}", BoFormItemTypes.it_STATIC).Specific as StaticText;
                titleText.Item.Top = _yPos;
                titleText.Item.Left = 10;
                titleText.Item.Width = 100;
                titleText.Caption = dialogInput.Title;

                // Datasource
                _form.DataSources.UserDataSources.Add(dialogInput.Id, dialogInput.DataType, dialogInput.Length);

                // Input
                var item = _form.Items.Add(dialogInput.Id, dialogInput.ItemType);
                item.Top = _yPos;
                item.Left = 100;
                dialogInput.Item = item;
            }

            _yPos += 20;

            var okButton = _form.Items.Add("okButton", BoFormItemTypes.it_BUTTON).Specific as Button;
            okButton.Caption = "Ok";
            okButton.Item.Top = _yPos;
            okButton.Item.Left = 100;

            _form.DefButton = "okButton";
            _form.Visible = true;

            var wait = new ManualResetEvent(false);
            okButton.PressedAfter += (o, e) =>
            {
                wait.Set();
            };

            wait.WaitOne();
            wait.Reset();

            while (_dialogInputs.Any(d => !d.Validated))
            {
                var invalidInputMessage = "Missing values in: " + string.Join(", ", _dialogInputs.Where(d => !d.Validated).Select(i => i.Title));
                SboApp.Application.StatusBar.SetText(invalidInputMessage);

                wait.WaitOne();
                wait.Reset();
            }
            
            var resultDict = _dialogInputs.ToDictionary(
                dialogInput => dialogInput.Id, 
                dialogInput => dialogInput.GetValue());

            _form.Close();

            return resultDict;
        }
    }
}
