using System;
using System.Collections.Generic;
using ITCO.SboAddon.Framework.Helpers;
using SAPbobsCOM;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class DocumentExtensions
    {
        /// <summary>
        /// Add Document.
        /// Throws Exeption if failed instead of returnCode.
        /// Returns DocEntry
        /// </summary>
        /// <param name="documents"></param>
        /// <returns>Document Entry Key</returns>
        public static int AddEx(this IDocuments documents)
        {
            var returnCode = documents.Add();
            ErrorHelper.HandleErrorWithException(returnCode, "Could not Add Document");
            
            return int.Parse(SboApp.Company.GetNewObjectKey());
        }

        /// <summary>
        /// Add Document and load added entry into Business Object
        /// </summary>
        /// <param name="documents"></param>
        public static void AddAndLoadEx(this IDocuments documents)
        {
            var returnCode = documents.Add();
            ErrorHelper.HandleErrorWithException(returnCode, "Could not Add Document");

            var documentEntryKey = int.Parse(SboApp.Company.GetNewObjectKey());
            if (!documents.GetByKey(documentEntryKey))
                throw new Exception($"Could not load document with DocEntry {documentEntryKey}");
        }

        /// <summary>
        /// Update Document.
        /// Throws Exeption if failed instead of returnCode.
        /// </summary>
        /// <param name="documents"></param>
        public static void UpdateEx(this IDocuments documents)
        {
            var returnCode = documents.Update();
            ErrorHelper.HandleErrorWithException(returnCode, "Could not Update Document");
        }

        /// <summary>
        /// Get DocumentLines as Enumerable
        /// </summary>
        /// <param name="lines">Document_Lines</param>
        /// <returns></returns>
        public static IEnumerable<Document_Lines> AsEnumerable(this Document_Lines lines)
        {
            var line = -1;
            while (++line < lines.Count)
            {
                lines.SetCurrentLine(line);
                yield return lines;
            }
        }

        /// <summary>
        /// Search Document
        /// </summary>
        /// <param name="document"></param>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static bool Search(this IDocuments document, string table, string where)
        {
            var recordSet = SboApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
            recordSet.DoQuery($"SELECT * FROM [{table}] WHERE {where}");
            document.Browser.Recordset = recordSet;
            return recordSet.RecordCount != 0;
        }
    }
}
