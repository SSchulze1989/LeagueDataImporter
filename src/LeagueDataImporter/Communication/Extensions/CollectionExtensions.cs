using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Extensions
{
    public static class CollectionExtensions
    {
        public static void MapCollection<T>(this ICollection<T> collecction, IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            collecction.Clear();
            foreach(var item in source)
            {
                collecction.Add(item);
            }
        }
    }
}
