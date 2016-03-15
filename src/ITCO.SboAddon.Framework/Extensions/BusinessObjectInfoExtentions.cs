using System.Xml;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class BusinessObjectInfoExtentions
    {
        public static int GetDocEntry(this BusinessObjectInfo businessObjectInfo)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(businessObjectInfo.ObjectKey);
            return int.Parse(xmlDoc.SelectSingleNode("/DocumentParams/DocEntry").InnerText);
        }
    }
}
