
namespace ITCO.SboAddon.Framework.Helpers
{
    using System.Linq;
    using SAPbobsCOM;
    using ITCO.SboAddon.Framework.Queries;

    /// <summary>
    /// Database Helper Functions
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Get SQL Version
        /// </summary>
        public static DatabaseVersion GetDataBaseVersion()
        { 
            switch (SboApp.Company.DbServerType)
            {
                case BoDataServerTypes.dst_HANADB:
                    return DatabaseVersion.HANA;
                case BoDataServerTypes.dst_MSSQL2005:
                    return DatabaseVersion.Mssql2005;
                case BoDataServerTypes.dst_MSSQL2008:
                    return DatabaseVersion.Mssql2008;
                case BoDataServerTypes.dst_MSSQL2012:
                    return DatabaseVersion.Mssql2012;
                case BoDataServerTypes.dst_MSSQL2014:
                    return DatabaseVersion.Mssql2014;
                default:
                    return DatabaseVersion.Mssql;
            }
        }
        /// <summary>
        /// Check If Procedure Exists
        /// </summary>
        public static bool ProcedureExists(string ProcedureName)
        {
            var DatabaseName = SboApp.Application.Company.DatabaseName;
            var count = SboRecordset.NonQuery(FrameworkQueries.Instance.ProcedureExistsQuery(DatabaseName, ProcedureName));
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drops Procedure If It Exists
        /// </summary>
        public static void DropProcedureIfExists(string ProcedureName)
        {
            var DatabaseName = SboApp.Application.Company.DatabaseName;
            if(ProcedureExists(ProcedureName))
            {
                SboRecordset.NonQuery(FrameworkQueries.Instance.DropProcedureIfExistsQuery(DatabaseName, ProcedureName));
            }
        }
    }
}
/// <summary>
/// Database version (Example: HANA, Mssql, Mssql2012).
/// </summary>
public enum DatabaseVersion
    {
        /// <summary>
        /// Hana Version
        /// </summary>
        HANA,
        /// <summary>
        /// Mssql Version
        /// </summary>
        Mssql,
        /// <summary>
        /// Mssql2005 Version
        /// </summary>
        Mssql2005,
        /// <summary>
        /// Mssql2008 Version
        /// </summary>
        Mssql2008,
        /// <summary>
        /// Mssql2012 Version
        /// </summary>
        Mssql2012,
        /// <summary>
        /// Mssql2014
        /// </summary>
        Mssql2014 
    }

   
