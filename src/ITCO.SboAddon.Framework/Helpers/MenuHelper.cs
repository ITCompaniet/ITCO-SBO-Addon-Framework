using ITCO.SboAddon.Framework.Forms;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// Helpers for menu handling
    /// </summary>
    public static class MenuHelper
    {
        /// <summary>
        /// Get Menu Items Events from Form Controller Classes
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ICollection<AddonMenuEvent> LoadMenuItemsFromFormControllers(Assembly assembly)
        {
            var addonMenuEvents = new List<AddonMenuEvent>();

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
                        ((FormController)Activator.CreateInstance(formController)).Start();
                    }
                };
                addonMenuEvents.Add(item);
            }

            return addonMenuEvents;
        }

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
                SboApp.Application.SetStatusBarMessage(string.Format("Error creating menu item (string) {0}: {1}", itemId, e.Message));
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
                throw new Exception(string.Format("Menu {0} not found in {1}", itemId, parentMenuItem.UID), e);
            }
        }
    }
}
