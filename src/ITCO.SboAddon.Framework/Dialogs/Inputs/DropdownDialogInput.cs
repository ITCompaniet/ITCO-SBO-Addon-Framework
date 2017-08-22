using SAPbouiCOM;
using System;
using System.Collections.Generic;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    /// <summary>
    /// DropDownDialogInput
    /// </summary>
    public class DropdownDialogInput : IDialogInput
    {
        private string _selected;
        private readonly string _id;
        private readonly bool _withEmpty;
        private readonly string _title;
        private Item _item;
        private ComboBox _comboBox;
        private readonly IDictionary<string, string> _options;
        /// <summary>
        /// DropDialogInput
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="options"></param>
        /// <param name="selected"></param>
        /// <param name="withEmpty"></param>
        public DropdownDialogInput(string id, string title, IDictionary<string, string> options, string selected = null, bool withEmpty = true)
        {
            _id = id;
            _title = title;
            _options = options;
            _selected = selected;
            _withEmpty = withEmpty;
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
                _comboBox  = _item.Specific as ComboBox;
                if (_withEmpty)
                foreach (var option in _options)
                {
                    _comboBox.ValidValues.Add(option.Key, option.Value);
                }

                _comboBox.ExpandType = BoExpandType.et_ValueDescription;
                _comboBox.DataBind.SetBound(true, "", _id);
                if (_selected != null)
                    _comboBox.Select(_selected, BoSearchKey.psk_ByValue);
            }
        }
        /// <summary>
        /// Get Required
        /// </summary>
        public bool Required => false;
        /// <summary>
        /// Get Title
        /// </summary>
        public string Title => _title;
        /// <summary>
        /// Get ItemType
        /// </summary>
        public BoFormItemTypes ItemType => BoFormItemTypes.it_COMBO_BOX;
        /// <summary>
        /// Get Validated
        /// </summary>
        public bool Validated => true;
        /// <summary>
        /// Get DataType
        /// </summary>
        public BoDataType DataType => BoDataType.dt_SHORT_TEXT;
        /// <summary>
        /// Get Length
        /// </summary>
        public int Length => 20;
        /// <summary>
        /// Get DefaultValue
        /// </summary>
        public string DefaultValue => null;
        /// <summary>
        /// GEt Value
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _comboBox.Selected.Value;
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
