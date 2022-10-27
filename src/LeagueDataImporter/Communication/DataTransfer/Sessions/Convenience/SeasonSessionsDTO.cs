using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Sessions.Convenience
{
    /// <summary>
    /// Convenience DTO for basic session information of the whole season
    /// </summary>
    [DataContract]
    public class SeasonSessionsDTO : BaseDTO
    {
        [DataMember]
        public long SeasonId { get; set; }
        [DataMember]
        public string SeasonName { get; set; }
        /// <summary>
        /// Total number of sessions
        /// </summary>
        [DataMember]
        public int SessionsCount { get; set; }
        /// <summary>
        /// Total number of sessions finished
        /// </summary>
        [DataMember]
        public int SessionsFinished { get; set; }
        /// <summary>
        /// Total number of race sessions
        /// </summary>
        [DataMember]
        public int RacesCount { get; set; }
        /// <summary>
        /// Total number of races finished
        /// </summary>
        [DataMember]
        public int RacesFinished { get; set; }
        [DataMember]
        public ScheduleSessionsDTO[] Schedules { get; set; }
    }
}
