using System;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public class OpenFileDialogInput : IDialogInput
    {
        private readonly string _id;
        private string _defaultValue;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;

        public OpenFileDialogInput(string id, string title, string defaultValue = null, bool required = false)
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
                _editText.Value = _defaultValue;
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

                return true;
            }
        }

        public BoDataType DataType => BoDataType.dt_LONG_TEXT;

        public int Length => 255;

        public string DefaultValue => _defaultValue;

        public object GetValue()
        {
            return _editText.Value;
        }

        public void Extras(Form form, int yPos)
        {
            form.CreateDirButton(_id, yPos, OpenFile);
        }

        private void OpenFile()
        {
            try
            {
                _editText.Value = FileDialogHelper.OpenFile(null, _editText.Value);
            }
            catch (DialogCanceledException)
            {
            }
            catch (Exception e)
            {
                SboApp.Logger.Error(e.Message);
            }
        }
    }
}
