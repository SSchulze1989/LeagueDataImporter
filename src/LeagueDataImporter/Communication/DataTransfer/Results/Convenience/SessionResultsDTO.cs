using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Results.Convenience
{
    /// <summary>
    /// Convencience DTO for easy access to all scored results of a session
    /// Might also include the raw results data
    /// </summary>
    [DataContract]
    public class SessionResultsDTO : BaseDTO
    {
        /// <summary>
        /// Id of the session
        /// </summary>
        [DataMember]
        public long SessionId { get; set; }
        /// <summary>
        /// Id of the schedule
        /// </summary>
        [DataMember]
        public long? ScheduleId { get; set; }
        /// <summary>
        /// Name of the schedule
        /// </summary>
        [DataMember]
        public string ScheduleName { get; set; }
        /// <summary>
        /// Id of the session location "trackid-configid"
        /// </summary>
        [DataMember]
        public string LocationId { get; set; }
        /// <summary>
        /// Number of the race in this season in chronological order (only for race sessions)
        /// </summary>
        [DataMember]
        public int RaceNr { get; set; }
        /// <summary>
        /// Number of scored results
        /// </summary>
        [DataMember] 
        public int Count { get; set; }
        /// <summary>
        /// Scored results data
        /// </summary>
        [DataMember] 
        public ScoredResultDataDTO[] ScoredResults { get; set; }
        /// <summary>
        /// Raw results data (if included)
        /// </summary>
        [DataMember(EmitDefaultValue = false)] 
        public ResultDataDTO RawResults { get; set; }
        /// <summary>
        /// Sim session details
        /// </summary>
        [DataMember(EmitDefaultValue = false)] 
        public SimSessionDetailsDTO SessionDetails { get; set; }
    }
}
