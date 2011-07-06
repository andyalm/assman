using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
{
    public static class EnumerableExtensions
    {
        public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, IEnumerable<KeyValuePair<TKey,TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                dictionary.Add(pair);
            }
        }

        public static void AddRange<T>(this ICollection<T> set, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                set.Add(value);
            }
        }

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> collection, Comparison<T> comparison)
        {
            var list = new List<T>(collection);
            list.Sort(comparison);

            return list;
        }

        /// <summary>
        /// Sorts the given collection of items while preserving the order of items that are equal
        /// </summary>
        public static IEnumerable<T> StableSort<T>(this IEnumerable<T> collection, Comparison<T> comparison)
        {
            //this uses an insertion sort algorithm.
            //implementation adapted from http://www.csharp411.com/c-stable-sort/
            
            var list = new List<T>(collection);
            int count = list.Count;
            for (int j = 1; j < count; j++)
            {
                T key = list[j];

                int i = j - 1;
                for (; i >= 0 && comparison(list[i], key) > 0; i--)
                {
                    list[i + 1] = list[i];
                }
                list[i + 1] = key;
            }

            return list;
        }

        public static bool HasAtLeast<T>(this IEnumerable<T> collection, int minimum)
        {
            if (collection is ICollection<T>)
                return ((ICollection<T>) collection).Count >= minimum;
            
            int i = 0;
            foreach (var item in collection)
            {
                i += 1;
                if (i >= minimum)
                    return true;
            }

            return false;
        }

        // this algorithm based on the stackoverflow question here: http://stackoverflow.com/questions/1982592/topological-sorting-using-linq
        public static IEnumerable<TSource> PartialOrderBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Comparison<TKey> comparer)
        {
            var values = source.ToArray();
            var keys = values.Select(keySelector).ToArray();
            int count = values.Length;
            var notYieldedIndexes = Enumerable.Range(0, count).ToArray();
            int valuesToGo = count;

            while (valuesToGo > 0)
            {
                //Start with first value not yielded yet
                int minIndex = notYieldedIndexes.First(i => i >= 0);

                //Find minimum value amongst the values not yielded yet
                for (int i = 0; i < count; i++)
                    if (notYieldedIndexes[i] >= 0)
                        if (comparer(keys[i], keys[minIndex]) < 0)
                        {
                            minIndex = i;
                        }

                //Yield minimum value and mark it as yielded
                yield return values[minIndex];
                notYieldedIndexes[minIndex] = -1;
                valuesToGo--;
            }
        }
    }
}