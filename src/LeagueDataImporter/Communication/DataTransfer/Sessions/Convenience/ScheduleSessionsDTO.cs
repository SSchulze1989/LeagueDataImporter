using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Sessions.Convenience
{
    /// <summary>
    /// Convenience DTO for getting basic session information for a schedule
    /// </summary>
    [DataContract]
    public class ScheduleSessionsDTO : BaseDTO
    {
        /// <summary>
        /// Id of the schedule
        /// </summary>
        [DataMember]
        public long ScheduleId { get; set; }
        /// <summary>
        /// Full schedule name
        /// </summary>
        [DataMember]
        public string ScheduleName { get; set; }
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
        /// <summary>
        /// Array of sessions for the whole Schedule
        /// </summary>
        [DataMember]
        public SessionDataDTO[] Sessions { get; set; }
    }
}
