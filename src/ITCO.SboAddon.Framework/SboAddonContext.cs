using SAPbouiCOM;
using System;
using System.Windows.Forms;

namespace ITCO.SboAddon.Framework
{
    /// <summary>
    /// SBO ApplicationContext
    /// </summary>
    /// <example>
    /// [STAThread]
    /// static void Main()
    /// {
    ///     Application.Run(new SboAddonContext());
    /// }
    /// </example>
    public class SboAddonContext : ApplicationContext
    {
        /// <summary>
        /// Connecting to SBO
        /// </summary>
        public SboAddonContext()
        {
            try
            {
                // Debug connection string
                var connectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";

                // Get connection string from SBO
                if (Environment.GetCommandLineArgs().Length > 1)
                    connectionString = Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));

                SboApp.Connect(connectionString);

                Setup();

                SboApp.Application.SetFilter(EventFilters());
                MenuItems();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("SBO Connect Error: {0}\nExiting...", e.Message));
                ExitThread();
            }
        }

        public virtual void Setup()
        {

        }

        public virtual EventFilters EventFilters()
        {
            return new EventFilters();
        }

        public virtual void MenuItems()
        {

        }
    }
}
