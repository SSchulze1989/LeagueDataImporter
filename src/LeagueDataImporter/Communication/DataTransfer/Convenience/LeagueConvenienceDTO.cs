using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Convenience
{
    public class LeagueConvenienceDTO : BaseDTO
    {
        /// <summary>
        /// Shortname of the league. To be used as "leagueName" for all API methods 
        /// Cannot contain spaces and special characters
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Long name of the league. Can contain spaces and special characters
        /// </summary>
        [DataMember]
        public string LongName { get; set; }
        [DataMember]
        public int SeasonsCount { get; set; }
        [DataMember]
        public SeasonConvenieneDTO[] Seasons { get; set; }
    }
}
