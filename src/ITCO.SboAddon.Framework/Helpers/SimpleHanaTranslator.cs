namespace ITCO.SboAddon.Framework.Helpers
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Simple HANA SQL-SCript translator
    /// </summary>
    public class SimpleHanaTranslator
    {
        /// <summary>
        /// Return query for currect database type
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="hanaSql"></param>
        /// <returns></returns>
        public static string GetDbQuery(string sql, string hanaSql = null)
        {
            if (SboApp.IsHana)
                return hanaSql ?? ConvertSqlToHana(sql);

            return sql;
        }

        /// <summary>
        /// Convert T-SQL to HANA SQL-Script
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string ConvertSqlToHana(string sql)
        {
            var hanaSql = sql;
            var sqlToHanaWords = new Dictionary<string, string>
                                     {
                                         { "[", "\"" },
                                         { "]", "\"" },
                                         { "ISNULL", "IFNULL" },
                                         { "GETDATE", "NOW" },
                                         { "AS BIT", "AS BOOLEAN" },

                                         // Try replace + with || only when strings are involved
                                         { "'+", "'||" },
                                         { "+'", "||'" },
                                         { "' +", "' ||" },
                                         { "+ '", "|| '" }
                                     };

            foreach (var sqlToHanaWord in sqlToHanaWords)
            {
                hanaSql = Regex.Replace(hanaSql, sqlToHanaWord.Key, sqlToHanaWord.Value, RegexOptions.IgnoreCase);
            }

            SboApp.Logger.Debug($"ConvertSqlToHana: '{sql}' -> '{hanaSql}'");

            return hanaSql;
        }
    }
    
    /// <summary>
    /// Object for both SQL and HANA Query
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Create Query Object for both SQL and HANA Query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="hanaSql"></param>
        public Query(string sql, string hanaSql = null)
        {
            if (SboApp.IsHana)
                _sql = hanaSql ?? SimpleHanaTranslator.ConvertSqlToHana(sql);
            else
                _sql = sql;
        }

        private readonly string _sql;

        public override string ToString()
        {
            return _sql;
        }
    }
}
