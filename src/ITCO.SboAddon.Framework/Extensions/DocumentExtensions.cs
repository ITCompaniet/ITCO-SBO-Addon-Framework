namespace ITCO.SboAddon.Framework.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using SAPbobsCOM;

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
        /// Get Expenses as Enumerable
        /// </summary>
        /// <param name="expenses"></param>
        /// <returns></returns>
        public static IEnumerable<DocumentsAdditionalExpenses> AsEnumerable(this DocumentsAdditionalExpenses expenses)
        {
            var line = -1;
            while (++line < expenses.Count)
            {
                expenses.SetCurrentLine(line);
                yield return expenses;
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

        /// <summary>
        /// Copy document
        /// </summary>
        /// <param name="sourceDocument"></param>
        /// <param name="copyToType"></param>
        /// <param name="copyExpenses">Copy Expenses</param>
        /// <param name="setObjectProperties">Set extra target document properties</param>
        /// <returns>DocEntry</returns>
        public static int CopyTo(this IDocuments sourceDocument, BoObjectTypes copyToType, bool copyExpenses = true, Action<Documents> setObjectProperties = null)
        {
            using (var copyTo = SboApp.Company.GetBusinessObject<Documents>(copyToType))
            {
                var targetDocument = copyTo.Object;

                targetDocument.CardCode = sourceDocument.CardCode;

                setObjectProperties?.Invoke(targetDocument);

                foreach (var sourceLine in sourceDocument.Lines.AsEnumerable())
                {
                    targetDocument.Lines.BaseEntry = sourceLine.DocEntry;
                    targetDocument.Lines.BaseLine = sourceLine.LineNum;
                    targetDocument.Lines.BaseType = (int) sourceDocument.DocObjectCode;
                    targetDocument.Lines.Add();
                }

                if (copyExpenses)
                {
                    foreach (var sourceExpense in sourceDocument.Expenses.AsEnumerable().Where(e => e.LineTotal > 0))
                    {
                        targetDocument.Expenses.BaseDocEntry = sourceDocument.DocEntry;
                        targetDocument.Expenses.BaseDocLine = sourceExpense.LineNum;
                        targetDocument.Expenses.BaseDocType = (int) sourceDocument.DocObjectCode;
                        targetDocument.Expenses.Add();
                    }
                }

                targetDocument.AddAndLoadEx();

                return targetDocument.DocEntry;
            }
        }

        /// <summary>
        /// Get Line weight in mg
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static decimal GetWeightInMg(this Document_Lines line)
        {
            using (var weightMeasures = SboApp.Company.GetBusinessObject<WeightMeasures>(BoObjectTypes.oWeightMeasures))
            {
                if (weightMeasures.Object.GetByKey(line.Weight1Unit))
                    return (decimal) (line.Weight1 * weightMeasures.Object.UnitWeightinmg);

                return 0;
            }
        }

        /// <summary>
        /// Get Package Line weight in mg
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static decimal GetWeightInMg(this IDocumentPackages line)
        {
            using (var weightMeasures = SboApp.Company.GetBusinessObject<WeightMeasures>(BoObjectTypes.oWeightMeasures))
            {
                if (weightMeasures.Object.GetByKey(line.Units))
                    return (decimal) (line.TotalWeight * weightMeasures.Object.UnitWeightinmg);

                return 0;
            }
        }

        /// <summary>
        /// Get table name from BoObjectTypes
        /// </summary>
        /// <param name="boObjectType"></param>
        /// <returns>Table name</returns>
        public static string GetTableName(this BoObjectTypes boObjectType)
        {
            switch (boObjectType)
            {
                case BoObjectTypes.oBusinessPartners:
                    return "OCRD";
                case BoObjectTypes.oItems:
                    return "OITM";
                case BoObjectTypes.oInvoices:
                    return "OINV";
                case BoObjectTypes.oDeliveryNotes:
                    return "ODLN";
                case BoObjectTypes.oOrders:
                    return "ORDR";
            }
            throw new NotImplementedException($"I dont know the table for {boObjectType} yet :(");
        }

        /// <summary>
        /// Add Comment to document
        /// </summary>
        /// <param name="documents">IDocuments object</param>
        /// <param name="comment">Comment text</param>
        public static void AddComment(this IDocuments documents, string comment)
        {
            documents.Comments = documents.Comments.AddNewLine(comment);
        }

        /// <summary>
        /// Add new line of text
        /// </summary>
        /// <param name="existingText"></param>
        /// <param name="newLine"></param>
        /// <returns></returns>
        public static string AddNewLine(this string existingText, string newLine)
        {
            var lines = existingText.Split(new[] { "\n" }, StringSplitOptions.None);
            if (lines[lines.Length - 1] != "")
                existingText += $"\n{newLine}\n";
            else
                existingText += $"{newLine}\n";

            return existingText;
        }

        /// <summary>
        /// Get DocEntry from DocNum
        /// </summary>
        /// <param name="docNum">DocNum</param>
        /// <param name="table">SBO Table</param>
        /// <returns>DocEntry</returns>
        public static int? GetDocEntry(this int docNum, string table)
        {
            using (var query = new SboRecordsetQuery($"SELECT [DocEntry] FROM [{table}] WHERE [DocNum]={docNum}"))
            {
                if (query.Count == 0) return null;
                return int.Parse(query.Result.First().Item("DocEntry").Value.ToString());
            }
        }
    }
}
