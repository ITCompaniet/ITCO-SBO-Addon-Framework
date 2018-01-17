namespace ITCO.SboAddon.Framework.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using global::Dapper;
    using ITCO.SboAddon.Framework.Helpers;
    using Sap.Data.Hana;

    /// <summary>
    /// Connection handler using Dapper
    /// </summary>
    public class SboDbConnection : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Data.SboDbConnection"/> class. 
        /// Creates new connection
        /// </summary>
        public SboDbConnection()
        {
            var connectionString = DatabaseHelper.GetConnectionString();

            switch (connectionString.ProviderName)
            {
                case "Sap.Data.Hana":
                    this.DbConnection = new HanaConnection(connectionString.ConnectionString);
                    break;

                case "System.Data.SqlClient":
                    this.DbConnection = new SqlConnection(connectionString.ConnectionString);
                    break;

                default:
                    throw new Exception($"Invalid connection ProviderName: {connectionString.ProviderName}");
            }

            this.DbConnection.Open();
        }

        /// <summary>
        /// Gets current DbConnection
        /// </summary>
        public DbConnection DbConnection { get; }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            return this.DbConnection.Query<T>(sql, param);
        }

        /// <summary>
        /// Executes a single-row connection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T QueryFirst<T>(string sql, object param = null)
        {
            return this.DbConnection.QueryFirst<T>(sql, param);
        }

        /// <summary>
        /// Dispose current connection
        /// </summary>
        public void Dispose()
        {
            this.DbConnection.Dispose();
        }
    }
}
