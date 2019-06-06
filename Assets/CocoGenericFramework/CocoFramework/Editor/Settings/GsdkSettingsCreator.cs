using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;
using System.IO;

namespace TabTale.Editor
{
	[InitializeOnLoad]
	//[CustomEditor(typeof(GsdkSettingsData))]
	public static class GsdkSettingsCreator
	{
		static GsdkSettingsCreator() 
		{
			didReloadScripts ();
		}


		[PostProcessBuild(39000)]
		public static void OnPostProcessBuild( BuildTarget target, string path )
		{
			didReloadScripts ();
		}

		
		[DidReloadScripts]
		static void didReloadScripts() 
		{
			//CreateMySerializableObject ();
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("Assets/Create/GSDK Settings")]
		#endif
		static GsdkSettingsData CreateMySerializableObject() 
		{
			var asset = Resources.Load("GsdkSettingsData") as GsdkSettingsData;

			if (asset == null) 
			{	
				Debug.Log ("Cannot find gsdk settings object - creating it...");
				asset = ScriptableObject.CreateInstance<GsdkSettingsData> ();

				string dir = System.IO.Path.GetDirectoryName (GsdkSettingsData.GAME_SPECIFIC_PATH);
				string parentDir = System.IO.Path.GetDirectoryName (dir);
				if (! System.IO.Directory.Exists (dir))
				{
					AssetDatabase.CreateFolder (parentDir, "TabTale");
				}

				dir = System.IO.Path.GetFullPath(GsdkSettingsData.GAME_SPECIFIC_RES);
				
				if (! System.IO.Directory.Exists (dir))
				{
					AssetDatabase.CreateFolder (GsdkSettingsData.GAME_SPECIFIC_PATH, "Resources");
				}

				AssetDatabase.CreateAsset(asset, GsdkSettingsData.GAME_SPECIFIC_RES + "/GsdkSettingsData" + ".asset");
				
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				Selection.activeObject = asset;
			}
			return asset;
		}
		
		[MenuItem("TabTale/GSDK/Settings &g")]
		static void ShowGsdkSettings() 
		{
			GsdkSettingsData asset = CreateMySerializableObject ();
			Selection.activeObject = asset;
		}
	}
}
