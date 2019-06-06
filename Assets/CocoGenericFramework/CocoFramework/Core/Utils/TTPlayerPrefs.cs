using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class TTPlayerPrefs 
	{
		public static void DeleteAll() {
			PlayerPrefs.DeleteAll();
		}
		
		public static void DeleteKey(string key) {
			PlayerPrefs.DeleteKey(key);
		}
		
		public static bool HasKey(string key) {
			return PlayerPrefs.HasKey(key);
		}
		

		public static void Save()
		{
			try {
				PlayerPrefs.Save();
			} 
			catch (PlayerPrefsException e)
			{
				Debug.Log (e);
			}
		}

		
		// INT
		public static void SetValue(string key, int value)
		{
			try {
				PlayerPrefs.SetInt(key, value);
			} 
			catch (PlayerPrefsException e)
			{
				Debug.Log (e);
			}
		}
		
		public static int GetValue(string key, int defaultv)
		{
			try {
			int value = PlayerPrefs.GetInt(key, defaultv);
			return value;
			}
			catch (PlayerPrefsException e)
			{
				Debug.Log (e);
			}
			return defaultv;
		}
		
		// BOOL
		public static void SetValue(string key, bool value)
		{
			int v = value ? 1 : 0;
			SetValue(key, v);
		}
		
		public static bool GetValue(string key, bool defaultv)
		{

			int i = defaultv ? 1 : 0;
			i = GetValue(key, i);
			return i == 1;
		}
		

		// FLOAT
		public static void SetValue(string key, float value)
		{
			try {
			PlayerPrefs.SetFloat(key, value);
			} 
			catch (PlayerPrefsException e)
			{
				Debug.Log (e);
			}
		}
		
		public static float GetValue(string key, float defaultv)
		{
			try {
			float value = PlayerPrefs.GetFloat(key, defaultv);
		
			return value;
			}
			catch (PlayerPrefsException e)
			{
				Debug.Log (e);
			}
			return defaultv;
		}
		
		// STRING
		public static void SetValue(string key, string value)
		{
			try {
			PlayerPrefs.SetString(key, value);
			} 
			catch (PlayerPrefsException e)
			{
				Debug.Log (e);
			}
		}
		
		public static string GetValue(string key, string defaultv)
		{
			try {
				string value = PlayerPrefs.GetString(key, defaultv);
				return value;
			}
			catch (PlayerPrefsException e)
			{
				Debug.Log (e);
			}
			return defaultv;
		}
		
		
	}
}