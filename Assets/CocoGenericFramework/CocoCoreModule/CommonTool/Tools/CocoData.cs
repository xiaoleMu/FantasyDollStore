using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Random = UnityEngine.Random;

namespace CocoPlay
{
	public class CocoData
	{
		#region Create List/Array

		public static T[] CreateArray<T> (int count, T value)
		{
			T[] array = new T[count];
			for (int i = 0; i < count; i++)
				array [i] = value;
			return array;
		}

		public static List<T> CreateList<T> (int count, T value)
		{
			return new List<T> (CreateArray (count, value));
		}

		#endregion


		#region Dictionary

		public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue> (IList<TValue> values, Func<TValue, TKey> keyFunc)
		{
			if (values == null) {
				return null;
			}

			var dictionary = new Dictionary<TKey, TValue> (values.Count);
			foreach (var value in values) {
				var key = keyFunc (value);
				if (dictionary.ContainsKey (key)) {
					continue;
				}
				dictionary.Add (key, value);
			}

			return dictionary;
		}

		#endregion


		#region Random

		public static T GetRandomItem<T> (IList<T> list)
		{
			if (list == null || list.Count <= 0) {
				return default(T);
			}

			return list [Random.Range (0, list.Count)];
		}

		#endregion


		#region Persistent

		public static T LoadFromJsonFile<T> (string fullPath)
		{
			if (!File.Exists (fullPath)) {
				return default(T);
			}

			var json = File.ReadAllText (fullPath);
			return JsonMapper.ToObject<T> (json);
		}

		public static bool SaveToJsonFile (object obj, string fullPath, bool prettyPrint = false)
		{
			var writer = new JsonWriter { PrettyPrint = prettyPrint };

			JsonMapper.ToJson (obj, writer);
			var json = writer.ToString ();
			if (string.IsNullOrEmpty (json)) {
				Debug.LogErrorFormat ("CocoData->SaveToJsonFile: object [{0}<{1}>] convert to json failed!", fullPath, obj.GetType ().Name);
				return false;
			}

			if (File.Exists (fullPath)) {
				File.Delete (fullPath);
			}

			var directory = Path.GetDirectoryName (fullPath);
			if (!string.IsNullOrEmpty (directory) && !Directory.Exists (directory)) {
				Directory.CreateDirectory (directory);
			}

			File.WriteAllText (fullPath, json);
			return true;
		}

		public static T LoadFromJsonResource<T> (string path)
		{
			var jsonData = Resources.Load<TextAsset> (path);
			if (jsonData == null) {
				return default(T);
			}

			var data = JsonMapper.ToObject<T> (jsonData.text);
			Resources.UnloadAsset (jsonData);

			return data;
		}

		#endregion
	}
}