namespace ITCO.SboAddon.Framework.ChangeTracker
{
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using SAPbobsCOM;
    using Setup;
    using ITCO.SboAddon.Framework.Queries;

    /// <summary>
    /// Interface for Change Tracker Manager
    /// </summary>
    public interface IChangeTrackerManager
    {
        /// <summary>
        /// Get Changed Items for object since timestamp
        /// </summary>
        /// <param name="timeStamp">Timestamp (secounds from a given startpoint)</param>
        /// <param name="objectType">BoObject Type</param>
        /// <returns>Collection of Key and timestamp for updated objects</returns>
        ICollection<KeyAndTimeStampModel> GetChanged(int timeStamp, BoObjectTypes objectType);
    }

    /// <summary>
    /// Change Tracker Manager
    /// </summary>
    public class ChangeTrackerManager : IChangeTrackerManager
    {
        private static ChangeTrackerManager _instance;

        /// <summary>
        /// Get static instance of ChangeTrackerManager
        /// </summary>
        public static ChangeTrackerManager Instance => _instance ?? (_instance = new ChangeTrackerManager());

        /// <summary>
        /// Run Setup via SetupManager
        /// </summary>
        public static void RunSetup() => SetupManager.RunSetup(new ChangeTrackerSetup());

        /// <summary>
        /// Get Changed Items for object since timestamp
        /// </summary>
        /// <param name="timeStamp">Timestamp (secounds from a given startpoint)</param>
        /// <param name="objectType">BoObject Type</param>
        /// <returns>Collection of Key and timestamp for updated objects</returns>
        public ICollection<KeyAndTimeStampModel> GetChanged(int timeStamp, BoObjectTypes objectType)
        {
            using (var query = new SboRecordsetQuery(FrameworkQueries.Instance.GetChangedQuery(timeStamp,(int)objectType)))
            {
                if (query.Count == 0)
                    return new List<KeyAndTimeStampModel>();

                return query.Result.Select(r => new KeyAndTimeStampModel
                {
                    Key = r.Item(0).Value.ToString(),
                    Timestamp = int.Parse(r.Item(1).Value.ToString())
                }).ToList();
            }
        }
    }

    /// <summary>
    /// Change Tracker Model
    /// </summary>
    public class KeyAndTimeStampModel
    {
        /// <summary>
        /// Object Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Timestamp in secounds
        /// </summary>
        public int Timestamp { get; set; }
    }
}
