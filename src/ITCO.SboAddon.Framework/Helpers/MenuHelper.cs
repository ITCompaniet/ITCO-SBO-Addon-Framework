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
    public class MenuHelper
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
        /// Add Menu Folder
        /// </summary>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="parentItemId"></param>
        /// <param name="position"></param>
        public static void AddFolder(string title, string itemId, string parentItemId, int position = -1)
        {
            try
            {
                var parentMenuItem = SboApp.Application.Menus.Item(parentItemId);

                if (!parentMenuItem.SubMenus.Exists(itemId))
                {
                    var creationPackage = SboApp.Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams) as MenuCreationParams;

                    creationPackage.Type = BoMenuType.mt_POPUP;
                    creationPackage.UniqueID = itemId;
                    creationPackage.String = title;
                    creationPackage.Enabled = true;
                    creationPackage.Position = position;

                    parentMenuItem.SubMenus.AddEx(creationPackage);
                }
            }
            catch (Exception e)
            {
                SboApp.Application.SetStatusBarMessage(string.Format("Error creating menu item (folder) {0}: {1}", itemId, e.Message));
            }
        }

        /// <summary>
        /// Add Menu Item
        /// </summary>
        /// <param name="title"></param>
        /// <param name="itemId"></param>
        /// <param name="parentItemId"></param>
        /// <param name="position"></param>
        public static void AddItem(string title, string itemId, string parentItemId, int position = -1)
        {
            try
            {
                var parentMenuItem = SboApp.Application.Menus.Item(parentItemId);

                if (!parentMenuItem.SubMenus.Exists(itemId))
                {
                    var creationPackage = SboApp.Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams) as MenuCreationParams;

                    creationPackage.Type = BoMenuType.mt_STRING;
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
        }

    }
}
