using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;

namespace TabTale.PSDK.Editor {

	[InitializeOnLoad]
	public static class PsdkSettingsEditor {

		static PsdkSettingsEditor() {
			didReloadScripts ();
		}

		
		[PostProcessBuild(40000)]
		public static void OnPostProcessBuild( BuildTarget target, string path )
		{
			didReloadScripts ();
		}

		[DidReloadScripts]
		static void didReloadScripts() {
			CreateMySeriliazbleDataObject ();
		}
		
		static PsdkSettingsData CreateMySeriliazbleDataObject() {
			//AssetDatabase.DeleteAsset(PsdkSerializedData.ASSET_PATH);
			PsdkSettingsData asset =  AssetDatabase.LoadAssetAtPath(PsdkSettingsData.ASSET_PATH,typeof(PsdkSettingsData)) as PsdkSettingsData;
			if (asset == null) { // Didn't found asset, create one
				asset = PsdkSettingsData.CreateInstance<PsdkSettingsData>();
				if (asset == null) return null;
				string dir = System.IO.Path.GetDirectoryName (PsdkSettingsData.ASSET_PATH);
				string parentDir = System.IO.Path.GetDirectoryName (dir);
				if (! System.IO.Directory.Exists (dir)) {
					AssetDatabase.CreateFolder (parentDir, "Resources");
				}
				AssetDatabase.CreateAsset (asset, PsdkSettingsData.ASSET_PATH);
			}
			return asset;
		}

		[MenuItem("TabTale/PSDK/Settings")]
		static void ShowPsdkSettings() {
			PsdkSettingsData asset = CreateMySeriliazbleDataObject ();
			Selection.activeObject = asset;
		}
	}
}
