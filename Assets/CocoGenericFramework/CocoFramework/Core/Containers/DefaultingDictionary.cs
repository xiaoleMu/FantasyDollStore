using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	/// <summary>
	/// This is a dictionary which plays nicely with missing keys - instead of raising
	/// an exception when a missing key is provided, it returns the default value
	/// which was provided in the constructor. If no such value was provided, the default
	/// defaults (no pun intended) to the language's default(TValue).
	/// </summary>
	public class DefaultingDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		TValue _default;

		public DefaultingDictionary(TValue defaultValue = default(TValue))
			: base()
		{
			_default = defaultValue;
		}

		public new TValue this[TKey key]
		{
			get
			{
				TValue value;
				if(!TryGetValue(key, out value))
					return _default;

				return value;
			}

			set { base[key] = value; }
		}

		public bool AddIfMissing(TKey key, TValue value)
		{
			if(base.ContainsKey(key))
				return false;

			base[key] = value;
			return true;
		}

		public bool AddIfMissing(KeyValuePair<TKey, TValue> pair)
		{
			return AddIfMissing(pair.Key, pair.Value);
		}

		public TValue Default
		{
			get { return _default; }
			set { _default = value; }
		}
	}

	public class DictionaryMapper<TKey, TValue> : Dictionary<TKey, TValue>
	{
		System.Func<TKey, TValue> _mapper = k => default(TValue);

		public DictionaryMapper(System.Func<TKey, TValue> mapper = null)
		{
			if(mapper != null)
				_mapper = mapper;
		}

		public new TValue this[TKey key]
		{
			get
			{
				TValue value;
				if(!TryGetValue(key, out value))
					return _mapper(key);
				
				return value;
			}
			
			set { base[key] = value; }
		}
	}

	public class CachedMapper<TKey, TValue> : Dictionary<TKey, TValue>
	{
		System.Func<TKey, TValue> _mapper = k => default(TValue);
		
		public CachedMapper(System.Func<TKey, TValue> mapper = null)
		{
			if(mapper != null)
				_mapper = mapper;
		}
		
		public new TValue this[TKey key]
		{
			get
			{
				TValue cachedValue;
				if(!TryGetValue(key, out cachedValue))
				{
					TValue newValue = _mapper(key);
					base[key] = newValue;
					return newValue;
				}
				
				return cachedValue;
			}
			
			set { base[key] = value; }
		}
	}
}
