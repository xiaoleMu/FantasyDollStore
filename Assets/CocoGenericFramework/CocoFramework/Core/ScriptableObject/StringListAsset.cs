using UnityEngine;
using System.Collections;

namespace TabTale
{
	[System.Serializable]
	public class StringListAsset : ScriptableObject
	{
		public string[] strings;

#if UNITY_EDITOR
		
		[UnityEditor.MenuItem("Assets/Create/String List")]
		public static void CreateStringListAsset()
		{
			AssetUtils.CreateAsset<StringListAsset>();
		}
		
#endif
	}
}
