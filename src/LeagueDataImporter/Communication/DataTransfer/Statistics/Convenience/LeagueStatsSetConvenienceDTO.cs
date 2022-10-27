using iRLeagueDatabase.DataTransfer.Members.Convenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Statistics.Convenience
{
    [DataContract]
    public class LeagueStatsSetConvenienceDTO : StatsSetConvenienceDTO
    {
        [DataMember]
        public DriverDTO CurrentChamp { get; set; }
    }
}
