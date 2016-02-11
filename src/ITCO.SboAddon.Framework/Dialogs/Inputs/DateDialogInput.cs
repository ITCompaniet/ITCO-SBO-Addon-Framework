using SAPbouiCOM;
using System;
using System.Globalization;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public class DateDialogInput : IDialogInput
    {
        private DateTime? _defaultValue;
        private readonly string _id;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;

        public DateDialogInput(string id, string title, DateTime? defaultValue = null, bool required = false)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
            _required = required;
        }

        public string Id => _id;

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

        public bool Required => _required;

        public string Title => _title;

        public BoFormItemTypes ItemType => BoFormItemTypes.it_EDIT;

        public bool Validated
        {
            get
            {
                if (string.IsNullOrEmpty(_editText.Value) && _required)                
                    return false;                

                return true;
            }
        }

        public BoDataType DataType => BoDataType.dt_DATE;

        public int Length => 0;

        public object GetValue()
        {
            if (_editText.Value.Length != 8)
                return null;
            
            return DateTime.ParseExact(_editText.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}
