using System;
using System.Linq;
using System.Threading;
using ITCO.SboAddon.Framework.Helpers;
using SAPbobsCOM;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class TransactionExtensions
    {
        /// <summary>
        /// Wait for open transaction to complete
        /// Useful when using BeginTransaction
        /// </summary>
        /// <param name="company"></param>
        /// <param name="sleep"></param>
        /// <param name="tryCount"></param>
        public static void WaitForOpenTransactions(this Company company, int sleep = 500, int tryCount = 10)
        {
            for (var i = 0; i < tryCount; i++)
            {
                using (var query = new SboRecordsetQuery(
                    $"SELECT hostname, loginame FROM sys.sysprocesses WHERE open_tran=1 AND dbid=DB_ID('{company.CompanyDB}')"))
                {
                    if (query.Count == 0)
                        return;

                    var openTransaction = query.Result.First();

                    SboApp.Logger.Trace($"Open Transaction by {openTransaction.Item("hostname").Value}, waiting {sleep} ms...");
                }
                Thread.Sleep(sleep);
            }

            throw new Exception($"Waiting for open transactions to long! ({sleep * tryCount} ms)");
        }
    }
}
