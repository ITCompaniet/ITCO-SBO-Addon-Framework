using SAPbouiCOM;
using System;
using ITCO.SboAddon.Framework.Forms;
using ITCO.SboAddon.Framework.Helpers;

namespace ITCO.SboAddon.Framework.Extensions
{
    /// <summary>
    /// Form Extensions
    /// </summary>
    public static class FormExtensions
    {
        /// <summary>
        /// Get Form object from SBOItemEventArg
        /// </summary>
        /// <param name="pVal"></param>
        /// <returns>Form object</returns>
        public static Form GetForm(this SBOItemEventArg pVal)
        {
            return SboApp.Application.Forms.Item(pVal.FormUID);
        }

        /// <summary>
        /// Get Item Object in Form
        /// </summary>
        /// <typeparam name="T">Form Item Type</typeparam>
        /// <param name="form"></param>
        /// <param name="itemId">Item Id</param>
        /// <returns></returns>
        [Obsolete("Use GetEditText, GetButton, ... instead")]
        public static T Get<T>(this IForm form, string itemId)
        {
            return (T)form.Items.Item(itemId).Specific;
        }

        /// <summary>
        /// Get Static Text
        /// </summary>
        /// <param name="form"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static StaticText GetStaticText(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as StaticText;
        }

        /// <summary>
        /// Get Combo Box
        /// </summary>
        /// <param name="form"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static ComboBox GetComboBox(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as ComboBox;
        }

        /// <summary>
        /// Get Button
        /// </summary>
        /// <param name="form"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Button GetButton(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as Button;
        }

        /// <summary>
        /// Get Edit Text
        /// </summary>
        /// <param name="form"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static EditText GetEditText(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as EditText;
        }

        /// <summary>
        /// Get Check Box
        /// </summary>
        /// <param name="form"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static CheckBox GetCheckBox(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as CheckBox;
        }

        /// <summary>
        /// Get Matrix
        /// </summary>
        /// <param name="form"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Matrix GetMatrix(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as Matrix;
        }

        /// <summary>
        /// Add values into Combobox from SQL
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="sql"></param>
        public static void AddComboBoxValues(this ComboBox comboBox, string sql)
        {
            using (var query = new SboRecordsetQuery(sql))
            {
                foreach (var combo in query.Result)
                {
                    comboBox.ValidValues.Add(combo.Item(0).Value.ToString(), combo.Item(1).Value.ToString());
                }
            }
        }

        /// <summary>
        /// Freeze Form
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Freeze FreezeEx(this IForm form)
        {
            return new Freeze(form);
        }
    }

}