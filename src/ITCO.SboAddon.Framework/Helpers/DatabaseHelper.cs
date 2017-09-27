
namespace ITCO.SboAddon.Framework.Helpers
{
    using System.Linq;

    /// <summary>
    /// Database Helper Functions
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Get MS SQL Version
        /// </summary>
        public static DatabaseVersion GetSQLDataBaseVersion()
        {
            using (var query = new SboRecordsetQuery("SELECT SERVERPROPERTY('productversion')"))
            {
                var fullVersion = query.Result.First().Item(0).Value.ToString();
                var version = fullVersion.Split('.')[0];

                switch (version)
                {
                    case "9":
                        return DatabaseVersion.Mssql2005;
                    case "10":
                        return DatabaseVersion.Mssql2008;
                    case "11":
                        return DatabaseVersion.Mssql2012;
                    case "12":
                        return DatabaseVersion.Mssql2014;
                    default:
                        return DatabaseVersion.Mssql;
                }
            }
        }

        /// <summary>
        /// Check If Procedure Exists
        /// </summary>
        public static bool ProcedureExists(string ProcedureName)
        {
            var DatabaseName = SboApp.Application.Company.DatabaseName;

            var count = 0;
            if (SboApp.IsHana)
                count = SboRecordset.NonQuery($"SELECT \"PROCEDURE_NAME\" FROM \"SYS\".\"PROCEDURES\" WHERE \"PROCEDURE_NAME\" = '{ProcedureName}' AND \"SCHEMA_NAME\" = '{DatabaseName}'");
            else
                count = SboRecordset.NonQuery($"SELECT id FROM [{DatabaseName}].[dbo].[sysobjects] WHERE id = OBJECT_ID(N'[{DatabaseName}].[dbo].[{ProcedureName}]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1");

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
            var DataBaseName = SboApp.Application.Company.DatabaseName;
            if (ProcedureExists(ProcedureName))
            {
                if (SboApp.IsHana)
                    SboRecordset.NonQuery($"DROP PROCEDURE \"{DataBaseName}\".\"{ProcedureName}\"");
                else
                    SboRecordset.NonQuery($"DROP PROCEDURE [dbo].[{ProcedureName}]");
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

   
