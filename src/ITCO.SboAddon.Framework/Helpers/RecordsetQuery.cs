using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class SboRecordsetQuery : IDisposable
    {
        private Recordset _recordSetObject;

        public SboRecordsetQuery()
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
        }

        public SboRecordsetQuery(string sql, params object[] args)
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

            _recordSetObject.DoQuery(string.Format(sql, args));
        }

        public void DoQuery(string sql, params object[] args)
        {
            try
            {
                _recordSetObject.DoQuery(string.Format(sql, args));
                if (_recordSetObject.EoF)
                    _recordSetObject.MoveFirst();
            }
            catch (Exception e)
            {
            }
        }

        public int Count => _recordSetObject.RecordCount;

        public Recordset Recordset => _recordSetObject;

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
            GC.Collect();
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

        public SboRecordsetQuery(BoObjectTypes boObjectTypes)
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            _businessObject = SboApp.Company.GetBusinessObject(boObjectTypes);
        }

        public SboRecordsetQuery(string sql, BoObjectTypes boObjectTypes, params object[] args)
        {
            _recordSetObject = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            _businessObject = SboApp.Company.GetBusinessObject(boObjectTypes);

            _recordSetObject.DoQuery(string.Format(sql, args));
            _businessObject.Browser.Recordset = _recordSetObject;
        }

        public int Count => _recordSetObject.RecordCount;

        public T BusinessObject => _businessObject;
        public Recordset Recordset => _recordSetObject;

        public void DoQuery(string sql, params object[] args)
        {
            try
            {
                _recordSetObject.DoQuery(string.Format(sql, args));
                if (_recordSetObject.EoF)
                    _recordSetObject.MoveFirst();

                if (_recordSetObject.RecordCount > 0)
                    _businessObject.Browser.Recordset = _recordSetObject;
            }
            catch (Exception e)
            {
                
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
            try
            {
                if (recordSetObject == null)
                    throw new Exception("Failed to get Recordset Object");

                recordSetObject.DoQuery(string.Format(sql, args));
                return recordSetObject.RecordCount;
            }
            catch (Exception e)
            {
                SboApp.Logger.Debug($"NonQuery Error: {e.Message}, SQL={sql}");
                throw;
            }
            finally
            {
                if (recordSetObject != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(recordSetObject);
                }
                recordSetObject = null;
                GC.Collect();
            }
        }
    }

    /// <summary>
    /// System SQL Connection for faster queries
    /// </summary>
    public class SboSqlConnection : IDisposable
    {
        private readonly SqlConnection _sqlConnection;
        private readonly SqlDataReader _reader;

        public SboSqlConnection(string query = null)
        {
            var dbPassword = ConfigurationManager.AppSettings["Sbo:DbPassword"];
            var connectionString = $"Server={SboApp.Company.Server};Initial Catalog={SboApp.Company.CompanyDB};User ID={SboApp.Company.DbUserName};Password={dbPassword}";
            _sqlConnection = new SqlConnection(connectionString);

            if (query == null) return;

            var command = new SqlCommand(query, _sqlConnection);
            _sqlConnection.Open();
            _reader = command.ExecuteReader();
        }

        public SqlConnection SqlConnection => _sqlConnection;
        public bool HasRows => _reader.HasRows;

        /// <summary>
        /// Result
        /// </summary>
        public IEnumerable<SqlDataReader> Result
        {
            get
            {
                while (_reader.Read())
                    yield return _reader;

                _reader.Close();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() => _sqlConnection.Close();
    }
}
