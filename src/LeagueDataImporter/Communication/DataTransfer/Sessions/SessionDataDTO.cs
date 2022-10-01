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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using iRLeagueManager.Enums;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Sessions.Convenience;

namespace iRLeagueDatabase.DataTransfer.Sessions
{
    /// <summary>
    /// Base type for league sessions.
    /// </summary>
    [Serializable()]
    [DataContract]
    [KnownType(typeof(SeasonInfoDTO))]
    [KnownType(typeof(RaceSessionDataDTO))]
    [KnownType(typeof(SessionConvenienceDTO))]
    [KnownType(typeof(RaceSessionConvenienceDTO))]
    public class SessionDataDTO : SessionInfoDTO
    {
        //[DataMember]
        //public int SessionId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        //public ScheduleInfoDTO Schedule { get; set; }
        public long? ScheduleId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        //public ResultInfoDTO SessionResult { get; set; }
        public long? SessionResultId { get; set; }

        [DataMember]
        public long? ParentSessionId { get; set; }

        [DataMember]
        //public IncidentReviewInfoDTO[] Reviews { get; set; }
        public long[] ReviewIds { get; set; }

        [DataMember]
        /// <summary>
        /// Date of the session.
        /// </summary>
        public DateTime Date { get; set; }

        [DataMember]
        /// <summary>
        /// Id of the track and track-config for the session.
        /// </summary>
        public string LocationId { get; set; }

        [DataMember]
        /// <summary>
        /// Duration of the session. In case of a race with attached qualy, this also includes the times of free practice and qualifiying.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Ids of sub sessions e.g: for single heats or with multi race events
        /// </summary>
        [DataMember]
        public SessionDataDTO[] SubSessions { get; set; }

        [DataMember]
        public int SubSessionNr { get; set; }

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
