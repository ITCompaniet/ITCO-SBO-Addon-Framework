using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class SboRecordsetQuery : IDisposable
    {
        public SboRecordsetQuery()
        {
            Recordset = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
        }

        public SboRecordsetQuery(string sql, params object[] args)
        {
            Recordset = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

            SboApp.Logger.Debug($"SQL: {sql}, Args = {string.Join(",", args)}");
            Recordset.DoQuery(string.Format(sql, args));
        }

        /// <summary>
        /// Run query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void DoQuery(string sql, params object[] args)
        {

            try
            {
                SboApp.Logger.Debug($"SQL: {sql}, Args = {string.Join(",", args)}");

                Recordset.DoQuery(string.Format(sql, args));
                if (Recordset.EoF)
                    Recordset.MoveFirst();
            }
            catch (Exception e)
            {
                SboApp.Logger.Error($"DoQuery Error {e.Message}", e);
                throw;
            }
        }

        /// <summary>
        /// Record cound
        /// </summary>
        public int Count => Recordset.RecordCount;

        /// <summary>
        /// Recordset object
        /// </summary>
        public Recordset Recordset { get; private set; }

        /// <summary>
        /// Note that Fields is COM object reference
        /// so you can only access data in loop!
        /// </summary>
        public IEnumerable<Fields> Result
        {
            get
            {
                while (!Recordset.EoF)
                {
                    yield return Recordset.Fields;
                    Recordset.MoveNext();
                }
            }
        }

        public void Dispose()
        {
            if (Recordset != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Recordset);

            Recordset = null;
            GC.Collect();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">SBO Business Object</typeparam>
    public class SboRecordsetQuery<T> : IDisposable
    {
        private dynamic _businessObject;

        public SboRecordsetQuery(BoObjectTypes boObjectTypes)
        {
            Recordset = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            _businessObject = SboApp.Company.GetBusinessObject(boObjectTypes);
        }

        public SboRecordsetQuery(string sql, BoObjectTypes boObjectTypes, params object[] args)
        {
            Recordset = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            _businessObject = SboApp.Company.GetBusinessObject(boObjectTypes);
            SboApp.Logger.Debug($"SQL: {sql}, Args = {string.Join(",", args)}");
            Recordset.DoQuery(string.Format(sql, args));
            _businessObject.Browser.Recordset = Recordset;
        }

        public int Count => Recordset.RecordCount;

        public T BusinessObject => _businessObject;
        public Recordset Recordset { get; private set; }

        public void DoQuery(string sql, params object[] args)
        {
            try
            {
                SboApp.Logger.Debug($"SQL: {sql}, Args = {string.Join(",", args)}");
                Recordset.DoQuery(string.Format(sql, args));
                if (Recordset.EoF)
                    Recordset.MoveFirst();

                if (Recordset.RecordCount > 0)
                    _businessObject.Browser.Recordset = Recordset;
            }
            catch (Exception e)
            {
                SboApp.Logger.Debug($"Unexpected errorin DoQuery", e);
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
            if (Recordset != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Recordset);

            if (_businessObject != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_businessObject);

            Recordset = null;
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

                SboApp.Logger.Debug($"SQL: {sql}, Args = {string.Join(",", args)}");
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
    [Obsolete("Use SboDbConnection instead")]
    public class SboSqlConnection : IDisposable
    {
        private readonly SqlDataReader _reader;

        public SboSqlConnection(string query = null)
        {
            var dbPassword = ConfigurationManager.AppSettings["Sbo:DbPassword"];
            var connectionString = $"Server={SboApp.Company.Server};Initial Catalog={SboApp.Company.CompanyDB};User ID={SboApp.Company.DbUserName};Password={dbPassword}";
            SqlConnection = new SqlConnection(connectionString);

            if (query == null) return;

            var command = new SqlCommand(query, SqlConnection);
            SqlConnection.Open();
            _reader = command.ExecuteReader();
        }

        public SqlConnection SqlConnection { get; }
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
        public void Dispose() => SqlConnection.Close();
    }
}
