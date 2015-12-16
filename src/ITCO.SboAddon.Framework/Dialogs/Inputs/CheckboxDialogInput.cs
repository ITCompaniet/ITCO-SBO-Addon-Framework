using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public class CheckboxDialogInput : IDialogInput
    {
        private bool _defaultValue;
        private readonly string _id;
        private bool _required;
        private string _title;
        private Item _item;
        private CheckBox _checkBox;

        public CheckboxDialogInput(string id, string title, bool defaultValue = false)
        {
            _id = id;
            _title = title;
            _defaultValue = defaultValue;
        }

        public string Id
        {
            get { return _id; }
        }

        public Item Item
        {
            set
            {
                _item = value;
                _checkBox = _item.Specific as CheckBox;
                _checkBox.Checked = _defaultValue;
                _checkBox.DataBind.SetBound(true, "", _id);
            }
        }

        public bool Required
        {
            get { return _required; }
        }

        public string Title
        {
            get { return _title; }
        }

        public BoFormItemTypes ItemType
        {
            get { return BoFormItemTypes.it_CHECK_BOX; }
        }

        public bool Validated
        {
            get { return true; }
        }

        public BoDataType DataType
        {
            get { return BoDataType.dt_SHORT_TEXT; }
        }

        public int Length
        {
            get { return 1; }
        }

        public object GetValue()
        {
            return _checkBox.Checked;
        }
    }
}
