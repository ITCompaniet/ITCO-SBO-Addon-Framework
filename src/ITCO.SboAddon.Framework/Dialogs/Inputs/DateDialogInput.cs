using SAPbouiCOM;
using System;
using System.Globalization;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    /// <summary>
    /// DateInputDialog
    /// </summary>
    public class DateDialogInput : IDialogInput
    {
        private DateTime? _defaultValue;
        private readonly string _id;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;
        /// <summary>
        /// DateDialogInput
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="defaultValue"></param>
        /// <param name="required"></param>
        public DateDialogInput(string id, string title, DateTime? defaultValue = null, bool required = false)
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
        /// Set Item
        /// </summary>
        public Item Item
        {
            set
            {
                _item = value;
                _editText = _item.Specific as EditText;
                if (_defaultValue.HasValue)
                    _editText.Value = _defaultValue.Value.ToString("yyyy-MM-dd");

                _editText.DataBind.SetBound(true, "", _id);
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
        public BoDataType DataType => BoDataType.dt_DATE;
        /// <summary>
        /// Get Lenght
        /// </summary>
        public int Length => 0;
        /// <summary>
        /// Get DefaultValue
        /// </summary>
        public string DefaultValue => _defaultValue?.ToString("yyyyMMdd");
        /// <summary>
        /// Get Value
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            if (_editText.Value.Length != 8)
                return null;
            
            return DateTime.ParseExact(_editText.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
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
