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

        public string Id => _id;

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

        public bool Required => _required;

        public string Title => _title;

        public BoFormItemTypes ItemType => BoFormItemTypes.it_CHECK_BOX;

        public bool Validated => true;

        public BoDataType DataType => BoDataType.dt_SHORT_TEXT;

        public int Length => 1;

        public string DefaultValue => null;

        public object GetValue()
        {
            return _checkBox.Checked;
        }
    }
}
