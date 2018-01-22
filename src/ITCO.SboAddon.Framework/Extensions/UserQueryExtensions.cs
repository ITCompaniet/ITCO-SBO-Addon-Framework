namespace ITCO.SboAddon.Framework.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Helpers;
    using ITCO.SboAddon.Framework.Queries;
    using SAPbobsCOM;
    
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
        /// Db parameter: SQL @p0, HANA :p0
        /// </summary>
        Database,

        /// <summary>
        /// String {0}
        /// </summary>
        String,
        
        [Obsolete("Use 'Database' instead")]
        Sql
    }

    /// <summary>
    /// SBO User Query Helper
    /// </summary>
    public static class UserQueryExtensions
    {
        private static readonly Dictionary<string, int> QueryCategoryCache = new Dictionary<string, int>();

        /// <summary>
        /// Get or create User Query
        /// </summary>
        /// <param name="company">Company Object</param>
        /// <param name="userQueryName">User Query Name</param>
        /// <param name="userQueryDefaultQuery">Default query</param>
        /// <param name="parameterFormat">Define parameter format [%0]/(@p0/:p0)/{0}</param>
        /// <param name="queryCategoryName">Add query in category with this name</param>
        /// <returns>SQL Query</returns>
        public static string GetOrCreateUserQuery(this Company company, string userQueryName, string userQueryDefaultQuery, ParameterFormat parameterFormat = ParameterFormat.Database, string queryCategoryName = null)
        {
            var userQuery = userQueryDefaultQuery;
            using (var userQueryObject = new SboRecordsetQuery<UserQueries>(FrameworkQueries.Instance.GetOrCreateUserQueryQuery(userQueryName), BoObjectTypes.oUserQueries))
            {
                var queryCategoryCode = GetOrCreateQueryCategory(queryCategoryName);
                if (userQueryObject.Count == 0)
                {
                    userQueryObject.BusinessObject.QueryDescription = userQueryName;
                    userQueryObject.BusinessObject.Query = userQueryDefaultQuery;
                    userQueryObject.BusinessObject.QueryCategory = queryCategoryCode;
                    var response = userQueryObject.BusinessObject.Add();

                    ErrorHelper.HandleErrorWithException(response, $"Could not create User Query '{userQueryName}'");
                }
                else
                {
                    var row = userQueryObject.Result.First();
                    userQuery = row.Query;
                    if (queryCategoryCode > 0 && userQueryObject.BusinessObject.QueryCategory != queryCategoryCode)
                    {
                        userQueryObject.BusinessObject.QueryCategory = queryCategoryCode;
                        var response = userQueryObject.BusinessObject.Update();
                        ErrorHelper.HandleErrorWithException(response, $"Could not update User Query '{userQueryName}' with Query Category {queryCategoryCode}");
                    }
                }
            }
            
            userQuery = ReturnParameterStyle(userQuery, parameterFormat);

            return userQuery;
        }

        /// <summary>
        /// Get query category code, create if not exists
        /// </summary>
        /// <param name="queryCategoryName">Query category name</param>
        /// <returns>Returns query category key</returns>
        public static int GetOrCreateQueryCategory(string queryCategoryName)
        {
            var queryCategoryCode = -1;
            if (string.IsNullOrEmpty(queryCategoryName))
            {
                return queryCategoryCode;
            }

            if (QueryCategoryCache.ContainsKey(queryCategoryName))
            {
                return QueryCategoryCache[queryCategoryName];
            }

            var sql = FrameworkQueries.Instance.GetOrCreateQueryCategoryQuery(queryCategoryName);

            using (var queryCategoryObject = new SboRecordsetQuery<QueryCategories>(sql, BoObjectTypes.oQueryCategories))
            {
                if (queryCategoryObject.Count == 1)
                {
                    queryCategoryCode = queryCategoryObject.BusinessObject.Code;
                }
                else
                {
                    queryCategoryObject.BusinessObject.Name = queryCategoryName;
                    var response = queryCategoryObject.BusinessObject.Add();
                    ErrorHelper.HandleErrorWithException(response, $"Could not create Query Category '{queryCategoryName}'");
                    queryCategoryCode = int.Parse(SboApp.Company.GetNewObjectKey());
                }
            }
 
            QueryCategoryCache.Add(queryCategoryName, queryCategoryCode);
            return queryCategoryCode;
        }

        /// <summary>
        /// Replace parameters with requested parameter format
        /// </summary>
        /// <param name="userQuery">SQL Query with SAP parameters</param>
        /// <param name="parameterFormat">Parameter format</param>
        /// <returns>SQL Query with requested parameter format</returns>
        public static string ReturnParameterStyle(string userQuery, ParameterFormat parameterFormat)
        {
            switch (parameterFormat)
            {
                case ParameterFormat.Database:
                case ParameterFormat.Sql:
                    if (SboApp.Company.DbServerType == BoDataServerTypes.dst_HANADB)
                    {
                        userQuery = Regex.Replace(userQuery, @"'?\[%([0-9])\]'?", ":p$1");
                    }
                    else
                    {
                        userQuery = Regex.Replace(userQuery, @"'?\[%([0-9])\]'?", "@p$1");
                    }

                    break;
                case ParameterFormat.String:
                    userQuery = Regex.Replace(userQuery, @"'\[%([0-9])\]'", "'{$1}'");
                    userQuery = Regex.Replace(userQuery, @"\[%([0-9])\]", "{$1}");
                    break;
            }

            return userQuery;
        }
    }
}
