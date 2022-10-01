using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Statistics.Convenience
{
    [DataContract]
    public class StatisticConvenienceDTO : BaseDTO
    {
        [DataMember(EmitDefaultValue = false)]
        public SeasonStatsSetConvenienceDTO[] SeasonStats { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public LeagueStatsSetConvenienceDTO[] LeagueStats { get; set; }
    }
}
