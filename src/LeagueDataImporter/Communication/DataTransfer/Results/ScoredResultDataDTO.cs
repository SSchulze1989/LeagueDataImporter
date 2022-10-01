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
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace iRLeagueDatabase.DataTransfer.Results
{
    [DataContract]
    public class ScoredResultDataDTO : ResultInfoDTO, IMappableDTO
    {
        //[DataMember]
        //public long? ScoredResultId { get; set; }
        [DataMember]
        //public ScoringInfoDTO Scoring { get; set; }
        public long ScoringId { get; set; }

        [DataMember]
        public string ScoringName { get; set; }

        public override object MappingId => new long[] { ResultId.GetValueOrDefault(), ScoringId };

        public override object[] Keys => new object[] { ResultId.GetValueOrDefault(), ScoringId };

        [DataMember(Name = "HrdChrgIds")]
        public long[] HardChargerMemberIds { get; set; }
        [DataMember(Name  = "MostPosGainedIds")]
        public long[] MostPositionsGainedMemberIds { get; set; }
        [DataMember(Name = "MostPosGained")]
        public int MostPositionsGained { get; set; }
        [DataMember(Name = "CleanDrvIds")]
        public long[] CleanesDriverMemberIds { get; set; }
        [DataMember(Name = "FLapDrvId")]
        public long? FastestLapDriverId { get; set; }
        [DataMember(Name = "FLapTime")]
        public TimeSpan FastesLapTime { get; set; }
        [DataMember(Name = "QLapDrvId")]
        public long? FastestQualyLapDriver { get; set; }
        [DataMember(Name = "QLapTime")]
        public TimeSpan FastestQualyLapTime { get; set; }
        [DataMember(Name = "FAvgLapDrvId")]
        public long? FastestAvgLapDriver { get; set; }
        [DataMember(Name = "FAvgLapTime")]
        public TimeSpan FastestAvgLapTime { get; set; }

        public override Type Type => typeof(ScoredResultDataDTO);
        //object IMappableDTO.MappingId => MappingId;
        [DataMember]
        public ScoredResultRowDataDTO[] FinalResults { get; set; }

        #region Version Info
        [DataMember]
        public new DateTime? CreatedOn { get => base.CreatedOn; set => base.CreatedOn = value; }
        [DataMember]
        public new DateTime? LastModifiedOn { get => base.LastModifiedOn; set => base.LastModifiedOn = value; }
        [DataMember]
        public new string CreatedByUserId { get => base.CreatedByUserId; set => base.CreatedByUserId = value; }
        [DataMember]
        public new string LastModifiedByUserId { get => base.LastModifiedByUserId; set => base.LastModifiedByUserId = value; }
        [DataMember]
        public new string CreatedByUserName { get => base.CreatedByUserName; set => base.CreatedByUserName = value; }
        [DataMember]
        public new string LastModifiedByUserName { get => base.LastModifiedByUserName; set => base.LastModifiedByUserName = value; }
        #endregion
    }
}
