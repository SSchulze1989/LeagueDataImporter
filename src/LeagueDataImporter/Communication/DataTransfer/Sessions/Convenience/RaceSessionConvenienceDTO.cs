using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Sessions.Convenience
{
    [DataContract]
    public class RaceSessionConvenienceDTO : RaceSessionDataDTO
    {
        /// <summary>
        /// Name of the schedule
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ScheduleName { get; set; }

        /// <summary>
        /// Number of the race in the whole season in chronological order
        /// </summary>
        [DataMember]
        public int RaceNr { get; set; }
        [DataMember]
        public bool HasResult { get; set; }

        // Exclude information
        [IgnoreDataMember]
        //public ResultInfoDTO SessionResult { get; set; }
        public new long? SessionResultId { get; set; }
        [IgnoreDataMember]
        //public IncidentReviewInfoDTO[] Reviews { get; set; }
        public new long[] ReviewIds { get; set; }

        #region Version Info
        // Exclude version information
        [IgnoreDataMember]
        public new DateTime? CreatedOn { get => base.CreatedOn; set => base.CreatedOn = value; }
        [IgnoreDataMember]
        public new DateTime? LastModifiedOn { get => base.LastModifiedOn; set => base.LastModifiedOn = value; }
        [IgnoreDataMember]
        public new string CreatedByUserId { get => base.CreatedByUserId; set => base.CreatedByUserId = value; }
        [IgnoreDataMember]
        public new string LastModifiedByUserId { get => base.LastModifiedByUserId; set => base.LastModifiedByUserId = value; }
        [IgnoreDataMember]
        public new string CreatedByUserName { get => base.CreatedByUserName; set => base.CreatedByUserName = value; }
        [IgnoreDataMember]
        public new string LastModifiedByUserName { get => base.LastModifiedByUserName; set => base.LastModifiedByUserName = value; }
        #endregion
    }
}
