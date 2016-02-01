using SAPbouiCOM;
using System;
using System.Reflection;

namespace ITCO.SboAddon.Framework.Helpers
{
    public static class FormHelper
    {
        /// <summary>
        /// Create SBO Form from internal resource
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="formtype">Form type (identifyer)</param>
        /// <param name="formId">Optional FormId</param>
        /// <param name="assembly"></param>
        /// <returns>IForm reference</returns>
        public static IForm CreateFormFromResource(string resourceName, string formtype, string formId = null, Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            var formXml = string.Empty;
            try
            {
                resourceName = string.Concat(assembly.GetName().Name, ".", resourceName);
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    throw new Exception(string.Format("Failed to load resource {0}", resourceName));

                using (var textStreamReader = new System.IO.StreamReader(stream))
                    formXml = textStreamReader.ReadToEnd();

                var creationPackage = SboApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams) as FormCreationParams;

                creationPackage.FormType = formtype;
                creationPackage.BorderStyle = BoFormBorderStyle.fbs_Fixed;
                creationPackage.XmlData = formXml;

                if (formId != null)
                    creationPackage.UniqueID = formId;

                var form = SboApp.Application.Forms.AddEx(creationPackage);
                return form;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Failed to create form from resource {0}: {1}", resourceName, e.Message));
            }
        }
    }
}
