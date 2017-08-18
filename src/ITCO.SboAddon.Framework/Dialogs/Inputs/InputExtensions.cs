
using System;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    /// <summary>
    /// Input Exensions
    /// </summary>
    public static class InputExtensions
    {
        /// <summary>
        /// Add TextDialog
        /// </summary>
        /// <param name="inputHelper"></param>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="defaultValue"></param>
        /// <param name="required"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static InputHelper AddText(this InputHelper inputHelper, string id, string title, string defaultValue = null, bool required = false, int length = 20)
        {
            return inputHelper.AddInput(new TextDialogInput(id, title, defaultValue, required, length));
        }

        /// <summary>
        /// Create Directory Button
        /// </summary>
        /// <param name="form"></param>
        /// <param name="id"></param>
        /// <param name="yPos"></param>
        /// <param name="action"></param>
        public static void CreateDirButton(this Form form, string id, int yPos, Action action)
        {
            var dirButton = form.Items.Add($"{id}E", BoFormItemTypes.it_BUTTON);

            dirButton.Top = yPos + 2;
            dirButton.Left = 235;
            dirButton.Height = 11;
            dirButton.Width = 13;

            var dirButtonSpec = dirButton.Specific as Button;
            dirButtonSpec.Caption = "...";
            dirButtonSpec.ClickAfter += (sboObject, val) => action();
        }
    }
}
