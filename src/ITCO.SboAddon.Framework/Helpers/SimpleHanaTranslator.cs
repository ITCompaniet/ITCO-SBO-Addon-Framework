namespace ITCO.SboAddon.Framework.Helpers
{
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
            var sqlChars = new char[] { '[', ']' };
            var hanaChars = new char[] { '"', '"' };

            for (int i = 0; i < sqlChars.Length; i++)
                hanaSql = hanaSql.Replace(sqlChars[i], hanaChars[i]);

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
