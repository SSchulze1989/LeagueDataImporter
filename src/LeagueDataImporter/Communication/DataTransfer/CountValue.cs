using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer
{
    /// <summary>
    /// Helper class to provide an easy target for countable items.
    /// For example: count of vote categories for each vote
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class CountValue<T> : BaseDTO
    {
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public T Value { get; set; }
    }
}
