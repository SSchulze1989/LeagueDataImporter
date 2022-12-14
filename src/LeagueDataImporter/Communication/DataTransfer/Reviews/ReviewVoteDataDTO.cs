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
using iRLeagueManager.Enums;
using iRLeagueDatabase.DataTransfer.Members;
using System.Runtime.Serialization;

namespace iRLeagueDatabase.DataTransfer.Reviews
{
    [DataContract]
    public class ReviewVoteDataDTO : MappableDTO
    {
        [DataMember]
        public long ReviewVoteId { get; set; }
        [DataMember]
        public VoteEnum Vote { get; set; }
        [DataMember]
        //public LeagueMemberInfoDTO MemberAtFault { get; set; }
        public long? MemberAtFaultId { get; set; }
        [DataMember]
        public long? VoteCategoryId { get; set; }
        [DataMember]
        public string CatText { get; set; }
        [DataMember]
        public int CatPenalty { get; set; }
        [DataMember]
        public string Description { get; set; }

        public override object MappingId => ReviewVoteId;
        public override object[] Keys => new object[] { ReviewVoteId };
    }
}
