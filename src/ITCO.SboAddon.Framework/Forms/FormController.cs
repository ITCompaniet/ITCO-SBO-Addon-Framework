using ITCO.SboAddon.Framework.Helpers;
using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Forms
{
    /// <summary>
    /// Base Form Controller
    /// </summary>
    public abstract class FormController
    {
        /// <summary>
        /// Form Object
        /// </summary>
        protected readonly IForm _form;

        /// <summary>
        /// Eg. MyProject.Forms.MyForm.srf
        /// </summary>
        public abstract string FormResource { get; }

        /// <summary>
        /// Eg. NS_MyFormType1
        /// </summary>
        public abstract string FormType { get; }

        /// <summary>
        /// Create new Form
        /// </summary>
        public FormController()
        {
            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            _form = FormHelper.CreateFormFromResource(FormResource, FormType, assembly);
            BindFormEvents();
            _form.Visible = true;
        }

        /// <summary>
        /// Bind Events
        /// </summary>
        public virtual void BindFormEvents()
        {
        }

    }
}
