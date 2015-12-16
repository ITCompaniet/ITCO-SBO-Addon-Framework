using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public class IntegerDialogInput : IDialogInput
    {
        private readonly string _id;
        private int? _defaultValue;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;

        public IntegerDialogInput(string id, string title, int? defaultValue = null, bool required = false)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
            _required = required;
        }

        public string Id
        {
            get { return _id; }
        }

        public bool Required
        {
            get { return _required; }
        }

        public string Title
        {
            get { return _title; }
        }

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

        public BoFormItemTypes ItemType
        {
            get { return BoFormItemTypes.it_EDIT; }
        }

        public bool Validated
        {
            get
            {
                if (string.IsNullOrEmpty(_editText.Value) && _required)
                    return false;

                int intValue = 0;
                if (!int.TryParse(_editText.Value, out intValue))
                    return false;

                return true;
            }
        }

        public BoDataType DataType
        {
            get { return BoDataType.dt_LONG_NUMBER; }
        }

        public int Length
        {
            get { return 0; }
        }

        public object GetValue()
        {
            return int.Parse(_editText.Value);
        }
    }
}
