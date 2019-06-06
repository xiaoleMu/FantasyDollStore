using UnityEngine;
using System.Collections;
using LitJson;
namespace CocoPlay
{
	public static class CCJsonExtensions
	{
		static public bool ContainsKey(this JsonData data, string key)
		{
			if (data == null)
				return false;
			
			if (!data.IsObject)
				return false;
			
			IDictionary tdictionary = data as IDictionary;
			if (tdictionary == null)
				return false;

			return tdictionary.Contains(key);
		}
	}
}

