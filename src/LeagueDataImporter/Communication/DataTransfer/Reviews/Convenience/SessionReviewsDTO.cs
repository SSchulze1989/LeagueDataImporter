using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews.Convenience
{
    [DataContract]
    public class SessionReviewsDTO : BaseDTO
    {
        /// <summary>
        /// Id of the session for the reviews - if reviews are from multiple sessions this value is null
        /// </summary>
        [DataMember]
        public long SessionId { get; set; }
        /// <summary>
        /// Number of the race in this season in chronological order
        /// </summary>
        [DataMember]
        public int RaceNr { get; set; }
        /// <summary>
        /// Total number of reviews
        /// </summary>
        [DataMember]
        public int Total { get; set; }
        /// <summary>
        /// Nubmer of reviews that have votes
        /// </summary>
        [DataMember]
        public int Voted { get; set; }
        /// <summary>
        /// Number of reviews that are still open
        /// </summary>
        [DataMember]
        public int Open { get; set; }
        /// <summary>
        /// Number of reviews that are closed
        /// </summary>
        [DataMember]
        public int Closed { get; set; }
        /// <summary>
        /// Acceted votes and count of occurance of each vote category
        /// </summary>
        [DataMember]
        public CountValue<VoteCategoryDTO>[] Results { get; set; }
        /// <summary>
        /// Total number of penalties given
        /// </summary>
        [DataMember]
        public ReviewsPenaltySummaryDTO Penalties { get; set; }
        /// List of reviews
        /// </summary>
        [DataMember]
        public IncidentReviewDataDTO[] Reviews { get; set; }

    }
}
