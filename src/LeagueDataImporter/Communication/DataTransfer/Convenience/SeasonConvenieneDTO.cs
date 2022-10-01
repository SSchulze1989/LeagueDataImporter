using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Convenience
{
    [DataContract]
    public class SeasonConvenieneDTO : BaseDTO
    {
        [DataMember]
        public long SeasonId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int SessionCount { get; set; }
        [DataMember]
        public int RacesCount { get; set; }
        [DataMember]
        public int RacesFinished { get; set; }
        [DataMember]
        public DateTime? SeasonStart { get; set; }
        [DataMember]
        public DateTime? SeasonEnd { get; set; }
        [DataMember]
        public bool IsFinished { get; set; }
        [DataMember]
        public bool IsCurrentSeason { get; set; }
    }
}
