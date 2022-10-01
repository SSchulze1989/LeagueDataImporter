using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Members.Convenience
{
    /// <summary>
    /// Short driver info for a single member
    /// </summary>
    public class DriverDTO : BaseDTO
    {
        /// <summary>
        /// Member id of the driver
        /// </summary>
        public long MemberId { get; set; }
        /// <summary>
        /// Full name of the driver
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Team name of the driver.
        /// If this is part of a result in the past the team name might be different from the current team of the member.
        /// </summary>
        public string TeamName { get; set; }
    }
}
