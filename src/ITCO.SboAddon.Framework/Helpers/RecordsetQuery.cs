using SAPbobsCOM;
using System;
using System.Collections.Generic;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class RecordsetQuery : IDisposable
    {
        private Recordset _recordSetObject;

        public RecordsetQuery(string sql)
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

            _recordSetObject.DoQuery(sql);
        }

        public int Count
        {
            get
            {
                return _recordSetObject.RecordCount;
            }
        }

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


    public class RecordsetQuery<T> : IDisposable
    {
        private Recordset _recordSetObject;
        private dynamic _businessObject;

        public RecordsetQuery(string sql, BoObjectTypes boObjectTypes)
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            _businessObject = SboApp.Company.GetBusinessObject(boObjectTypes);

            _recordSetObject.DoQuery(sql);
            _businessObject.Browser.Recordset = _recordSetObject;
        }

        public int Count
        {
            get
            {
                return _recordSetObject.RecordCount;
            }
        }

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

            _recordSetObject = null;
        }
    }
}
