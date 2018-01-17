namespace ITCO.SboAddon.Framework.Data.Dapper
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
