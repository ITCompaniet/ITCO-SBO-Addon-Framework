using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public class TextDialogInput : IDialogInput
    {
        private readonly string _id;
        private string _defaultValue;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;
        private int _length;

        public TextDialogInput(string id, string title, string defaultValue = null, bool required = false, int length = 20)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
            _required = required;
            _length = length;
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
                _editText.Value = _defaultValue;
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

                return true;
            }
        }

        public BoDataType DataType
        {
            get { return BoDataType.dt_SHORT_TEXT; }
        }

        public int Length
        {
            get { return _length; }
        }

        public object GetValue()
        {
            return _editText.Value;
        }
    }
}
