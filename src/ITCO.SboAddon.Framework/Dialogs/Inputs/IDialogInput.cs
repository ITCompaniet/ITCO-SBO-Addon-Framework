using SAPbouiCOM;

namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public interface IDialogInput
    {
        string Id { get; }
        string Title { get; }
        bool Required { get; }
        Item Item { set; }
        BoFormItemTypes ItemType { get; }
        BoDataType DataType { get; }
        bool Validated { get; }
        int Length { get; }
        string DefaultValue { get; }

        object GetValue();
        void Extras(Form form, int yPos);
    }
}
