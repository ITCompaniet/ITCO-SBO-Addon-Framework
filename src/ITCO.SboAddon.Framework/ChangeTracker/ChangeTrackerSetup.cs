namespace ITCO.SboAddon.Framework.ChangeTracker
{
    using Helpers;
    using Setup;

    /// <summary>
    /// Setup for Change Tracker
    /// </summary>
    public class ChangeTrackerSetup : ISetup
    {
        public const string UdtChangeTracker = "ITCO_CHANGETRACKER";
        public const string UdfKey = "ITCO_CT_Key";
        public const string UdfObj = "ITCO_CT_Obj";

        public int Version => 1;

        public void Run()
        {
            UserDefinedHelper.CreateTable(UdtChangeTracker, "Change Tracker")
                .CreateUDF(UdfKey, "Key")
                .CreateUDF(UdfObj, "Object Type");
        }
    }
}
