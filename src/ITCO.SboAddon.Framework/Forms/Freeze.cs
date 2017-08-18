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
        /// <summary>
        /// Freeze Form
        /// </summary>
        /// <param name="form"></param>
        public Freeze(IForm form)
        {
            _form = form;
            _form.Freeze(true);
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _form.Freeze(false);
        }
    }
}
