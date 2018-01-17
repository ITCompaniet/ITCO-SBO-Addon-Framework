namespace ITCO.SboAddon.Framework.Helpers.Dapper
{
    /// <summary>
    /// All type-handlers for Hana
    /// </summary>
    public static class HanaHandlers
    {
        /// <summary>
        /// Register all type-handlers for Hana
        /// </summary>
        public static void Register()
        {
            global::Dapper.SqlMapper.AddTypeHandler(HanaDecimalTypeHandler.Default);
        }
    }
}
