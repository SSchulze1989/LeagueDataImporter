using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Results.Convenience
{
    /// <summary>
    /// Convencience DTO for easy access to all scored results of a season
    /// </summary>
    [DataContract]
    public class SeasonResultsDTO : BaseDTO
    {
        /// <summary>
        /// Id of the season
        /// </summary>
        [DataMember]
        public long SeasonId { get; set; }
        /// <summary>
        /// Number of sessions on this season
        /// </summary>
        [DataMember]
        public int SessionCount { get; set; }
        /// <summary>
        /// Number of schedules on this season
        /// </summary>
        [DataMember]
        public int SchedulesCount { get; set; }
        /// <summary>
        /// Number of sessions with results on this season
        /// </summary>
        [DataMember]
        public int ResultsCount { get; set; }
        /// <summary>
        /// Results for each session
        /// </summary>
        [DataMember]
        public SessionResultsDTO[] SessionResults { get; set; }
    }
}
