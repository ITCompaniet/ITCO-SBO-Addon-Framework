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
        private readonly List<AddonMenuEvent> _addonMenuEvents = new List<AddonMenuEvent>();
        /// <summary>
        /// Connecting to SBO
        /// </summary>
        public SboAddonContext()
        {
            System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;
            
            try
            {
                var mainAssembly = Assembly.GetEntryAssembly();

                SboApp.Connect();

                SetupManager.FindAndRunSetups(mainAssembly);

                SboApp.Application.SetFilter(EventFilters());
                MenuItems();
                BindEvents();

                var formMenuEvents = MenuHelper.LoadMenuItemsFromFormControllers(mainAssembly);
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

        private void Application_MenuEvent(ref MenuEvent pVal, out bool bubbleEvent)
        {
            bubbleEvent = true;

            if (pVal.BeforeAction)
                return;

            var menuId = pVal.MenuUID;
            var menuEvent = _addonMenuEvents.FirstOrDefault(e => e.MenuId == menuId);

            menuEvent?.Action();
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

    /// <summary>
    /// Menu Item Event
    /// </summary>
    public class AddonMenuEvent
    {
        /// <summary>
        /// Parent Menu Id
        /// </summary>
        public string ParentMenuId { get; set; }

        /// <summary>
        /// Menu ID
        /// </summary>
        public string MenuId { get; set; }

        /// <summary>
        /// Action when clicking the menu item
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Position, -1 = Last
        /// </summary>
        public int Position { get; set; }
    }
}
