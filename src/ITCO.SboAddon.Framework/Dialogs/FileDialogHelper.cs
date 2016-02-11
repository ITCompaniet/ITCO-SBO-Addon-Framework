using ITCO.SboAddon.Framework.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ITCO.SboAddon.Framework.Dialogs
{
    /// <summary>
    /// Dialog helper
    /// </summary>
    /// <exception cref="DialogCanceledException">Dialog is canceled</exception>
    public static class FileDialogHelper
    {
        #region File/Folder Dialogs
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

            var result = new STAInvoker<SaveFileDialog, DialogResult>(fileSelector, (x) => x.ShowDialog(ForegroundWindowWrapper.GetWindow())).Invoke();

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

            var result = new STAInvoker<OpenFileDialog, DialogResult>(fileSelector, (x) => x.ShowDialog(ForegroundWindowWrapper.GetWindow())).Invoke();

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

            var result = new STAInvoker<FolderBrowserDialog, DialogResult>(folderBrowserDialog, (x) => x.ShowDialog(ForegroundWindowWrapper.GetWindow())).Invoke();

            if (result != DialogResult.OK)
                throw new DialogCanceledException();

            return folderBrowserDialog.SelectedPath;
        }
        #endregion

    }
    
    /// <summary>
    /// Dialog is canceled
    /// </summary>
    public class DialogCanceledException : Exception
    {
        /// <summary>
        /// Dialog is Canceled
        /// </summary>
        public DialogCanceledException()
            :base("Dialog canceled")
        { }
    }
    
    internal class ForegroundWindowWrapper : IWin32Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public virtual IntPtr Handle { get; }

        public ForegroundWindowWrapper(IntPtr handle)
        {
            this.Handle = handle;
        }

        public static ForegroundWindowWrapper GetWindow()
        {
            return new ForegroundWindowWrapper(GetForegroundWindow());
        }
    }
}
