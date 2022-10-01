using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews.Convenience
{
    /// <summary>
    /// Convencience DTO to get all reviews data of a whole season
    /// </summary>
    [DataContract]
    public class SeasonReviewsDTO : BaseDTO
    {
        /// <summary>
        /// Id of the season
        /// </summary>
        [DataMember]
        public long SeasonId { get; set; }
        /// <summary>
        /// Total number of reviews
        /// </summary>
        [DataMember]
        public int ReviewsCount { get; set; }
        /// <summary>
        /// Total number of sessions
        /// </summary>
        [DataMember]
        public int SessionCount { get; set; }
        /// <summary>
        /// Total number of sessions with reviews
        /// </summary>
        [DataMember]
        public int WithReviewsCount { get; set; }
        /// <summary>
        /// Reviews attached to each Session
        /// </summary>
        [DataMember]
        public SessionReviewsDTO[] SessionReviews { get; set; }
        /// <summary>
        /// Total number of schedules
        /// </summary>
        [DataMember]
        public int SchedulesCount { get; set; }
    }
}
