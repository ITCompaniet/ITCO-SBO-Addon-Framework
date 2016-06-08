
namespace ITCO.SboAddon.Framework.Dialogs.Inputs
{
    public static class InputExtensions
    {
        public static InputHelper AddText(this InputHelper inputHelper, string id, string title, string defaultValue = null, bool required = false, int length = 20)
        {
            return inputHelper.AddInput(new TextDialogInput(id, title, defaultValue, required, length));
        }
    }
}
