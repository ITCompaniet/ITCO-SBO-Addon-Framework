using System;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    /// <summary>
    /// TextDialogInput
    /// </summary>
    public class TextDialogInput : IDialogInput
    {
        private readonly string _id;
        private string _defaultValue;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;
        private int _length;

        /// <summary>
        /// TextDialogInput
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="defaultValue"></param>
        /// <param name="required"></param>
        /// <param name="length"></param>
        public TextDialogInput(string id, string title, string defaultValue = null, bool required = false, int length = 20)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
            _required = required;
            _length = length;
        }
        /// <summary>
        /// Get Id
        /// </summary>
        public string Id => _id;
        /// <summary>
        /// Get Required
        /// </summary>
        public bool Required => _required;
        /// <summary>
        /// Get Title
        /// </summary>
        public string Title => _title;
        /// <summary>
        /// Set Item
        /// </summary>
        public Item Item
        {
            set
            {
                _item = value;
                _editText = _item.Specific as EditText;
                _editText.Value = _defaultValue;
                _editText.DataBind.SetBound(true, "", _id);
            }
        }
        /// <summary>
        /// Get ItemType
        /// </summary>
        public BoFormItemTypes ItemType => BoFormItemTypes.it_EDIT;
        /// <summary>
        /// Get Validated
        /// </summary>
        public bool Validated
        {
            get
            {
                if (string.IsNullOrEmpty(_editText.Value) && _required)                
                    return false;                

                return true;
            }
        }
        /// <summary>
        /// Get DataType
        /// </summary>
        public BoDataType DataType => BoDataType.dt_LONG_TEXT;
        /// <summary>
        /// Get Length
        /// </summary>
        public int Length => _length;
        /// <summary>
        /// Get Default Value
        /// </summary>
        public string DefaultValue => _defaultValue;
        /// <summary>
        /// Get Value of EditTExt
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _editText.Value;
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
