using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	/// <summary>
	/// This class inherits from Dictionary, and adds an auto-add feature - 
	/// if a key doest not exist, it is automatically created.
	/// </summary>
	public class AutoDictionary<TKey, TValue> : Dictionary<TKey, TValue>
		where TValue : new()
	{
		System.Func<TKey, TValue> _factory;

		public AutoDictionary(System.Func<TKey, TValue> factory)
		{
			_factory = factory;
		}

		public AutoDictionary()
			: this(key => new TValue())
		{
		}

		public new TValue this[TKey key]
		{
			set { base[key] = value;}
			get
			{
				TValue value;
				if(base.TryGetValue(key, out value))
					return value;

				base[key] = _factory(key);

				return value;
			}
		}
	}
}
