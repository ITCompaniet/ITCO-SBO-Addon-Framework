using System;
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
        public const string FormType = "ITCO_FW_Dialog";

        private Form _form;
        private int _yPos;
        private readonly List<IDialogInput> _dialogInputs = new List<IDialogInput>();
        private bool _canceled;
        private ManualResetEvent _formWait = new ManualResetEvent(false);

        public InputHelper(string title, params IDialogInput[] dialogs)
        {
            var formCreator = SboApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams) as FormCreationParams;
            formCreator.FormType = FormType;
            formCreator.BorderStyle = BoFormBorderStyle.fbs_Fixed;

            _form = SboApp.Application.Forms.AddEx(formCreator);
            _form.Title = title;
            _form.Height = 300;
            _form.Width = 250;

            _yPos = 5;

            if (dialogs != null)
                _dialogInputs.AddRange(dialogs);
        }

        public static InputHelper GetInputs(string title, params IDialogInput[] dialogs)
        {
            return new InputHelper(title, dialogs);
        }
        
        /// <summary>
        /// Add Dialog Input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public InputHelper AddInput(IDialogInput input)
        {
            _dialogInputs.Add(input);
            return this;
        }

        /// <summary>
        /// Wait for Dialog result
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="DialogCanceledException">Dialog in canceled</exception>
        public IDictionary<string, object> Result()
        {
            _form.Height = 100 + (_dialogInputs.Count()*15);

            foreach (var dialogInput in _dialogInputs)
            {
                _yPos += 15;
                // Caption
                var titleText = _form.Items.Add($"T{dialogInput.Id}", BoFormItemTypes.it_STATIC).Specific as StaticText;
                titleText.Item.Top = _yPos;
                titleText.Item.Left = 10;
                titleText.Item.Width = 150;
                titleText.Caption = dialogInput.Title;

                // Datasource
                _form.DataSources.UserDataSources.Add(dialogInput.Id, dialogInput.DataType, dialogInput.Length);

                if (dialogInput.DefaultValue != null)
                    _form.DataSources.UserDataSources.Item(dialogInput.Id).ValueEx = dialogInput.DefaultValue;

                // Input
                var item = _form.Items.Add(dialogInput.Id, dialogInput.ItemType);
                item.Top = _yPos;
                item.Left = 150;
                dialogInput.Item = item;
            }

            _yPos += 20;

            var okButton = _form.Items.Add("okButton", BoFormItemTypes.it_BUTTON).Specific as Button;
            okButton.Caption = "Ok";
            okButton.Item.Top = _yPos;
            okButton.Item.Left = 100;

            _form.DefButton = "okButton";
            _form.Visible = true;
            
            okButton.PressedAfter += (o, e) =>
            {
                _formWait.Set();
            };

            SboApp.Application.ItemEvent += ApplicationOnItemEvent;

            _formWait.WaitOne();
            _formWait.Reset();

            try
            {
                if (_canceled)
                    throw new DialogCanceledException();

                while (_dialogInputs.Any(d => !d.Validated))
                {
                    var invalidInputMessage = "Missing values in: " +
                                              string.Join(", ",
                                                  _dialogInputs.Where(d => !d.Validated).Select(i => i.Title));
                    SboApp.Application.StatusBar.SetText(invalidInputMessage);

                    _formWait.WaitOne();
                    _formWait.Reset();

                    if (_canceled)
                        throw new DialogCanceledException();
                }

                var resultDict = _dialogInputs.ToDictionary(
                    dialogInput => dialogInput.Id,
                    dialogInput => dialogInput.GetValue());

                _form.Close();
                return resultDict;
            }
            finally
            {
                SboApp.Application.ItemEvent -= ApplicationOnItemEvent;
            }
        }

        private void ApplicationOnItemEvent(string formUid, ref ItemEvent pVal, out bool bubbleEvent)
        {
            bubbleEvent = true;
            if (pVal.EventType == BoEventTypes.et_FORM_UNLOAD
                && pVal.FormTypeEx == FormType
                && pVal.FormUID == _form.UniqueID
                && !pVal.BeforeAction)
            {
                _canceled = true;
                _formWait.Set();
            }
        }
    }
}
