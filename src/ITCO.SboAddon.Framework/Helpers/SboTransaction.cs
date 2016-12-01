using SAPbobsCOM;
using System;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// SBO Transaction Object
    /// </summary>
    public class SboTransaction : IDisposable
    {
        private readonly Company _company;
        private bool _transactionEnded;

        /// <summary>
        /// New SBO Transaction, commits automatically on dispose
        /// </summary>
        /// <param name="company"></param>
        public SboTransaction(Company company)
        {
            _company = company;

            if (_company.InTransaction)
                throw new Exception("Already in transation");

            _company.StartTransaction();
            SboApp.Logger.Debug("StartTransaction");
        }

        /// <summary>
        /// Rollback Transaction
        /// </summary>
        public void Rollback()
        {
            _company.EndTransaction(BoWfTransOpt.wf_RollBack);
            _transactionEnded = true;
            SboApp.Logger.Debug("Rollback");
        }

        /// <summary>
        /// Commit Transaction
        /// </summary>
        public void Commit()
        {
            if (!_transactionEnded && _company.InTransaction)
            {
                _company.EndTransaction(BoWfTransOpt.wf_Commit);
                SboApp.Logger.Debug("Commit");
            }
        }

        /// <summary>
        /// Commit Transaction
        /// </summary>
        public void Dispose()
        {
            Commit();
        }
    }
}
