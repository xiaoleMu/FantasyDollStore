using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public abstract class Cache<TKey, TValue> : ICache<TKey, TValue>
		where TKey : class
		where TValue : class
	{
		protected Cache(System.Func<TKey, TValue> factory)
			: this(factory, v => {})
		{
		}

		protected Cache(System.Func<TKey, TValue> factory, 
		             	System.Action<TValue> destroyer, int maxSize)
		{			
			_factory = factory;
			_destroyer = destroyer;
			_maxSize = maxSize;
		}

		public static int DefaultMaxSize = 20;

		protected Cache(System.Func<TKey, TValue> factory, 
		                System.Action<TValue> destroyer)
				: this(factory, destroyer, DefaultMaxSize)
		{
		}

		public void Add(TKey key)
		{
			if(!_data.ContainsKey(key))
			{
				if(_data.Count >= _maxSize)
				{
					CollectGarbage();
				}
			} else
			{
				return;
			}
			
			TValue value = _factory(key);
			_data[key] = new CacheEntry(value);
		}

		public void Add(TKey key, TValue value)
		{
			if(!_data.ContainsKey(key))
			{
				if(_data.Count >= _maxSize)
				{
					CollectGarbage();
				}
			} else
			{
				return;
			}
			
			_data[key] = new CacheEntry(value);
		}

		protected class CacheEntry
		{
			TValue _value;
			int _refCount = 0;
			public TValue Value
			{
				get { return _value; }
			}

			int _requestCount;
			public int RequestCount
			{
				get { return _requestCount; }
			}

			public void IncRequestCount()
			{
				_requestCount++;				
			}

			public void Retain()
			{
				_refCount++;
			}

			public bool ReadyForGabage
			{
				get { return _refCount == 0; }
			}

			public void Release()
			{
				_refCount--;
			}

			float _creationTime;

			public CacheEntry(TValue value)
			{
				_value = value;
				_requestCount = 1;
				_creationTime = Time.time;
			}

			public float RequestFrequency
			{
				get { return (float)_requestCount / (Time.time - _creationTime); }
			}
		}

		protected IDictionary<TKey, CacheEntry> _data;

		int _maxSize = 20;
		public int MaxSize
		{
			get { return _maxSize; }
			set
			{
				_maxSize = value; 
			}
		}

		public void Release(TKey key)
		{
			CacheEntry value;
			if(_data.TryGetValue(key, out value))
				value.Release();
		}

		public void Release(TValue value)
		{
			foreach(CacheEntry entry in _data.Values.Where(e => e.Value == value))
			{
				entry.Release();
			}
		}

		public bool ContainsKey(TKey key)
		{
			return _data.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			CacheEntry entry;
			if(_data.TryGetValue(key, out entry))
			{
				value = entry.Value;
				return true;
			}
			
			value = default(TValue);
			return false;
		}

		void CollectGarbage()
		{
			float minFrequency = float.MaxValue;
			KeyValuePair<TKey, CacheEntry> toRemove = new KeyValuePair<TKey, CacheEntry>(default(TKey), null);
			
			//note: not using Linq because of Jit problems
			foreach(KeyValuePair<TKey, CacheEntry> kvp in _data)
			{
				CacheEntry cacheEntry = kvp.Value;
				if(cacheEntry.ReadyForGabage)
				{
					if(cacheEntry.RequestFrequency < minFrequency)
					{
						minFrequency = kvp.Value.RequestFrequency;
						toRemove = kvp;
					}
				}				
            }
            
            if(toRemove.Key != default(TKey))
            {
				if(toRemove.Value.Value != default(TValue))
					_destroyer(toRemove.Value.Value);

                _data.Remove(toRemove);
            }
		}

		public TValue Get(TKey key, bool retain = true)
		{
			CacheEntry entry;
			if(_data.TryGetValue(key, out entry))
			{
				entry.IncRequestCount();
				if(retain)
					entry.Retain();
				return entry.Value;
			}
			
			if(_data.Count >= _maxSize)
			{
				CollectGarbage();
			}
			
			TValue value = _factory(key);
			_data[key] = new CacheEntry(value);
			
			return value;
		}	

		System.Func<TKey, TValue> _factory;
		System.Action<TValue> _destroyer;
	}

	public class MapCache<TKey, TValue> : Cache<TKey, TValue>
		where TKey : class
		where TValue : class
	{
		public MapCache(System.Func<TKey, TValue> factory)
			: this(factory, v => {})
		{
			_data = new Dictionary<TKey, CacheEntry>();
		}
		
		public MapCache(System.Func<TKey, TValue> factory, 
		             	System.Action<TValue> destroyer, int maxSize)
			: base(factory, destroyer, maxSize)
		{
			_data = new Dictionary<TKey, CacheEntry>();
		}

		public MapCache(System.Func<TKey, TValue> factory, 
		                System.Action<TValue> destroyer)
				: this(factory, destroyer, DefaultMaxSize)
		{
		}
	}

	public class SortedCache<TKey, TValue> : Cache<TKey, TValue>
		where TKey : class
		where TValue : class
	{
		public SortedCache(System.Func<TKey, TValue> factory)
			: this(factory, v => {})
		{
			_data = new SortedDictionary<TKey, CacheEntry>();
		}
		
		public SortedCache(System.Func<TKey, TValue> factory, 
	                	   System.Action<TValue> destroyer, int maxSize)
			: base(factory, destroyer, maxSize)
		{
			_data = new SortedDictionary<TKey, CacheEntry>();
		}

		public SortedCache(System.Func<TKey, TValue> factory, 
		                   System.Action<TValue> destroyer)
			: this(factory, destroyer, DefaultMaxSize)
		{
		}
	}
}
