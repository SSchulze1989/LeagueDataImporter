using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Statistics.Special
{
    [DataContract]
    public class StatisticRowCSV : DriverStatisticRowDTO
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string IRacingId { get; set; }
        [DataMember]
        public string TeamName { get; set; }
        [DataMember]
        public double FairPlayRating { get; set; }
        [DataMember]
        public string DriverRank { get; set; }
        [DataMember]
        public int RankValue { get; set; }
        [DataMember]
        public new int Titles { get => base.Titles; set => base.Titles = value; }
        [DataMember]
        public int HeTitles { get; set; }
        [DataMember]
        public new int RacesCompletedPct { get => (int)Math.Round(100 * base.RacesCompletedPct); set => base.RacesCompletedPct = (double)value / 100; }
        
        [DataMember]
        public bool IsCurrentChamp { get; set; }
        [DataMember]
        public bool IsCurrentHeChamp { get; set; }
    }
}
