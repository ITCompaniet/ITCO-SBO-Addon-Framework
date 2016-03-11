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
        /// </summary>
        /// <param name="documents"></param>
        /// <returns>NewObjectKey</returns>
        public static string AddEx(this IDocuments documents)
        {
            var returnCode = documents.Add();
            ErrorHelper.HandleErrorWithException(returnCode, "Could not Add Document");
            
            return SboApp.Company.GetNewObjectKey();
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
    }
}
