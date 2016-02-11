using SAPbouiCOM;
using System;
using ITCO.SboAddon.Framework.Helpers;

namespace ITCO.SboAddon.Framework.Extensions
{
    public static class FormExtensions
    {
        /// <summary>
        /// Get Form object from SBOItemEventArg
        /// </summary>
        /// <param name="pVal"></param>
        /// <returns>Form object</returns>
        public static Form GetForm(this SBOItemEventArg pVal)
        {
            return SboApp.Application.Forms.Item(pVal.FormUID) as Form;
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
        
        public static StaticText GetStaticText(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as StaticText;
        }

        public static ComboBox GetComboBox(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as ComboBox;
        }

        public static Button GetButton(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as Button;
        }

        public static EditText GetEditText(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as EditText;
        }

        public static CheckBox GetCheckBox(this IForm form, string itemId)
        {
            return form.Items.Item(itemId).Specific as CheckBox;
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
    }

}