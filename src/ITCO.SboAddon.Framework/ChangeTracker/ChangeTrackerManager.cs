namespace ITCO.SboAddon.Framework.ChangeTracker
{
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
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
            var sql = string.Empty;
            if (SboApp.IsHana)
                sql = "SELECT DISTINCT \"U_ITCO_CT_Key\" AS \"Key\", CAST(\"Code\" AS int) AS \"Timestamp\" FROM \"@ITCO_CHANGETRACKER\" " +
                $"WHERE \"U_ITCO_CT_Obj\" = {(int)objectType} AND CAST(\"Code\" AS int) > {timeStamp} " +
                "ORDER BY CAST(\"Code\" AS int) ASC";
            else
                sql = "SELECT DISTINCT [U_ITCO_CT_Key] AS [Key], CAST([Code] AS int) AS [Timestamp] FROM [@ITCO_CHANGETRACKER] " +
                    $"WHERE [U_ITCO_CT_Obj] = {(int)objectType} AND CAST([Code] AS int) > {timeStamp} " +
                    "ORDER BY CAST([Code] AS int) ASC";

            using (var query = new SboRecordsetQuery(sql))
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
