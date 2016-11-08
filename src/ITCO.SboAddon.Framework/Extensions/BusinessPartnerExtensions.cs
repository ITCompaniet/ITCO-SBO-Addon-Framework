namespace ITCO.SboAddon.Framework.Extensions
{
    using System.Linq;
    using Helpers;
    using SAPbobsCOM;

    /// <summary>
    /// Business Partner Extensions
    /// </summary>
    public static class BusinessPartnerExtensions
    {
        /// <summary>
        /// SetCurrentLine by CntctCode
        /// </summary>
        /// <param name="businessPartners"></param>
        /// <param name="contactCode"></param>
        /// <returns></returns>
        public static bool SetContactEmployeesLineByContactCode(this BusinessPartners businessPartners, int contactCode)
        {
            var cardCode = businessPartners.CardCode;
            var contactEmployees = businessPartners.ContactEmployees;

            using (var query = new SboRecordsetQuery(
                "SELECT [LineNum] FROM (SELECT ROW_NUMBER() OVER (ORDER BY [CntctCode] ASC) - 1 AS [LineNum], [CntctCode] FROM [OCPR] " +
                $"WHERE [CardCode]='{cardCode}') AS [T0] WHERE [CntctCode]={contactCode}"))
            {
                if (query.Count == 0) return false;

                var lineNum = (int) query.Result.First().Item(0).Value;
                SboApp.Logger.Debug($"CntctCode {contactCode} is LineNum {lineNum} for CardCode {cardCode}");

                contactEmployees.SetCurrentLine(lineNum);
                return true;
            }
        }

        /// <summary>
        /// SetCurrentLine by "Contact ID"
        /// </summary>
        /// <param name="businessPartners"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static bool SetContactEmployeesLineByContactId(this BusinessPartners businessPartners, string contactId)
        {
            var cardCode = businessPartners.CardCode;
            var contactEmployees = businessPartners.ContactEmployees;

            using (var query = new SboRecordsetQuery(
                "SELECT [LineNum] FROM (SELECT ROW_NUMBER() OVER (ORDER BY [CntctCode] ASC) - 1 AS [LineNum], [Name] FROM [OCPR] " +
                $"WHERE [CardCode]='{cardCode}') AS [T0] WHERE [Name]='{contactId}'"))
            {
                if (query.Count == 0) return false;

                var lineNum = (int) query.Result.First().Item(0).Value;
                SboApp.Logger.Debug($"Contact ID '{contactId}' is LineNum {lineNum} for CardCode {cardCode}");

                contactEmployees.SetCurrentLine(lineNum);
                return true;
            }
        }
    }
}
