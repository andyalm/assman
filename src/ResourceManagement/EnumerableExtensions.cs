using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
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
	}
}