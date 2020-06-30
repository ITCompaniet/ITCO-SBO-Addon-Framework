using SAPbouiCOM;
using System;
using System.Reflection;

namespace ITCO.SboAddon.Framework.Helpers
{
    /// <summary>
    /// /FormHelper
    /// </summary>
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
        public static IForm CreateFormFromResource(string resourceName, string formtype, string formId = null, Assembly assembly = null, BoFormModality modality = BoFormModality.fm_None, BoFormBorderStyle borderStyle = BoFormBorderStyle.fbs_Fixed)
        {
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            if (formId != null)
            {
                // Try get existing form
                try
                {
                    var form = SboApp.Application.Forms.Item(formId);                    
                    return form;
                }
                catch
                {
                    // ignored
                }
            }

            try
            {
                string formXml;

                resourceName = string.Concat(assembly.GetName().Name, ".", resourceName);
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    var embededResources = string.Join(", ", assembly.GetManifestResourceNames());
                    throw new Exception($"Failed to load embeded resource '{resourceName}' from Assembly '{assembly.GetName().Name}'. Available Resources: {embededResources}");
                }

                using (var textStreamReader = new System.IO.StreamReader(stream))
                    formXml = textStreamReader.ReadToEnd();

                var creationPackage = SboApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams) as FormCreationParams;

                creationPackage.FormType = formtype;
                creationPackage.BorderStyle = borderStyle;
                creationPackage.XmlData = formXml;
                creationPackage.Modality = modality;

                if (formId != null)
                    creationPackage.UniqueID = formId;

                var form = SboApp.Application.Forms.AddEx(creationPackage);
                return form;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create form from resource {resourceName}: {e.Message}");
            }
        }
    }
}
