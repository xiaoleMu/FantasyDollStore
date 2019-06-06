using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;

namespace TabTale.PSDK.Editor {

	[InitializeOnLoad]
	public static class PsdkInternalSettingsEditor {

		static PsdkInternalSettingsEditor() {
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
		
		static PsdkInternalSettingsData CreateMySeriliazbleDataObject() {
			//AssetDatabase.DeleteAsset(PsdkSerializedData.ASSET_PATH);
			PsdkInternalSettingsData asset =  AssetDatabase.LoadAssetAtPath(PsdkInternalSettingsData.ASSET_PATH,typeof(PsdkInternalSettingsData)) as PsdkInternalSettingsData;
			if (asset == null) { // Didn't found asset, create one
				asset = PsdkInternalSettingsData.CreateInstance<PsdkInternalSettingsData>();
				if (asset == null) return null;
				string dir = System.IO.Path.GetDirectoryName (PsdkInternalSettingsData.ASSET_PATH);
				string parentDir = System.IO.Path.GetDirectoryName (dir);
				if (! System.IO.Directory.Exists (dir)) {
					AssetDatabase.CreateFolder (parentDir, "Resources");
				}
				AssetDatabase.CreateAsset (asset, PsdkInternalSettingsData.ASSET_PATH);
			}
			if (! asset.socialGamePackage) {
				try {
					string psdkJson = PsdkUtils.ReadPsdkConfigFromFile ();
					IDictionary<string,object> dict = TabTale.Plugins.PSDK.Json.Deserialize (psdkJson) as IDictionary<string,object>;
					if (dict != null) {
						if (dict.ContainsKey ("socialGame") && (bool)(dict ["socialGame"] as IDictionary<string,object>) ["included"]) {
							UnityEngine.Debug.Log ("Updating PsdkInternalSettings.socialGame=true according to psdk.json");
							asset.socialGamePackage = true;
						}
					} else {
						Debug.LogError ("PSDK json dict is null, no updating PsdkInternalSettings.socialGame");
					}
				} catch (System.Exception e) {
					Debug.LogException (e);
				}
			}	

			return asset;
		}

	}
}
