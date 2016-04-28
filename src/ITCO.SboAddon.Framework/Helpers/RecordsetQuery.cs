using SAPbobsCOM;
using System;
using System.Collections.Generic;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class SboRecordsetQuery : IDisposable
    {
        private Recordset _recordSetObject;

        public SboRecordsetQuery(string sql, params object[] args)
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

            _recordSetObject.DoQuery(string.Format(sql, args));
        }

        public int Count => _recordSetObject.RecordCount;

        /// <summary>
        /// Note that Fields is COM object reference
        /// so you can only access data in loop!
        /// </summary>
        public IEnumerable<Fields> Result
        {
            get
            {
                while (!_recordSetObject.EoF)
                {
                    yield return _recordSetObject.Fields;
                    _recordSetObject.MoveNext();
                }
            }
        }

        public void Dispose()
        {
            if (_recordSetObject != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_recordSetObject);

            _recordSetObject = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">SBO Business Object</typeparam>
    public class SboRecordsetQuery<T> : IDisposable
    {
        private Recordset _recordSetObject;
        private dynamic _businessObject;

        public SboRecordsetQuery(string sql, BoObjectTypes boObjectTypes, params object[] args)
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            _businessObject = SboApp.Company.GetBusinessObject(boObjectTypes);

            _recordSetObject.DoQuery(string.Format(sql, args));
            _businessObject.Browser.Recordset = _recordSetObject;
        }

        public int Count => _recordSetObject.RecordCount;

        /// <summary>
        /// Note that T is COM object reference
        /// so you can only access data in loop!
        /// </summary>
        public IEnumerable<T> Result
        {
            get
            {
                while (!_businessObject.Browser.EoF)
                {
                    yield return _businessObject;
                    _businessObject.Browser.MoveNext();
                }
            }
        }

        public void Dispose()
        {
            if (_recordSetObject != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_recordSetObject);

            if (_businessObject != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_businessObject);

            _recordSetObject = null;
            _businessObject = null;
        }
    }


    public static class SboRecordset
    {
        /// <summary>
        /// Executes SQL without result set
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="args">SQL Argument</param>
        /// <returns>Affected rows</returns>
        public static int NonQuery(string sql, params object[] args)
        {
            var recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            recordSetObject.DoQuery(string.Format(sql, args));
            var recordCount = recordSetObject.RecordCount;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSetObject);
            recordSetObject = null;

            return recordCount;
        }
    }
}
