using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueDataImporter
{
    public static class ImporterExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((x, i) => (x, i));
        }
    }
}
