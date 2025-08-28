using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.Utils
{
    internal static class LinqExtensions
    {
        public static IEnumerable<TResult> FullOuterJoinJoin<TSource, TInner, TKey, TResult>(
            this IEnumerable<TSource> source,
            IEnumerable<TInner> inner,
            Func<TSource, TKey> selectKeyA,
            Func<TInner, TKey> selectKeyB,
            Func<TSource, TInner, TKey, TResult> result,
            TSource defaultA = default(TSource),
            TInner defaultB = default(TInner),
            IEqualityComparer<TKey> cmp = null)
        where TSource : class where TInner : class
        {
            cmp = cmp ?? EqualityComparer<TKey>.Default;
            var alookup = source.ToLookup(selectKeyA, cmp);
            var blookup = inner.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TKey>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       from xa in alookup[key].DefaultIfEmpty(defaultA)
                       from xb in blookup[key].DefaultIfEmpty(defaultB)
                       select result(xa, xb, key);
            return join.ToList();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
