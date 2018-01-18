namespace ITCO.SboAddon.Framework.ChangeTracker
{
    /// <summary>
    /// Change Tracker Model
    /// </summary>
    public class KeyAndTimeStampModel
    {
        /// <summary>
        /// Gets Object Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets Timestamp in seconds
        /// </summary>
        public int Timestamp { get; set; }
    }
}