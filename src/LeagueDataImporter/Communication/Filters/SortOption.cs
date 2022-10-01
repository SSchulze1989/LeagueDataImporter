using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class SortOption
    { 
        /// <summary>
        /// Option which value to use for order
        /// </summary>
        public SortOptionEnum Option { get; set; }
        /// <summary>
        /// + 1 for ascending, -1 for descending
        /// </summary>
        public int SortDirection { get; set; }

        public SortOption(SortOptionEnum option, int sortDirection)
        {
            Option = option;
            SortDirection = sortDirection;
        }
    }
}
