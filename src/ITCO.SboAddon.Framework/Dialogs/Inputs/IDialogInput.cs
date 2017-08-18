using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    /// <summary>
    /// Dialoginput Interface
    /// </summary>
    public interface IDialogInput
    {
        /// <summary>
        /// Get Id
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Get Title
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Get Required
        /// </summary>
        bool Required { get; }
        /// <summary>
        /// Set Item
        /// </summary>
        Item Item { set; }
        /// <summary>
        /// Get ItemType
        /// </summary>
        BoFormItemTypes ItemType { get; }
        /// <summary>
        /// Get DataType
        /// </summary>
        BoDataType DataType { get; }
        /// <summary>
        /// Get Validated
        /// </summary>
        bool Validated { get; }
        /// <summary>
        /// Get Length
        /// </summary>
        int Length { get; }
        /// <summary>
        /// Get DefaultValue
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// Get Value
        /// </summary>
        /// <returns></returns>
        object GetValue();
        /// <summary>
        /// Extras
        /// </summary>
        /// <param name="form"></param>
        /// <param name="yPos"></param>
        void Extras(Form form, int yPos);
    }
}
