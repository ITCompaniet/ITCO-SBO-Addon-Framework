using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Helpers
{
    public static class FormHelper
    {
        public static IForm CreateFormFromResource(string resourceName, string formtype)
        {
            var formXml = string.Empty;

            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            using (var textStreamReader = new System.IO.StreamReader(stream))
                formXml = textStreamReader.ReadToEnd();

            var creationPackage = SboApp.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams) as FormCreationParams;
            
            creationPackage.FormType = formtype;
            creationPackage.BorderStyle = BoFormBorderStyle.fbs_Fixed;
            creationPackage.XmlData = formXml;

            var form = SboApp.Application.Forms.AddEx(creationPackage);
            return form;
        }
    }
}
