using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;


[InitializeOnLoad]
public static class PsdkSerializedDataEditor {

	static PsdkSerializedDataEditor() {
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


	static void CreateMySeriliazbleDataObject() {

		bool created = false;
		//AssetDatabase.DeleteAsset(PsdkSerializedData.ASSET_PATH);
		PsdkSerializedData asset =  AssetDatabase.LoadAssetAtPath(PsdkSerializedData.ASSET_PATH,typeof(PsdkSerializedData)) as PsdkSerializedData;
		if (asset == null) { // Didn't found asset, create one
			asset = PsdkSerializedData.CreateInstance<PsdkSerializedData>();
			if (asset == null) return;
			created = true;
		}
		asset.unityVersion = Application.unityVersion;
		asset.productName = PlayerSettings.productName;
		asset.bundleIdentifier = PsdkUtils.BundleIdentifier;
		asset.bundleVersion = PlayerSettings.bundleVersion;
		asset.androidBundleVersionCode = PlayerSettings.Android.bundleVersionCode.ToString();
		string[] versionsStrings = psdkVersions().Split(new char[]{','});
		asset.psdkVersion = "";
		for( int i=0; i < versionsStrings.Length; ++i) {
			if (i > 0) asset.psdkVersion += ",";
			asset.psdkVersion += versionsStrings[i];
		}
		asset.psdkVersion += "\n";

		asset.unityScriptingBackend ="";
#if UNITY_IOS
		ScriptingImplementation backend = (ScriptingImplementation)PlayerSettings.GetPropertyInt("ScriptingBackend", BuildTargetGroup.iOS);
		asset.unityScriptingBackend = backend.ToString();
#endif


		asset.buildTime = System.DateTime.Now.ToString();
		if (created) {
			string dir = System.IO.Path.GetDirectoryName (PsdkSerializedData.ASSET_PATH);
			string parentDir = System.IO.Path.GetDirectoryName (dir);
			if (! System.IO.Directory.Exists (dir)) {
				AssetDatabase.CreateFolder (parentDir, "Resources");
			}
			AssetDatabase.CreateAsset (asset, PsdkSerializedData.ASSET_PATH);
		} else {
			EditorUtility.SetDirty(asset);
			//AssetDatabase.SaveAssets ();
		}
	}

	static string psdkVersions() {
		string versions = "";
		try {
			string versionsFile = "psdk";
			versionsFile = System.IO.Path.Combine(versionsFile,"versions");
			versionsFile = System.IO.Path.Combine(versionsFile,"versions.txt");
			versions = PsdkUtils.ReadStreamingAssetsFile(versionsFile);
		}
		catch (System.Exception e) {
			Debug.LogException(e);
		}
		return versions;
	}
	
	

}
