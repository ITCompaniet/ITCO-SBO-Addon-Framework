using SAPbouiCOM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// Dialog helper
    /// </summary>
    /// <exception cref="DialogCanceledException">Dialog is canceled</exception>
    public static class DialogHelper
    {
        /// <summary>
        /// Generate SBO Dialog
        /// </summary>
        /// <param name="title">Form Title</param>
        /// <param name="dialogInputs">Inputs</param>
        /// <returns>Result</returns>
        public static IDictionary<string, string> GetInputs(string title, ICollection<IDialogInput> dialogInputs)
        {
            var formCreator = SboApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams) as FormCreationParams;
            formCreator.FormType = "ITCO_FW_GetString";
            var form = SboApp.Application.Forms.AddEx(formCreator);
            form.Title = title;
            form.Height = 100 + (dialogInputs.Count() * 15);
            form.Width = 250;

            var top = 5;

            foreach (var dialogInput in dialogInputs)
            {
                top += 15;
                var titleText = form.Items.Add(string.Format("Title-{0}", dialogInput.Id), BoFormItemTypes.it_STATIC).Specific as StaticText;
                titleText.Item.Top = top;
                titleText.Item.Left = 10;
                titleText.Item.Width = 100;
                titleText.Caption = dialogInput.Title;

                var item = form.Items.Add(dialogInput.Id, dialogInput.Type);

                item.Top = top;
                item.Left = 100;
                dialogInput.Item = item;
            }

            top += 20;

            var okButton = form.Items.Add("okButton", BoFormItemTypes.it_BUTTON).Specific as SAPbouiCOM.Button;
            okButton.Caption = "Ok";
            okButton.Item.Top = top;
            okButton.Item.Left = 100;

            form.DefButton = "okButton";
            form.Visible = true;

            var wait = new ManualResetEvent(false);
            okButton.PressedAfter += (o, e) => { wait.Set(); };

            var validated = false;
            while (!validated)
            {
                wait.WaitOne();
                wait.Reset();
                validated = dialogInputs.Count(d => !d.Validated) == 0;
            }

            var resultDict = new Dictionary<string, string>();
            foreach (var dialogInput in dialogInputs)
            {
                resultDict.Add(dialogInput.Id, dialogInput.GetValue());
            }
            
            return resultDict;
        }


        #region Form Dialogs
        /// <summary>
        /// Opens 'Save File Dialog'
        /// </summary>
        /// <param name="filter">File filter, eg. "Text files (*.txt)|*.txt|All files (*.*)|*.*"</param>
        /// <param name="defaultFileName">Default file name</param>
        /// <returns></returns>
        public static string SaveFile(string filter = null, string defaultFileName = null)
        {
            var fileSelector = new SaveFileDialog();

            if (defaultFileName != null)
                fileSelector.FileName = defaultFileName;

            if (filter != null)
                fileSelector.Filter = filter;

            var result = new STAInvoker<SaveFileDialog, DialogResult>(fileSelector, (x) =>
            {
                return x.ShowDialog();
            }).Invoke();

            if (result != DialogResult.OK)
                throw new DialogCanceledException();

            return fileSelector.FileName;
        }

        /// <summary>
        /// Opens 'Open File Dialog'
        /// </summary>
        /// <param name="filter">File filter, eg. "Text files (*.txt)|*.txt|All files (*.*)|*.*"</param>
        /// <param name="defaultFileName"></param>
        /// <returns></returns>
        public static string OpenFile(string filter = null, string defaultFileName = null)
        {
            var fileSelector = new OpenFileDialog();

            if (defaultFileName != null)
                fileSelector.FileName = defaultFileName;

            if (filter != null)
                fileSelector.Filter = filter;

            var result = new STAInvoker<OpenFileDialog, DialogResult>(fileSelector, (x) =>
            {
                return x.ShowDialog();
            }).Invoke();

            if (result != DialogResult.OK)
                throw new DialogCanceledException();

            return fileSelector.FileName;
        }
        /// <summary>
        /// Opens 'Folder Browser Dialog'
        /// </summary>
        /// <param name="defaultFolder"></param>
        /// <returns></returns>
        public static string FolderBrowser(string defaultFolder = null)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (defaultFolder != null)
                folderBrowserDialog.SelectedPath = defaultFolder;

            var result = new STAInvoker<FolderBrowserDialog, DialogResult>(folderBrowserDialog, (x) =>
            {
                return x.ShowDialog();
            }).Invoke();

            if (result != DialogResult.OK)
                throw new DialogCanceledException();

            return folderBrowserDialog.SelectedPath;
        }
        #endregion

    }

    public interface IDialogInput
    {
        string Id { get; }
        string Title { get; }
        bool Required { get; }
        Item Item { set; }
        BoFormItemTypes Type { get; }
        bool Validated { get; }

        string GetValue();
    }

    public class TextDialogInput : IDialogInput
    {
        private string _id;
        private string _defaultValue;
        private bool _required;
        private string _title;
        private Item _item;
        private EditText _editText;

        public TextDialogInput(string id, string title, string defaultValue = null, bool required = false)
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
                _editText.Value = _defaultValue;
            }
        }

        public BoFormItemTypes Type
        {
            get { return BoFormItemTypes.it_EDIT; }
        }

        public bool Validated
        {
            get
            {
                if (string.IsNullOrEmpty(_editText.Value) && _required)
                {
                    _editText.BackColor = Convert.ToInt32("9999FF", 16);
                    return false;
                }

                return true;
            }
        }

        public string GetValue()
        {
            return _editText.Value;
        }
    }

    /// <summary>
    /// Dialog is canceled
    /// </summary>
    public class DialogCanceledException : Exception
    {
        public DialogCanceledException()
            :base("Dialog canceled")
        { }
    }

}
