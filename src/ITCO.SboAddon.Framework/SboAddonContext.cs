using ITCO.SboAddon.Framework.Helpers;
using ITCO.SboAddon.Framework.Setup;
using SAPbouiCOM;
using System;
using System.Reflection;
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
                var mainAssembly = Assembly.GetEntryAssembly();

                SboApp.Connect();

                SetupManager.FindAndRunSetups(mainAssembly);

                SboApp.Application.SetFilter(EventFilters());
                MenuItems();
                BindEvents();

                MenuHelper.LoadAndAddMenuItemsFromFormControllers(mainAssembly);
            }
            catch (Exception e)
            {
                MessageBox.Show($"SBO Connect Error: {e.Message}\nExiting...");
                ExitThread();
            }
        }

        /// <summary>
        /// Event Filters
        /// </summary>
        /// <returns></returns>
        public virtual EventFilters EventFilters()
        {
            var eventFilters = new EventFilters();
            //eventFilters.Add(BoEventTypes.et_MENU_CLICK);
            eventFilters.Add(BoEventTypes.et_ALL_EVENTS);
            return eventFilters;
        }

        /// <summary>
        /// Setup Menu Items
        /// </summary>
        public virtual void MenuItems()
        {

        }

        /// <summary>
        /// Bind Events
        /// </summary>
        public virtual void BindEvents()
        {
        }
    }
}
