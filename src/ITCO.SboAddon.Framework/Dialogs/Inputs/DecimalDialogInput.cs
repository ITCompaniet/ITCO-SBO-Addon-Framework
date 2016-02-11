using SAPbouiCOM;
using System.Globalization;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public class DecimalDialogInput : IDialogInput
    {
        private readonly string _id;
        private decimal? _defaultValue;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;

        public DecimalDialogInput(string id, string title, decimal? defaultValue = null, bool required = false)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
            _required = required;
        }

        public string Id => _id;

        public bool Required => _required;

        public string Title => _title;

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

        public BoFormItemTypes ItemType => BoFormItemTypes.it_EDIT;

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

        public BoDataType DataType => BoDataType.dt_QUANTITY;

        public int Length => 0;

        public object GetValue()
        {
            return decimal.Parse(_editText.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}
