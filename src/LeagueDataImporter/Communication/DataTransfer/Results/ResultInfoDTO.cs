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

using iRLeagueDatabase.DataTransfer.Sessions;

namespace iRLeagueDatabase.DataTransfer.Results
{
    [DataContract]
    [KnownType(typeof(ResultDataDTO))]
    [KnownType(typeof(ScoredResultDataDTO))]
    public class ResultInfoDTO : VersionInfoDTO, IMappableDTO
    {
        [DataMember]
        public long? ResultId { get; set; }
        [DataMember]
        //public SessionInfoDTO Session { get; set; }
        public long SessionId { get; set; }

        public override object MappingId => ResultId;

        public override object[] Keys => new object[] { ResultId };
        //object IMappableDTO.MappingId => ResultId;
    }
}
