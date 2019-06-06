using UnityEngine;
using System.Collections.Generic;
using System;

namespace CocoPlay
{
	public static class CocoDataExtensions
	{
		#region Dictionary

		public static V GetValue<K, V> (this Dictionary<K, V> dict, K key)
		{
			V val;
			if (dict != null) {
				dict.TryGetValue (key, out val);
			} else {
				val = default(V);
			}
			return val;
		}

		public static V GetOrAddNewValue<K, V> (this Dictionary<K, V> dict, K key) where V : class, new()
		{
			if (dict == null) {
				return default(V);
			}

			if (dict.ContainsKey (key)) {
				return dict [key];
			}

			V val = new V ();
			dict.Add (key, val);
			return val;
		}

		public static V GetOrAddDefaultValue<K, V> (this Dictionary<K, V> dict, K key)
		{
			if (dict == null) {
				return default(V);
			}

			if (dict.ContainsKey (key)) {
				return dict [key];
			}

			V val = default(V);
			dict.Add (key, val);
			return val;
		}

		public static void ForEach<K, V> (this Dictionary<K, V> dict, System.Action<K, V> action)
		{
			if (dict == null || action == null) {
				return;
			}

			foreach (KeyValuePair<K, V> kvp in dict) {
				action (kvp.Key, kvp.Value);
			}
		}

		public static void ForEach<K, V> (this Dictionary<K, V> dict, System.Action<V> action)
		{
			if (dict == null || action == null) {
				return;
			}

			foreach (KeyValuePair<K, V> kvp in dict) {
				action (kvp.Value);
			}
		}

		#endregion


		#region Enum

		public static bool ToEnum<TEnum> (this string value, out TEnum result) where TEnum : struct
		{
			bool success = true;
			try {
				result = (TEnum)Enum.Parse (typeof(TEnum), value);
			} catch (Exception ex) {
				result = default (TEnum);
				success = false;
			}
			return success;
		}

		#endregion
	}
}