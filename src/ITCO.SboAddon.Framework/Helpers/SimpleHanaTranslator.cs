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
        /// <param name="sql">T-SQL Query</param>
        /// <returns></returns>
        public static string ConvertSqlToHana(string sql)
        {
            var hanaSql = sql;

            var sqlToHanaWords = new Dictionary<string, string>
                                     {
                                         { @"\(SELECT 'X' AS \[DUMMY\]\) \[T\]", @"""DUMMY""" },
                                         { @"\(SELECT 'X' AS 'DUMMY'\) \[T\]", @"""DUMMY""" },
                                         { @"\[", @"""" },
                                         { @"\]", @"""" },

                                         // Replace variables
                                         { @"@", @":" },
                                         
                                         // Revert back possible table names starting with @
                                         { @""":", @"""@" },

                                         { "ISNULL", "IFNULL" },
                                         { "NEWID", "NEWUID"},
                                         { "GETDATE", "NOW" },
                                         { "AS BIT", "AS BOOLEAN" },
                                         { @"DATEDIFF\(SECOND,", @"SECONDS_BETWEEN(" },
                                         { @"DATEDIFF\(DAY,", @"DAYS_BETWEEN(" },

                                         // Try convert back UQ parameter format
                                         { "\"%([0-9])\"", "[%$1]" },

                                         // Try replace + with || only when strings are involved
                                         { @"'\+", "'||" },
                                         { @"\+'", "||'" },
                                         { @"' \+", "' ||" },
                                         { @"\+ '", "|| '" },

                                         // Try replace ' with " ONLY when AS is involved!
                                         { @"AS '(\w*)'", @"AS ""$1"""}

                                     };

            foreach (var sqlToHanaWord in sqlToHanaWords)
            {
                hanaSql = Regex.Replace(hanaSql, sqlToHanaWord.Key, sqlToHanaWord.Value, RegexOptions.IgnoreCase);
            }

            const string TopPattern = @" TOP\(([0-9]+)\)";
            var topMatch = Regex.Match(hanaSql, TopPattern);
            if (topMatch.Success && topMatch.Groups.Count > 1)
            {
                hanaSql = Regex.Replace(hanaSql, TopPattern, string.Empty);
                hanaSql += $" LIMIT {topMatch.Groups[1].Value}";
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

        public static implicit operator string(Query q) => q.ToString();

        private readonly string _sql;

        public override string ToString()
        {
            return _sql;
        }
    }
}
