using ITCO.SboAddon.Framework.Helpers;
using SAPbouiCOM;
using System;

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
        protected IForm _form;

        /// <summary>
        /// Eg. Forms.MyForm.srf
        /// </summary>
        public virtual string FormResource
        {
            get
            {
                return string.Format("Forms.{0}.srf", GetType().Name.Replace("Controller", string.Empty));
            }
        }

        /// <summary>
        /// Eg. NS_MyFormType1
        /// </summary>
        public virtual string FormType
        {
            get
            {
                return string.Format("ITCO_{0}", GetType().Name.Replace("Controller", string.Empty));
            }
        }
        /// <summary>
        /// Create new Form
        /// </summary>        
        public FormController(bool autoStart = false)
        {
            if (autoStart)
                Start();
        }

        /// <summary>
        /// Initalize and show form
        /// </summary>
        public void Start()
        {
            if (_form != null)
                return;
            try
            {
                var assembly = GetType().Assembly;
                _form = FormHelper.CreateFormFromResource(FormResource, FormType, assembly);
                BindFormEvents();
                _form.Visible = true;
            }
            catch (Exception e)
            {
                SboApp.Application.MessageBox(string.Format("Failed to open form {0}: {1}", FormType, e.Message));
            }      
        }

        /// <summary>
        /// Bind Events
        /// </summary>
        public virtual void BindFormEvents()
        {
        }

        #region Default IFormMenuItem
        public string MenuItemId
        {
            get { return string.Format("{0}_M", FormType); }
            
        }

        public int MenuItemPosition
        {
            get { return -1; }
        }
        #endregion
    }
}
