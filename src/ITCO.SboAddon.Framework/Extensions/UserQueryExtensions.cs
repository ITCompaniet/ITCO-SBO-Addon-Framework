
using System.Linq;
using ITCO.SboAddon.Framework.Helpers;
using SAPbobsCOM;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class UserQueryExtensions
    {
        public static string GetOrCreateUserQuery(this Company company, string userQueryName, string userQueryDefaultQuery)
        {
            using (var userQuery = new SboRecordsetQuery<UserQueries>(
                $"SELECT [IntrnalKey] FROM [OUQR] WHERE [QName] = '{userQueryName}'", BoObjectTypes.oUserQueries))
            {
                if (userQuery.Count == 0)
                {
                    userQuery.BusinessObject.QueryDescription = userQueryName;
                    userQuery.BusinessObject.Query = userQueryDefaultQuery;
                    userQuery.BusinessObject.QueryCategory = -1;
                    var response = userQuery.BusinessObject.Add();

                    ErrorHelper.HandleErrorWithException(response, $"Could not create User Query '{userQueryName}'");

                    return userQueryDefaultQuery;
                }

                return userQuery.Result.First().Query;
            }
        }
    }
}
