using System;
using System.Windows.Forms;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// Run Dialogs in STATTread
    /// </summary>
    /// <exception cref="DialogCanceledException">When Dialog is canceled</exception>
    public static class DialogHelper
    {
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
