namespace ITCO.SboAddon.Framework.Setup
{
    /// <summary>
    /// Setup interface, for automatic Setup
    /// </summary>
    public interface ISetup
    {
        /// <summary>
        /// Setup version
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Setup Run Method
        /// </summary>
        void Run();
    }
}
