namespace ITCO.SboAddon.Framework.ChangeTracker
{
    using Helpers;
    using Setup;

    /// <summary>
    /// Setup for Change Tracker
    /// </summary>
    public class ChangeTrackerSetup : ISetup
    {
        /// <summary>
        /// Change Tracker Table
        /// </summary>
        public const string UdtChangeTracker = "ITCO_CHANGETRACKER";

        /// <summary>
        /// Key Field for Change Tracker
        /// </summary>
        public const string UdfKey = "ITCO_CT_Key";

        /// <summary>
        /// Object Field for Change Tracker 
        /// </summary>
        public const string UdfObj = "ITCO_CT_Obj";

        /// <summary>
        /// Version of Setup
        /// </summary>
        public int Version => 1;

        /// <summary>
        /// Run Setup
        /// </summary>
        public void Run()
        {
            UserDefinedHelper.CreateTable(UdtChangeTracker, "Change Tracker")
                .CreateUDF(UdfKey, "Key")
                .CreateUDF(UdfObj, "Object Type");
        }
    }
}
