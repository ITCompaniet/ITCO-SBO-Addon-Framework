using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class FormExtensions
    {
        /// <summary>
        /// Get Form object from SBOItemEventArg
        /// </summary>
        /// <param name="pVal"></param>
        /// <returns></returns>
        public static Form GetForm(this SBOItemEventArg pVal)
        {
            return SboApp.Application.Forms.Item(pVal.FormUID) as Form;
        }

        /// <summary>
        /// Get Item Object in Form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="form"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static T Get<T>(this IForm form, string itemId)
        {
            return (T)form.Items.Item(itemId).Specific;
        }
    }
}