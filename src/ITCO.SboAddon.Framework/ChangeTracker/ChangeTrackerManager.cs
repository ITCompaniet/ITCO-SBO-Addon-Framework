namespace ITCO.SboAddon.Framework.ChangeTracker
{
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using SAPbobsCOM;

    /// <summary>
    /// Change Tracker Manager
    /// </summary>
    public static class ChangeTrackerManager
    {
        /// <summary>
        /// Get Changed Items for object since timestamp
        /// </summary>
        /// <param name="timeStamp">Timestamp (secounds from a given startpoint)</param>
        /// <param name="objectType">BoObject Type</param>
        /// <returns>Collection of Key and timestamp for updated objects</returns>
        public static ICollection<KeyAndTimeStampModel> GetChanged(int timeStamp, BoObjectTypes objectType)
        {
            using (var query = new SboRecordsetQuery(
                "SELECT DISTINCT [U_ITCO_CT_Key] AS [Key], CAST([Code] AS int) AS [Timestamp] FROM [@ITCO_CHANGETRACKER] " +
                $"WHERE [U_ITCO_CT_Obj] = {timeStamp} AND CAST([Code] AS int) > {(int) objectType} " +
                "ORDER BY CAST([Code] AS int) ASC"))
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
