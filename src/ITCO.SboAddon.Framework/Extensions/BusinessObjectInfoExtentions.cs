using System;
using System.Xml;
using SAPbobsCOM;
using SAPbouiCOM;
using ITCO.SboAddon.Framework.Queries;

namespace ITCO.SboAddon.Framework.Extensions
{
    /// <summary>
    /// Extension functions for BusinessObjects
    /// </summary>
    public static class BusinessObjectInfoExtentions
    {
        /// <summary>
        /// Get DocEntry from BusinessObjectInfo
        /// </summary>
        /// <param name="businessObjectInfo"></param>
        /// <returns>DocEntry</returns>
        public static int GetDocEntry(this BusinessObjectInfo businessObjectInfo)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(businessObjectInfo.ObjectKey);
            return int.Parse(xmlDoc.SelectSingleNode("/DocumentParams/DocEntry | AbsoluteEntry").InnerText); 
        }

        /// <summary>
        /// Get Document By DocNum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="businessObject"></param>
        /// <param name="docNum">DocNum</param>
        /// <returns></returns>
        public static bool GetByDocNum<T>(this BusinessObject<T> businessObject, int docNum) where T : Documents
        {
            return businessObject.Object.Search(businessObject.BoObjectType.GetTableName(), FrameworkQueries.Instance.GetByDocNumQuery(docNum));
        }

        /// <summary>
        /// Get Disposable BusinessObject for better COM-object dispose handling
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="company"></param>
        /// <param name="boObjectTypes"></param>
        /// <returns></returns>
        /// <example>
        /// using (var sboIncomingPaymentBo = SboApp.Company.GetBusinessObject&lt;Payments&gt;(BoObjectTypes.oIncomingPayments))
        /// {
        ///     // Code here
        /// }
        /// </example>
        public static BusinessObject<T> GetBusinessObject<T>(this SAPbobsCOM.Company company, BoObjectTypes boObjectTypes)
        {
            return new BusinessObject<T>(company, boObjectTypes);
        }
    }

    /// <summary>
    /// IDisposable BusinessObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusinessObject<T> : IDisposable
    {
        /// <summary>
        /// Business object type
        /// </summary>
        public readonly BoObjectTypes BoObjectType;
        /// <summary>
        /// Business object
        /// </summary>
        public readonly T Object;
        /// <summary>
        /// BusinessObject
        /// </summary>
        /// <param name="company"></param>
        /// <param name="boObjectType"></param>
        public BusinessObject(SAPbobsCOM.Company company, BoObjectTypes boObjectType)
        {
            BoObjectType = boObjectType;
            Object = (T)company.GetBusinessObject(BoObjectType);
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (Object != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Object);
        }
    }
}
