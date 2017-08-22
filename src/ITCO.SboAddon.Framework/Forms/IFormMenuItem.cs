
namespace ITCO.SboAddon.Framework.Forms
{
    /// <summary>
    /// Menu Item Interface
    /// </summary>
    public interface IFormMenuItem
    {
        /// <summary>
        /// MenuId
        /// </summary>
        string MenuItemId { get; }
        /// <summary>
        /// Text of MenuItem
        /// </summary>
        string MenuItemTitle { get; }
        /// <summary>
        /// Parent Menu Item (Financials, Banking, Tools etc.)
        /// </summary>
        string ParentMenuItemId { get; }
        /// <summary>
        /// Position under parent
        /// </summary>
        int MenuItemPosition { get; }
    }
}
