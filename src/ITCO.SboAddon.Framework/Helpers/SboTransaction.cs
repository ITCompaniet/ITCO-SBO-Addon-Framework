using SAPbobsCOM;
using System;

namespace ITCO.SboAddon.Framework.Helpers
{
    public class SboTransaction : IDisposable
    {
        private readonly Company _company;
        private bool transactionEnded;

        public SboTransaction(Company company)
        {
            _company = company;

            if (_company.InTransaction)
                throw new Exception("Already in transation");

            _company.StartTransaction();
        }

        public void Rollback()
        {
            _company.EndTransaction(BoWfTransOpt.wf_RollBack);
            transactionEnded = true;
        }

        public void Dispose()
        {
            if (!transactionEnded && _company.InTransaction)
                _company.EndTransaction(BoWfTransOpt.wf_Commit);
        }
    }
}
