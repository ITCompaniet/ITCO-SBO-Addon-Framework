namespace ITCO.SboAddon.Framework.Extensions
{
    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Truncate string
        /// </summary>
        /// <param name="value">String to truncate</param>
        /// <param name="maxLength">Max Length</param>
        /// <returns>Truncated string</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Truncate string and warn log if truncaded
        /// </summary>
        /// <param name="value">String to truncate</param>
        /// <param name="maxLength">Max Length</param>
        /// <returns>Truncated string</returns>
        public static string TruncateAndWarn(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var truncated = value.Length <= maxLength ? value : value.Substring(0, maxLength);

            SboApp.Logger.Warn($"Truncating '{value}' to '{truncated}' (Max {maxLength})");
            return truncated;
        }
    }
}
