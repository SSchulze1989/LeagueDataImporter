using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueManager.Locations
{
    [DataContract]
    public class TrackMapSvg
    {
        [DataMember]
        public string TrackBackgroundSvg { get; set; }
        [DataMember]
        public string TrackInactiveSvg { get; set; }
        [DataMember]
        public string TrackActiveSvg { get; set; }
        [DataMember]
        public string TrackPitRoadSvg { get; set; }
        [DataMember]
        public string TrackStartFinishSvg { get; set; }
        [DataMember]
        public string TrackTurnsSvg { get; set; }
    }
}
