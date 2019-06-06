using System.IO;
using TabTale.PSDK.UnityEditor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

// #if UNITY_IOS
// using UnityEditor.iOS.Xcode;
// #endif
using PSDK.UnityEditor.XCodeEditor;

namespace CocoPlay.Build
{
	public class PostprocessBuild : IPostprocessBuild
	{
		// order need higher than ManifestMod.cs, in order to update xpaths from mainfest
		public int callbackOrder {
			get { return 91; }
		}

		public void OnPostprocessBuild (BuildTarget target, string path)
		{
			switch (target) {
#if UNITY_IOS
			case BuildTarget.iOS:
				XcodePostprocessBuild (path);
				break;
#endif
#if UNITY_ANDROID
			case BuildTarget.Android:
				AndroidPostprocessBuild (path);
				break;
#endif
			}
		}

		//special path in Framework
		private const string PLATFORM_PATH_FRAMEWORK = "CocoGenericFramework/CocoCoreModule/CommonBase/CocoCommon/_MultiPlatformFolder";

		private const string PLATFORM_PATH_LOCAL = "_Game/CocoPlay/GameCommon/_MultiPlatformFolder";

		private static string ModRootPath (bool isLocal)
		{
			string pPlatformPath = isLocal ? PLATFORM_PATH_LOCAL : PLATFORM_PATH_FRAMEWORK;

			string modPath = Path.Combine (Application.dataPath, pPlatformPath);
			modPath = Path.Combine (modPath, "Editor");

#if UNITY_ANDROID
#if AMAZON
			modPath = Path.Combine (modPath, "AM");
#else
			modPath = Path.Combine (modPath, "GP");
#endif
#endif

			return modPath;
		}


		#region Xcode Build

#if UNITY_IOS
		private static void XcodePostprocessBuild (string pathToBuiltProject)
		{
			UpdateInfoPlist (pathToBuiltProject);
			UpdateXcodeProjectFile (pathToBuiltProject);
		}

		public const string BITCODE_KEY = "ENABLE_BITCODE";

		private static void UpdateXcodeProjectFile (string pathToBuiltProject)
		{
			string projPath = FindProjectPath (pathToBuiltProject);
			if (string.IsNullOrEmpty (projPath)) {
				Debug.LogWarning ("Error: missing xcodeproj file");
				return;
			}

			// use psdk XCProject instead of unity PBXProject to process project file
			// in order to avoid the compile error on ttbuilder

			// PBXProject proj = new PBXProject ();
			// proj.ReadFromFile (projPath);
			// string target = proj.TargetGuidByName ("Unity-iPhone");
			//
			// // disable bitcode
			// proj.SetBuildProperty (target, "ENABLE_BITCODE", "NO");
			//
			// proj.WriteToFile (projPath);

			var project = new XCProject (projPath);
			if (project.project == null) {
				Debug.LogWarning ("Error: project has no root object");
				return;
			}

			foreach (var configuration in project.buildConfigurations) {
				var buildSettings = configuration.Value.buildSettings;

				// disable bitcode
				if (buildSettings.ContainsKey (BITCODE_KEY)) {
					buildSettings [BITCODE_KEY] = "NO";
				} else {
					buildSettings.Add (BITCODE_KEY, "NO");
				}
			}

			project.Save ();
		}

		private static void UpdateInfoPlist (string pathToBuiltProject)
		{
			UpdateInfoPlist (ModRootPath (true), pathToBuiltProject);
			UpdateInfoPlist (ModRootPath (false), pathToBuiltProject);
		}

		private static string FindProjectPath (string filePath)
		{
			string projPath = string.Empty;
			if (filePath.EndsWith (".xcodeproj")) {
				projPath = filePath;
			} else {
				string[] projects = Directory.GetDirectories (filePath, "*.xcodeproj");
				if (projects.Length > 0) {
					projPath = projects [0];
				}
			}
			// if (!string.IsNullOrEmpty (projPath)) {
			// 	projPath += "/project.pbxproj";
			// }

			return projPath;
		}

		private static void UpdateInfoPlist (string modPath, string pathToBuiltProject)
		{
			if (!Directory.Exists (modPath)) {
				return;
			}

			InfoPlistMod.UpdateInfoPlistFromManifestModsUnderPath (modPath, pathToBuiltProject);
		}

#endif

		#endregion


		#region Android Build

//#if UNITY_ANDROID
		private const string ANDROID_MANIFEST_XML = "AndroidManifest.xml";

		private static void AndroidPostprocessBuild (string pathToBuildProject)
		{
			// in ManifestMod.cs, the method "DeleteXpathsFromManifest" be called earlier,
			// so some contents in "*.manifestmod" and "*.gradle_manifestmod" be added again.
			// we call "ManifestMod.UpdateManifestFromManifestModsUnderPath" in here only in
			// order to remove these contents again.
			// this code can be removed if the problem be fixed in ManifestMod.cs

			var manifestPath = GetManifestPath (pathToBuildProject);

			if (manifestPath=="") {
				return;
			}

			Debug.LogFormat ("PostprocessBuild->AndroidPostprocessBuild: update manifest [{0}]", manifestPath);
			UpdateManifest (ModRootPath (false), manifestPath);
			UpdateManifest (ModRootPath (true), manifestPath);
		}

		private static string GetManifestPath(string pathToBuildProject)
		{
			// ***** PLEASE make sure "PlayerSettings.productName" is same as project name in AppDB ****
			var projectName = PlayerSettings.productName;

			// combine manifest file path
			var mainProjectPath = Path.Combine (pathToBuildProject, projectName);
			var manifestPath = Path.Combine (mainProjectPath, ANDROID_MANIFEST_XML);

			if (!File.Exists (manifestPath)) {
				Debug.LogWarningFormat ("PostprocessBuild->AndroidPostprocessBuild: main manifest [{0}] NOT found!", manifestPath);
				projectName =projectName+ "/src/main";
				mainProjectPath = Path.Combine (pathToBuildProject, projectName);
				manifestPath = Path.Combine (mainProjectPath, ANDROID_MANIFEST_XML);
				if (!File.Exists (manifestPath)) {
					Debug.LogWarningFormat ("PostprocessBuild->AndroidPostprocessBuild: main manifest [{0}] NOT found!", manifestPath);
					return "";
				}
				return manifestPath;
			}
			return "";
		}

		private static void UpdateManifest (string modPath, string manifestPath)
		{
			if (!Directory.Exists (modPath)) {
				return;
			}

			ManifestMod.UpdateManifestFromManifestModsUnderPath (manifestPath, modPath, true);
		}

//#endif

		#endregion
	}
}