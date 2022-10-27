using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Statistics.Convenience
{
    [DataContract]
    public class StatsSetConvenienceDTO : BaseDTO
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Drivers { get; set; }

        [DataMember]
        public DriverStatisticRowDTO[] DriverStats { get; set; }
    }
}
