
using ITCO.SboAddon.Framework.Services;
using System;

namespace ITCO.SboAddon.Framework.Queries
{
    public class HANAFrameworkQueries : IFrameworkQueries
    {
        public string DropProcedureIfExistsQuery(string DatabaseName, string ProcedureName)
        {
            return $"DROP PROCEDURE \"{DatabaseName}\".\"{ProcedureName}\"";
        }

        public string GetByDocNumQuery(int docNum)
        {
            return $"\"DocNum\"={docNum}";
        }

        public string GetChangedQuery(int timeStamp, int objectType)
        {
            return "SELECT DISTINCT \"U_ITCO_CT_Key\" AS \"Key\", CAST(\"Code\" AS int) AS \"Timestamp\" FROM \"@ITCO_CHANGETRACKER\" " +
                $"WHERE \"U_ITCO_CT_Obj\" = {objectType} AND CAST(\"Code\" AS int) > {timeStamp} " +
                "ORDER BY CAST(\"Code\" AS int) ASC";
        }

        public string GetDocEntryQuery(string table, int docNum)
        {
            return $"SELECT \"DocEntry\" FROM \"{table}\" WHERE \"DocNum\"={docNum}";
        }

        public string GetFieldIdQuery(string tableName, string fieldAlias)
        {
            return $"SELECT \"FieldID\" FROM CUFD WHERE \"TableID\"='{tableName}' AND \"AliasID\"='{fieldAlias}'";
        }

        public string GetOrCreateQueryCategoryQuery(string queryCategoryName)
        {
            return  $"SELECT * FROM \"OQCN\" WHERE \"CatName\" = '{queryCategoryName}'";
        }

        public string GetOrCreateUserQueryQuery(string userQueryName)
        {
            return $"SELECT \"IntrnalKey\" FROM \"OUQR\" WHERE \"QName\" = '{userQueryName}'";
        }

        public string GetSettingAsStringQuery(string key)
        {
            return $"SELECT \"U_{SettingService.UdfSettingValue}\", \"Name\" FROM \"@{SettingService.UdtSettings}\" WHERE \"Code\" = '{key}'";
        }

        public string GetSettingTitleQuery(string key)
        {
            return $"SELECT \"Name\" FROM \"@{SettingService.UdtSettings}\" WHERE \"Code\" = '{key}'";
        }

        public string ProcedureExistsQuery(string DatabaseName, string ProcedureName)
        {
            return $"SELECT \"PROCEDURE_NAME\" FROM \"SYS\".\"PROCEDURES\" WHERE \"PROCEDURE_NAME\" = '{ProcedureName}' AND \"SCHEMA_NAME\" = '{DatabaseName}'";
        }

        public string SaveSettingExistsQuery(string key)
        {
            return $"SELECT \"U_{SettingService.UdfSettingValue}\", \"Name\" FROM \"@{SettingService.UdtSettings}\" WHERE \"Code\" = '{key}'";
        }

        public string SaveSettingInsertQuery(string key, string name, string value)
        {
            return $"INSERT INTO \"@{SettingService.UdtSettings}\" (\"Code\", \"Name\", \"U_{SettingService.UdfSettingValue}\") VALUES ('{key}', '{name}', {value})";
        }

        public string SaveSettingUpdateQuery(string key, string value)
        {
            return $"UPDATE \"@{SettingService.UdtSettings}\" SET \"U_{SettingService.UdfSettingValue}\" = {value} WHERE \"Code\" = '{key}'";
        }

        public string SearchQuery(string table, string where)
        {
            return $"SELECT * FROM \"{table}\" WHERE {where}";
        }

        public string SetContactEmployeesLineByContactCodeQuery(string cardCode, int contactCode)
        {
            return "SELECT \"LineNum\" FROM (SELECT ROW_NUMBER() OVER (ORDER BY \"CntctCode\" ASC) - 1 AS \"LineNum\", \"CntctCode\" FROM \"OCPR\" " +
                $"WHERE \"CardCode\"='{cardCode}') AS \"T0\" WHERE \"CntctCode\"={contactCode}";
        }

        public string SetContactEmployeesLineByContactIdQuery(string cardCode, string contactId)
        {
            return "SELECT \"LineNum\" FROM (SELECT ROW_NUMBER() OVER (ORDER BY \"CntctCode\" ASC) - 1 AS \"LineNum\", \"Name\" FROM \"OCPR\" " +
                $"WHERE \"CardCode\"='{cardCode}') AS \"T0\" WHERE \"Name\"='{contactId}'";
        }

        public string WaitForOpenTransactionsQuery(string companyDB)
        {
            throw new NotImplementedException(); //Impossible in HANA it seems
        }
    }
}
