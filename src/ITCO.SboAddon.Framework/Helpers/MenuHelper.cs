using System.Linq;
using System.Xml;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class MenuHelper
    {
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
    }
}
