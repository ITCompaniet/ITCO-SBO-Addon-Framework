namespace ITCO.SboAddon.Framework.ChangeTracker
{
    using System.Collections.Generic;
    using System.Linq;
    using ITCO.SboAddon.Framework.Data;
    using ITCO.SboAddon.Framework.Queries;
    using SAPbobsCOM;
    using Setup;

    /// <summary>
    /// Interface for Change Tracker Manager
    /// </summary>
    public interface IChangeTrackerManager
    {
        /// <summary>
        /// Get Changed Items for object since timestamp
        /// </summary>
        /// <param name="timeStamp">Timestamp (seconds from a given start point)</param>
        /// <param name="objectType">BoObject Type</param>
        /// <returns>Collection of Key and timestamp for updated objects</returns>
        ICollection<KeyAndTimeStampModel> GetChanged(int timeStamp, BoObjectTypes objectType);
    }

    /// <summary>
    /// Change Tracker Manager
    /// </summary>
    public class ChangeTrackerManager : IChangeTrackerManager
    {
        private static ChangeTrackerManager instance;

        /// <summary>
        /// Get static instance of ChangeTrackerManager
        /// </summary>
        public static ChangeTrackerManager Instance => instance ?? (instance = new ChangeTrackerManager());

        /// <summary>
        /// Run Setup via SetupManager
        /// </summary>
        public static void RunSetup() => SetupManager.RunSetup(new ChangeTrackerSetup());

        /// <summary>
        /// Get Changed Items for object since timestamp
        /// </summary>
        /// <param name="timeStamp">Timestamp (seconds from a given start point)</param>
        /// <param name="objectType">BoObject Type</param>
        /// <returns>Collection of Key and timestamp for updated objects</returns>
        public ICollection<KeyAndTimeStampModel> GetChanged(int timeStamp, BoObjectTypes objectType)
        {
            using (var sboDbConnection = new SboDbConnection())
            {
                var sql = FrameworkQueries.Instance.GetChangedQuery(timeStamp, (int)objectType);
                return sboDbConnection.Query<KeyAndTimeStampModel>(sql).ToList();
            }
        }
    }
}
