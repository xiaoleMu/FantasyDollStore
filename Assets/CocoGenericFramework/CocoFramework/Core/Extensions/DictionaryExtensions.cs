using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public static class DictionaryExtensions
	{
		public static IDictionary<TKey, TValue> Minus<TKey, TValue>(this IDictionary<TKey, TValue> _this, IDictionary<TKey, TValue> subtrahend)
		{
			IDictionary<TKey, TValue> difference = new Dictionary<TKey, TValue>();

			foreach(KeyValuePair<TKey, TValue> kvp in _this)
			{
				TValue otherValue;
				if(!subtrahend.TryGetValue(kvp.Key, out otherValue))
				{
					difference[kvp.Key] = kvp.Value;
				}
			}

			return difference;
		}
	}
}
