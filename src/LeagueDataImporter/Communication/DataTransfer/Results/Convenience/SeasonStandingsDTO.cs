using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Results.Convenience
{
    [DataContract]
    public class SeasonStandingsDTO : BaseDTO
    {
        [DataMember]
        public long SeasonId { get; set; }

        [DataMember]
        public StandingsDataDTO[] Standings { get; set; }
    }
}
