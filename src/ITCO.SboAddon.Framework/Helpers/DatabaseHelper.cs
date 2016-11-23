
namespace ITCO.SboAddon.Framework.Helpers
{
    using System.Linq;

    public static class DatabaseHelper
    {
        /// <summary>
        /// Get SQL Version
        /// </summary>
        public static SqlVersion GetSqlVersion()
        {
            using (var query = new SboRecordsetQuery("SELECT SERVERPROPERTY('productversion')"))
            {
                var fullVersion = query.Result.First().Item(0).Value.ToString();
                var version = fullVersion.Split('.')[0];

                switch (version)
                {
                    case "9":
                        return SqlVersion.Mssql2005;
                    case "10":
                        return SqlVersion.Mssql2008;
                    case "11":
                        return SqlVersion.Mssql2012;
                    case "12":
                        return SqlVersion.Mssql2014;
                    default:
                        return SqlVersion.Mssql;
                }
            }
        }
    }

    public enum SqlVersion
    {
        Mssql,
        Mssql2005,
        Mssql2008,
        Mssql2012,
        Mssql2014
    }
}
