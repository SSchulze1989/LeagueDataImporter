using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews.Convenience
{
    /// <summary>
    /// DTO containing summarized penalty information for a single driver
    /// </summary>
    [DataContract]
    public class MemberPenaltySummaryDTO : BaseDTO
    {
        /// <summary>
        /// Database member id
        /// </summary>
        [DataMember]
        public long MemberId { get; set; }
        /// <summary>
        /// Fullname of the driver
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Total number of penalties
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// Total sum of penalty points
        /// </summary>
        [DataMember]
        public int Points { get; set; }
        /// <summary>
        /// Array of all accepted vote results that resulted in a penalty
        /// </summary>
        [DataMember]
        public ReviewVoteDataDTO[] Penalties { get; set; }
    }
}
