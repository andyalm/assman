using System;
using System.Collections.Generic;
using System.Threading;

namespace Assman
{
    internal interface IThreadSafeInMemoryCache<TKey, TValue>
    {
        int Count { get; }
        TValue Get(TKey key);
        TValue GetOrAdd(TKey key, Func<TValue> getValue);
        void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs);
    }

    /// <summary>
	/// This class is intended to be used when you are accumulating a cache of stuff that can be accessed by multiple threads.
	/// It wraps a Dictionary, but is thread safe.  I didn't make it implement IDictionary because IDictionary's interface
	/// is pretty broad and that would be more work (YAGNI).
	/// </summary>
	/// <remarks>
	/// When we move to .NET 4, we can replace this with a ConcurrentDictionary.
	/// </remarks>
	internal class ThreadSafeInMemoryCache<TKey, TValue> : IThreadSafeInMemoryCache<TKey, TValue>
    {
		private readonly IDictionary<TKey, TValue> _dictionary;
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		public ThreadSafeInMemoryCache()
		{
			_dictionary = new Dictionary<TKey, TValue>();
		}

		public ThreadSafeInMemoryCache(IEqualityComparer<TKey> comparer)
		{
			_dictionary = new Dictionary<TKey, TValue>(comparer);
		}

		public int Count
		{
			get
			{
				using (_lock.ReadLock())
				{
					return _dictionary.Count;
				}
			}
		}

		public TValue Get(TKey key)
		{
			using (_lock.ReadLock())
			{
				return _dictionary[key];
			}
		}

		public TValue GetOrAdd(TKey key, Func<TValue> getValue)
		{
			//this double locking pattern was adapted from a blog post by Ayende
			//http://ayende.com/Blog/archive/2010/01/04/using-readerwriterlockslimrsquos-enterupgradeablereadlock.aspx
			using (_lock.ReadLock())
			{
				TValue value;
				if (_dictionary.TryGetValue(key, out value))
					return value;
			}
			using (_lock.WriteLock())
			{
				TValue value;
				if (_dictionary.TryGetValue(key, out value))
					return value;

				value = getValue();
				_dictionary[key] = value;

				return value;
			}
		}

		public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
		{
			using(_lock.WriteLock())
			{
				foreach (var pair in pairs)
				{
					_dictionary.Add(pair);
				}
			}
		}
	}

	internal static class ReaderWriterLockExtensions
	{
		public static IDisposable ReadLock(this ReaderWriterLockSlim @lock)
		{
			@lock.EnterReadLock();

			return new DisposableAction(@lock.ExitReadLock);
		}

		public static IDisposable WriteLock(this ReaderWriterLockSlim @lock)
		{
			@lock.EnterWriteLock();

			return new DisposableAction(@lock.ExitWriteLock);
		}

		private class DisposableAction : IDisposable
		{
			private readonly Action _onDispose;

			public DisposableAction(Action onDispose)
			{
				_onDispose = onDispose;
			}

			public void Dispose()
			{
				_onDispose();
			}
		}
	}

    public class NullThreadSafeInMemoryCache<TKey,TValue> : IThreadSafeInMemoryCache<TKey, TValue>
    {
        public NullThreadSafeInMemoryCache(){}
        
        public int Count
        {
            get { return 0; }
        }

        public TValue Get(TKey key)
        {
            return default(TValue);
        }

        public TValue GetOrAdd(TKey key, Func<TValue> getValue)
        {
            return getValue();
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs) {}
    }
}