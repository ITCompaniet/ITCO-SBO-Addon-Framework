using System;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    /// <summary>
    /// CheckboxDialogInput
    /// </summary>
    public class CheckboxDialogInput : IDialogInput
    {
        private bool _defaultValue;
        private readonly string _id;
        private bool _required;
        private string _title;
        private Item _item;
        private CheckBox _checkBox;

        /// <summary>
        /// CheckboxDialogInput
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="defaultValue"></param>
        public CheckboxDialogInput(string id, string title, bool defaultValue = false)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
            _required = false;
        }
        /// <summary>
        /// Get Id
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Set Item
        /// </summary>
        public Item Item
        {
            set
            {
                _item = value;
                _checkBox = _item.Specific as CheckBox;
                _checkBox.DataBind.SetBound(true, "", _id);
                _checkBox.Checked = _defaultValue;
            }
        }
        /// <summary>
        /// Get Required
        /// </summary>
        public bool Required => _required;
        /// <summary>
        /// Get Title
        /// </summary>
        public string Title => _title;
        /// <summary>
        /// Get ItemType
        /// </summary>
        public BoFormItemTypes ItemType => BoFormItemTypes.it_CHECK_BOX;

        /// <summary>
        /// Get Validated
        /// </summary>
        public bool Validated => true;
        /// <summary>
        /// Get DataType
        /// </summary>
        public BoDataType DataType => BoDataType.dt_SHORT_TEXT;
        /// <summary>
        /// Get Lenght
        /// </summary>
        public int Length => 1;

        /// <summary>
        /// Get DefaultValue
        /// </summary>
        public string DefaultValue => null;

        /// <summary>
        /// Get checked value
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _checkBox.Checked;
        }
        /// <summary>
        /// Extras
        /// </summary>
        /// <param name="form"></param>
        /// <param name="yPos"></param>
        public void Extras(Form form, int yPos)
        {
        }
    }
}
