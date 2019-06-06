#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using System.Text;
using System.IO;
using System.Diagnostics;
using TabTale.Plugins.PSDK;
using PSDK.UnityEditor.XCodeEditor;


namespace Tabtale.PSDK.Editor {

public static class PsdkUpdateiOSTeamIdPostProcess {


	static string filePath;
	static string projectRootPath;


		[MenuItem("TabTale/PSDK/iOS Team enabling")]
		static void MainMenuCall() {

			string path = Path.Combine (Application.dataPath, ".tmp");
			InjectingTeamIdToPbxProjFile (path);

		}
		

	[PostProcessBuild(40002)]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		filePath = path;
		projectRootPath = path;

		if( !System.IO.Directory.Exists( filePath ) ) {
			UnityEngine.Debug.LogError( "Path does not exists." + path);
			return;
		}
		
		if( filePath.EndsWith( ".xcodeproj" ) ) {
			UnityEngine.Debug.Log( "Opening project " + filePath );
			projectRootPath = Path.GetDirectoryName( filePath );
			filePath = filePath;
		} else {
			UnityEngine.Debug.Log( "Looking for xcodeproj files in " + filePath );
			string[] projects = System.IO.Directory.GetDirectories( filePath, "*.xcodeproj" );
			if( projects.Length == 0 ) {
				UnityEngine.Debug.LogWarning( "Error: missing xcodeproj file" );
				return;
			}
			
			projectRootPath = filePath;
			filePath = projects[ 0 ];	
		}
		
		// Convert to absolute
		projectRootPath = Path.GetFullPath(projectRootPath);
		
		string projectFileInfo = Path.Combine( filePath, "project.pbxproj" );
			InjectingTeamIdToPbxProjFile (path);
	}


	static void InjectingTeamIdToPbxProjFile(string file) {
		
			UnityEngine.Debug.Log ("InjectingTeamIdToPbxProjFile start");

			XCProject project = new XCProject( file );
			if (project == null) {
				UnityEngine.Debug.LogError("Didn't manage to open project.pbxproj at " + file);
				return;
			}

			string teamId = null;
			try {
				teamId = PsdkiOSPostProcessUtils.GetTeamIdFromMobileProvision ();
			}
			catch (System.Exception e) {
					UnityEngine.Debug.LogException(e);
			}

			if (teamId == null) {
				UnityEngine.Debug.LogError ("NULL iOS  TeamIdentifier !!");
				if (UnityEditorInternal.InternalEditorUtility.inBatchMode) {
					EditorApplication.Exit (-1);
				}
			}

			string targetGuid = null;
			foreach (var nt in project.nativeTargets) {
				targetGuid = nt.Key;
				break;
			}
			string rootObjectGuid = project.rootGroup.guid;
			
			PBXDictionary addedFile = project.AddFile ("Unity-iPhone/tabtale.entitlements",null,"SDKROOT",false,true);
			foreach (PBXFileReference fr in addedFile.Values) {
				if (fr.ContainsKey ("sourceTree")) {
					fr.data["sourceTree"] = "<group>";
				}
			}
			UnityEngine.Debug.Log("targetguid: " + targetGuid);
			UnityEngine.Debug.Log("root Object guid: " + rootObjectGuid);
			PBXDictionary rootDict = project.GetObject (rootObjectGuid) as PBXDictionary;
			if (rootDict == null) {
				UnityEngine.Debug.LogError("null rootDict");
				if (UnityEditorInternal.InternalEditorUtility.inBatchMode) {
					EditorApplication.Exit (-1);
				}
				return;
			}
			if (! rootDict.ContainsKey ("attributes")) {
				UnityEngine.Debug.LogError("root dict does not contain attributes !");
				if (UnityEditorInternal.InternalEditorUtility.inBatchMode) {
					EditorApplication.Exit (-1);
				}
				return;
			}
			PBXDictionary attributes = rootDict["attributes"] as PBXDictionary;
			if (! attributes.ContainsKey ("TargetAttributes")) {
				attributes.Add("TargetAttributes",new PBXDictionary());
			}
			PBXDictionary targetAttributes = attributes["TargetAttributes"] as PBXDictionary;
			if (! targetAttributes.ContainsKey (targetGuid)) {
				targetAttributes.Add(targetGuid,new PBXDictionary());
			}
			PBXDictionary targetGuidScope = targetAttributes[targetGuid] as PBXDictionary;
			if (! targetGuidScope.ContainsKey ("DevelopmentTeam")) {
				targetGuidScope.Add("DevelopmentTeam",teamId);
			}
			else {
				targetGuidScope["DevelopmentTeam"] = teamId;
			}

			if (! targetGuidScope.ContainsKey ("SystemCapabilities")) {
				targetGuidScope.Add("SystemCapabilities",new PBXDictionary());
			}
			PBXDictionary systemCapabilities = targetGuidScope["SystemCapabilities"] as PBXDictionary;
			if (! systemCapabilities.ContainsKey ("com.apple.Keychain")) {
				systemCapabilities.Add("com.apple.Keychain", new PBXDictionary());
			}
			PBXDictionary appleKeychain = systemCapabilities["com.apple.Keychain"] as PBXDictionary;
			if (! appleKeychain.ContainsKey ("enabled"))
				appleKeychain.Add ("enabled", 1);
			else
				appleKeychain ["enabled"] = 1;
			
			string jsonPath = System.IO.Path.Combine(Application.streamingAssetsPath, "psdk_ios.json");
			if(File.Exists(jsonPath)){
				string jsonStr = System.IO.File.ReadAllText(jsonPath);
				Dictionary<string,object> json = (Dictionary<string,object>)TabTale.Plugins.PSDK.Json.Deserialize(jsonStr);
				if(json.ContainsKey("crossDevicePersistency")){
					Dictionary<string,object> cdpDict = (Dictionary<string,object>)json["crossDevicePersistency"];
					if(cdpDict.ContainsKey("included")){
						if((bool)cdpDict["included"]){
							if (! systemCapabilities.ContainsKey ("com.apple.iCloud")) {
								systemCapabilities.Add("com.apple.iCloud", new PBXDictionary());
							}
							PBXDictionary appleICloud = systemCapabilities["com.apple.iCloud"] as PBXDictionary;
							if (! appleICloud.ContainsKey ("enabled"))
								appleICloud.Add ("enabled", 1);
							else
								appleICloud ["enabled"] = 1;

							string strToAdd = "<key>com.apple.developer.icloud-container-identifiers</key>\n\t<array/>\n\t<key>com.apple.developer.ubiquity-kvstore-identifier</key>\n\t<string>$(TeamIdentifierPrefix)$(CFBundleIdentifier)</string>";

							string ent = File.ReadAllText(projectRootPath + "/Unity-iPhone/tabtale.entitlements");
							if(!ent.Contains("<key>com.apple.developer.icloud-container-identifiers</key>")){
								int entryPoint = ent.IndexOf("</dict>")-1;
								ent = ent.Insert(entryPoint,strToAdd);
							}
							File.WriteAllText(projectRootPath + "/Unity-iPhone/tabtale.entitlements",ent);
						}
					}
				}
			}
			
			foreach (var item in project.buildConfigurations) {
				if (! item.Value.buildSettings.ContainsKey("CODE_SIGN_ENTITLEMENTS")) 
					item.Value.buildSettings.Add("CODE_SIGN_ENTITLEMENTS","Unity-iPhone/tabtale.entitlements");
				else
					item.Value.buildSettings["CODE_SIGN_ENTITLEMENTS"] = "Unity-iPhone/tabtale.entitlements";
			}
			project.Save();

			UnityEngine.Debug.Log ("InjectingTeamIdToPbxProjFile end seccssfully");

	}


}
}
#endif
