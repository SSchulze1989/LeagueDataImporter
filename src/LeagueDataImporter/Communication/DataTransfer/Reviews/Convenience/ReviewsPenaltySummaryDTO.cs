using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews.Convenience
{
    /// <summary>
    /// DTO containing summarized penalty information for a single review
    /// </summary>
    [DataContract]
    public class ReviewsPenaltySummaryDTO : BaseDTO
    {
        /// <summary>
        /// Total number of penalties applied
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// Total sum of penalty points
        /// </summary>
        [DataMember]
        public int Points { get; set; }
        /// <summary>
        /// Summarized penalty information for each penalized driver
        /// </summary>
        [DataMember]
        public MemberPenaltySummaryDTO[] DrvPenalties { get; set; }
    }
}
