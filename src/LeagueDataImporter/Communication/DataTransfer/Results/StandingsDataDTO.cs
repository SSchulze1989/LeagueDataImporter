// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.DataTransfer.Members;

namespace iRLeagueDatabase.DataTransfer.Results
{
    [DataContract]
    public class StandingsDataDTO : MappableDTO
    {
        [DataMember]
        //public ScoringInfoDTO Scoring { get; set; }
        public long ScoringId { get; set; }
        [DataMember]
        public long ScoringTableId { get; set; }
        [DataMember]
        public string Name { get; set; }
        public override object MappingId => new long[] { ScoringTableId, SessionId.GetValueOrDefault() };
        [DataMember]
        public long? SessionId { get; set; }

        public override object[] Keys => new object[] { ScoringTableId, SessionId.GetValueOrDefault() };
        [DataMember]
        public virtual StandingsRowDataDTO[] StandingsRows { get; set; }
        [DataMember]
        //public virtual LeagueMemberInfoDTO MostWinsDriver { get; set; }
        public long? MostWinsDriverId { get; set; }
        [DataMember]
        //public virtual LeagueMemberInfoDTO MostPolesDriver { get; set; }
        public long? MostPolesDriverId { get; set; }
        [DataMember]
        //public virtual LeagueMemberInfoDTO CleanestDriver { get; set; }
        public long? CleanestDriverId { get; set; }
        [DataMember]
        //public virtual LeagueMemberInfoDTO MostPenaltiesDriver { get; set; }
        public long? MostPenaltiesDriverId { get; set; }
    }
}
