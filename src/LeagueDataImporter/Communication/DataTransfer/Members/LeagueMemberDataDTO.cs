﻿// MIT License

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
using System.Xml.Serialization;
using iRLeagueDatabase.DataTransfer.Reviews;
using System.Runtime.Serialization;

namespace iRLeagueDatabase.DataTransfer.Members
{
    /// <summary>
    /// This class manages information considering the member's iracing user profile
    /// </summary>
    [DataContract(Name="LeagueMemberDataDTO")]
    [KnownType(typeof(LeagueMemberInfoDTO))]
    public class LeagueMemberDataDTO : LeagueMemberInfoDTO, IMappableDTO
    {
        //[DataMember]
        //public int MemberId { get; set; }
        [DataMember]
        public string Firstname { get; set; }
        [DataMember]
        public string Lastname { get; set; }
        [DataMember]
        public string IRacingId { get; set; }
        [DataMember]
        public string DanLisaId { get; set; }
        [DataMember]
        public string DiscordId { get; set; }
        [DataMember]
        public TeamDataDTO Team { get; set; }

        //[DataMember]
        //public LeagueMemberInfoDTO CreatedBy { get; set; }
        //[DataMember]
        //public LeagueMemberInfoDTO LastModifiedBy { get; set; }

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

        public LeagueMemberDataDTO() { }
    }
}
