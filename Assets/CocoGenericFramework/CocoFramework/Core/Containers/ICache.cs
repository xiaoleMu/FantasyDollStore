using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface ICache<TKey, TValue>
		where TKey : class
		where TValue : class
	{
		void Add(TKey key);
		void Add(TKey key, TValue value);

		TValue Get(TKey key, bool retain = true);
		void Release(TKey key);
		void Release(TValue value);

		bool ContainsKey(TKey key);
		bool TryGetValue(TKey key, out TValue value);
	}
}
