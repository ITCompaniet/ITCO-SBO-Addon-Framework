using ITCO.SboAddon.Framework.Forms;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// Helpers for menu handling
    /// </summary>
    public static class MenuHelper
    {
        private static readonly List<AddonMenuEvent> AddonMenuEvents = new List<AddonMenuEvent>();

        private static bool _isInitialized;

        private static void Init()
        {
            if (!_isInitialized)
            {
                BindEvents();
                SboApp.Logger.Debug("MenuHelper.Initialized");
            }

            _isInitialized = true;
        }

        private static void BindEvents()
        {
            SboApp.Logger.Trace("MenuHelper.BindEvents");
            // Bind Events only once
            System.Windows.Forms.Application.ApplicationExit -= Application_ApplicationExit;
            System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;

            SboApp.Application.MenuEvent -= Application_MenuEvent;
            SboApp.Application.MenuEvent += Application_MenuEvent;
        }

        #region Events
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            foreach (var item in AddonMenuEvents)
            {
                // TODO: Remove menu items
            }
        }

        private static void Application_MenuEvent(ref MenuEvent pVal, out bool bubbleEvent)
        {
            bubbleEvent = true;

            if (pVal.BeforeAction)
                return;

            var menuId = pVal.MenuUID;
            var menuEvent = AddonMenuEvents.FirstOrDefault(e => e.MenuId == menuId);

            if (menuEvent == null)
                return;

            if (menuEvent.ThreadedAction)
            {
                var thread = new Thread(() => menuEvent.Action())
                { Name = menuEvent.MenuId, IsBackground = true };

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else
            {
                menuEvent.Action();
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
        /// <param name="threadedAction"></param>
        public static void AddMenuItemEvent(string title, string menuId, string parentMenuId, Action action, int position = -1, bool threadedAction = false)
        {
            AddonMenuEvents.Add(new AddonMenuEvent
            {
                MenuId = menuId,
                ParentMenuId = parentMenuId,
                Action = action,
                ThreadedAction = threadedAction,
                Position = position
            });

            AddItem(title, menuId, parentMenuId, position);

            BindEvents();
        }

        /// <summary>
        /// Get Menu Items Events from Form Controller Classes
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static void LoadAndAddMenuItemsFromFormControllers(Assembly assembly)
        {
            var formControllers = (from t in assembly.GetTypes()
                                   where t.IsClass && !t.IsAbstract
                                   && t.IsSubclassOf(typeof(FormController))
                                   && t.GetInterfaces().Contains(typeof(IFormMenuItem))
                                   select t).ToList();

            foreach (var formController in formControllers)
            {
                var formMenuItem = Activator.CreateInstance(formController) as IFormMenuItem;
                var item = new AddonMenuEvent
                {
                    MenuId = formMenuItem.MenuItemId,
                    ParentMenuId = formMenuItem.ParentMenuItemId,
                    Position = formMenuItem.MenuItemPosition,
                    Title = formMenuItem.MenuItemTitle,
                    Action = () =>
                    {
                        CreateOrStartController(formController);
                    },
                    ThreadedAction = false
                };
                AddMenuItemEvent(item.Title, item.MenuId, item.ParentMenuId, item.Action, item.Position);
            }

            BindEvents();
        }

        private static void CreateOrStartController(Type formControllerType)
        {
            // Clean up disposed Form Controllers
            FormControllerIntances.RemoveAll(i => i.Form == null);
            GC.Collect();

            var formController = FormControllerIntances.FirstOrDefault(i => i.GetType() == formControllerType && i.Unique);
            if (formController == null)
            {
                formController = (FormController) Activator.CreateInstance(formControllerType);
                FormControllerIntances.Add(formController);
            }
            formController.Start();
        }

        private static readonly List<FormController> FormControllerIntances = new List<FormController>();

        /// <summary>
        /// Load Menu from XML
        /// </summary>
        /// <param name="fileName"></param>
        public static void LoadFromXML(string fileName)
        {
            var oXmlDoc = new XmlDocument();
            oXmlDoc.Load(fileName);

            var node = oXmlDoc.SelectSingleNode("/Application/Menus/action/Menu");
            var imageAttr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.Name == "Image");
            /*
            if (imageAttr != null && !String.IsNullOrWhiteSpace(imageAttr.Value))
            {
                imageAttr.Value = String.Format(imageAttr.Value, Application.StartupPath + @"\img");
            }
            */

            var tmpStr = oXmlDoc.InnerXml;
            SboApp.Application.LoadBatchActions(ref tmpStr);
            var result = SboApp.Application.GetLastBatchResults();
        }

        /// <summary>
        /// Add Folder (Fluid API Style)
        /// </summary>
        /// <param name="parentMenuItem"></param>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static MenuItem AddFolder(this MenuItem parentMenuItem, string title, string itemId, int position = -1)
        {
            return parentMenuItem.Add(title, itemId, position, BoMenuType.mt_POPUP);
        }

        /// <summary>
        /// Add Menu Folder
        /// </summary>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="parentItemId"></param>
        /// <param name="position"></param>
        public static MenuItem AddFolder(string title, string itemId, string parentItemId, int position = -1)
        {
            var parentMenuItem = SboApp.Application.Menus.Item(parentItemId);
            return parentMenuItem.Add(title, itemId, position, BoMenuType.mt_POPUP);
        }

        /// <summary>
        /// Add Menu Item (Fluid API Style)
        /// </summary>
        /// <param name="parentMenuItem"></param>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="position"></param>
        public static MenuItem AddItem(this MenuItem parentMenuItem, string title, string itemId, int position = -1)
        {
            return parentMenuItem.Add(title, itemId, position, BoMenuType.mt_STRING);
        }

        /// <summary>
        /// Add Menu Item
        /// </summary>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="parentItemId"></param>
        /// <param name="position"></param>
        public static MenuItem AddItem(string title, string itemId, string parentItemId, int position = -1)
        {
            var parentMenuItem = SboApp.Application.Menus.Item(parentItemId);
            return parentMenuItem.Add(title, itemId, position, BoMenuType.mt_STRING);
        }

        /// <summary>
        /// Add Separator
        /// </summary>
        /// <param name="parentMenuItem"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static MenuItem AddSeparator(this MenuItem parentMenuItem, string itemId)
        {
            return parentMenuItem.Add("", itemId, -1, BoMenuType.mt_SEPERATOR);
        }

        private static MenuItem Add(this MenuItem parentMenuItem, string title, string itemId, int position, BoMenuType type)
        {
            try
            {
                Init();

                if (!parentMenuItem.SubMenus.Exists(itemId))
                {
                    var creationPackage = SboApp.Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams) as MenuCreationParams;

                    creationPackage.Type = type;
                    creationPackage.UniqueID = itemId;
                    creationPackage.String = title;
                    creationPackage.Image = "";
                    creationPackage.Enabled = true;
                    creationPackage.Position = position;

                    parentMenuItem.SubMenus.AddEx(creationPackage);
                }
            }
            catch (Exception e)
            {
                SboApp.Application.SetStatusBarMessage($"Error creating menu item (string) {itemId}: {e.Message}");
            }

            try
            {
                if (type == BoMenuType.mt_POPUP)
                    return parentMenuItem.SubMenus.Item(itemId);
                else
                    return parentMenuItem;
            }
            catch (Exception e)
            {
                throw new Exception($"Menu {itemId} not found in {parentMenuItem.UID}", e);
            }
        }
    }

    /// <summary>
    /// Menu Item Event
    /// </summary>
    public class AddonMenuEvent
    {
        /// <summary>
        /// Threaded Action
        /// </summary>
        public bool ThreadedAction { get; set; }

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
