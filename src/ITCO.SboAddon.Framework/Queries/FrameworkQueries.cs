
using SAPbobsCOM;
using System;

namespace ITCO.SboAddon.Framework.Queries
{
    public class FrameworkQueries
    {
        private static FrameworkQueries _queries = null;
        private static IFrameworkQueries _instance;
        private FrameworkQueries()
        {
            switch (SboApp.Company.DbServerType)
            {
                //HANA
                case BoDataServerTypes.dst_HANADB:
                    _instance = new HANAFrameworkQueries();
                    break;

                //SQL
                case BoDataServerTypes.dst_MSSQL:
                case BoDataServerTypes.dst_MSSQL2005:
                case BoDataServerTypes.dst_MSSQL2008:
                case BoDataServerTypes.dst_MSSQL2012:
                case BoDataServerTypes.dst_MSSQL2014:
                case BoDataServerTypes.dst_MSSQL2016:
                    _instance = new SQLFrameworkQueries();
                    break;
                default:
                    throw new Exception($"Database type '{SboApp.Company.DbServerType}' is currently not supported");
            }
        }
        public static IFrameworkQueries Instance
        {
            get
            {
                if (_queries == null)
                    _queries = new FrameworkQueries();

                return _instance;
            }
        }

    }

    public interface IFrameworkQueries
    {
        string DropProcedureIfExistsQuery(string DatabaseName, string ProcedureName);
        string GetByDocNumQuery(int docNum);
        string GetChangedQuery(int timeStamp, int objectType);
        string GetDocEntryQuery(string table, int docNum);
        string GetFieldIdQuery(string tableName, string fieldAlias);
        string GetOrCreateQueryCategoryQuery(string queryCategoryName);
        string GetOrCreateUserQueryQuery(string userQueryName);
        string GetSettingAsStringQuery(string key);
        string GetSettingTitleQuery(string key);
        
        string ProcedureExistsQuery(string DatabaseName, string ProcedureName);
        string SaveSettingExistsQuery(string key);
        string SaveSettingUpdateQuery(string key, string value);
        string SaveSettingInsertQuery(string key, string name, string value);
        string SearchQuery(string table, string where);
        string SetContactEmployeesLineByContactCodeQuery(string cardCode, int contactCode);
        string SetContactEmployeesLineByContactIdQuery(string cardCode, string contactId);
        string WaitForOpenTransactionsQuery(string companyDB);
    }
}
