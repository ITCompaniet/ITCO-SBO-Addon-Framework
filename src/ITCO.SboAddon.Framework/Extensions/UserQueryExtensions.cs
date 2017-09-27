namespace ITCO.SboAddon.Framework.Extensions
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Helpers;
    using SAPbobsCOM;

    /// <summary>
    /// SBO User Query Helper
    /// </summary>
    public static class UserQueryExtensions
    {
        /// <summary>
        /// Get or create User Query
        /// </summary>
        /// <param name="company">Company Object</param>
        /// <param name="userQueryName">User Query Name</param>
        /// <param name="userQueryDefaultQuery">Query</param>
        /// <param name="parameterFormat">Define parameter format [%0]/@p0/{0}</param>
        /// <returns>SQL</returns>
        public static string GetOrCreateUserQuery(this Company company, string userQueryName, string userQueryDefaultQuery, ParameterFormat parameterFormat = ParameterFormat.Sbo)
        {
            var userQuery = userQueryDefaultQuery;

            var sql = string.Empty;
            if (SboApp.IsHana)
                sql = $"SELECT \"IntrnalKey\" FROM \"OUQR\" WHERE \"QName\" = '{userQueryName}'";
            else
                sql = $"SELECT [IntrnalKey] FROM [OUQR] WHERE [QName] = '{userQueryName}'";

            using (var userQueryObject = new SboRecordsetQuery<UserQueries>(sql, BoObjectTypes.oUserQueries))
            {
                if (userQueryObject.Count == 0)
                {
                    userQueryObject.BusinessObject.QueryDescription = userQueryName;
                    userQueryObject.BusinessObject.Query = userQueryDefaultQuery;
                    userQueryObject.BusinessObject.QueryCategory = -1;
                    var response = userQueryObject.BusinessObject.Add();

                    ErrorHelper.HandleErrorWithException(response, $"Could not create User Query '{userQueryName}'");
                }
                else
                {
                    userQuery = userQueryObject.Result.First().Query;
                }
            }
            
            userQuery = ReturnParameterStyle(userQuery, parameterFormat);

            return userQuery;
        }

        /// <summary>
        /// Replace parameters with requested parameterformat
        /// </summary>
        /// <param name="userQuery">SQL Query with SAP parmeters</param>
        /// <param name="parameterFormat"></param>
        /// <returns>SQL Query with requsted parameter format</returns>
        public static string ReturnParameterStyle(string userQuery, ParameterFormat parameterFormat)
        {
            switch (parameterFormat)
            {
                case ParameterFormat.Sql:
                    userQuery = Regex.Replace(userQuery, @"'?\[%([0-9])\]'?", "@p$1");
                    break;
                case ParameterFormat.String:
                    userQuery = Regex.Replace(userQuery, @"'\[%([0-9])\]'", "'{$1}'");
                    userQuery = Regex.Replace(userQuery, @"\[%([0-9])\]", "{$1}");
                    break;
            }
            return userQuery;
        }
    }

    /// <summary>
    /// Query Parameter Format
    /// </summary>
    public enum ParameterFormat
    {
        /// <summary>
        /// SBO [%0]
        /// </summary>
        Sbo,

        /// <summary>
        /// SQL @p0
        /// </summary>
        Sql,

        /// <summary>
        /// String {0}
        /// </summary>
        String
    }
}
