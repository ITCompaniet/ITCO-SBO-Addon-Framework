using System;
using System.Xml;
using SAPbobsCOM;
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

        /// <summary>
        /// Get Disposable BusinessObject for better COM-object dispose handling
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="company"></param>
        /// <param name="boObjectTypes"></param>
        /// <returns></returns>
        /// <example>
        /// using (var sboIncomingPaymentBo = SboApp.Company.GetBusinessObject<Payments>(BoObjectTypes.oIncomingPayments))
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
        private readonly T _businessObject;

        public BusinessObject(SAPbobsCOM.Company company, BoObjectTypes boObjectTypes)
        {
            _businessObject = (T)company.GetBusinessObject(boObjectTypes);
        }

        public T Object => _businessObject;

        public void Dispose()
        {
            if (_businessObject != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_businessObject);
        }
    }
}
