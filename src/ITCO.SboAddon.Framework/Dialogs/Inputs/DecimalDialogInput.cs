using System;
using SAPbouiCOM;
using System.Globalization;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    /// <summary>
    /// DecimalDialogInput
    /// </summary>
    public class DecimalDialogInput : IDialogInput
    {
        private readonly string _id;
        private decimal? _defaultValue;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;
        /// <summary>
        /// DecimalDialogInput
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="defaultValue"></param>
        /// <param name="required"></param>
        public DecimalDialogInput(string id, string title, decimal? defaultValue = null, bool required = false)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
            _required = required;
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
                if (_defaultValue.HasValue)
                    _editText.Value = _defaultValue.Value.ToString();

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

                decimal decimalValue = 0;
                if (!decimal.TryParse(_editText.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
                    return false;

                return true;
            }
        }
        /// <summary>
        /// Get DataType
        /// </summary>
        public BoDataType DataType => BoDataType.dt_QUANTITY;
        /// <summary>
        /// Get Length
        /// </summary>
        public int Length => 0;
        /// <summary>
        /// Get DefaultValue
        /// </summary>
        public string DefaultValue => _defaultValue == null ? null : Convert.ToString(_defaultValue, CultureInfo.InvariantCulture);
        /// <summary>
        /// Get Value
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return decimal.Parse(_editText.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="form"></param>
        /// <param name="yPos"></param>
        public void Extras(Form form, int yPos)
        {
            throw new NotImplementedException();
        }
    }
}
