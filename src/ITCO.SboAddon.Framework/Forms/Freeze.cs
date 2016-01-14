using SAPbouiCOM;
using System;

namespace ITCO.SboAddon.Framework.Forms
{
    /// <summary>
    /// Freeze form
    /// </summary>
    public class Freeze : IDisposable
    {
        private readonly IForm _form;
        public Freeze(IForm form)
        {
            _form = form;
            _form.Freeze(true);
        }

        public void Dispose()
        {
            _form.Freeze(false);
        }
    }
}
