using ITCO.SboAddon.Framework.Helpers;
using ITCO.SboAddon.Framework.Setup;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<AddonMenuEvent> _addonMenuEvents = new List<AddonMenuEvent>();
        /// <summary>
        /// Connecting to SBO
        /// </summary>
        public SboAddonContext()
        {
            System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;
            
            try
            {
                // Debug connection string
                var connectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";

                // Get connection string from SBO
                if (Environment.GetCommandLineArgs().Length > 1)
                    connectionString = Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));

                SboApp.Connect(connectionString);

                SetupManager.FindAndRunSetups(Assembly.GetEntryAssembly());

                SboApp.Application.SetFilter(EventFilters());
                MenuItems();

                var formMenuEvents = MenuHelper.LoadMenuItemsFromFormControllers(Assembly.GetEntryAssembly());
                foreach (var item in formMenuEvents)
                {
                    AddMenuItemEvent(item.Title, item.MenuId, item.ParentMenuId, item.Action, item.Position);
                }

                SboApp.Application.MenuEvent += Application_MenuEvent;
            }
            catch (Exception e)
            {
                MessageBox.Show($"SBO Connect Error: {e.Message}\nExiting...");
                ExitThread();
            }
        }

        #region Events
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            foreach (var item in _addonMenuEvents)
            {
                // TODO: Remove menu items
            }
        }

        private void Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (pVal.BeforeAction)
                return;

            var menuId = pVal.MenuUID;
            var menuEvent = _addonMenuEvents.FirstOrDefault(e => e.MenuId == menuId);

            if (menuEvent != null)
            {
                menuEvent.Action.Invoke();
            }
        }
        #endregion

        /// <summary>
        /// Add Menu Item with action
        /// </summary>
        /// <param name="title"></param>
        /// <param name="menuId"></param>
        /// <param name="parentMenuId"></param>
        /// <param name="action"></param>
        /// <param name="position"></param>
        protected void AddMenuItemEvent(string title, string menuId, string parentMenuId, Action action, int position = -1)
        {
            _addonMenuEvents.Add(new AddonMenuEvent
            {
                MenuId = menuId,
                ParentMenuId = parentMenuId,
                Action = action
            });
            MenuHelper.AddItem(title, menuId, parentMenuId, position);
        }

        public virtual EventFilters EventFilters()
        {
            var eventFilters = new EventFilters();
            eventFilters.Add(BoEventTypes.et_MENU_CLICK);
            return eventFilters;
        }

        public virtual void MenuItems()
        {

        }

    }

    public class AddonMenuEvent
    {
        public string ParentMenuId { get; set; }
        public string MenuId { get; set; }
        public Action Action { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
    }
}
