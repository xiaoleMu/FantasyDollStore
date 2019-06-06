using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.IO;

namespace Tabtale.Editor.PSDK {
	public static class PsdkBuildMultidexPsdkMenu {

		private static string _store = "google";
		// Build a folder containing unity3d file and html file
		[MenuItem ("TabTale/PSDK/Build multidexed APK")]
		static void BuildMultyDexedAPK(){

			#if AMAZON
			_store = "amazon";
			#else
			_store = "google";
			#endif

			BuildOptions _buildOptions = BuildOptions.None;
			_buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
			#if UNITY_5			
			EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
			#endif
			LogAndExitOnError( BuildPipeline.BuildPlayer( ScenesNames(), OutputFilePath() + "_project", BuildTarget.Android, _buildOptions).ToString());
		}

		private static string[] ScenesNames()
		{
			List<string> temp = new List<string>();
			foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
			{
				if (S.enabled)
				{
					string name = S.path.Substring(S.path.LastIndexOf('/')+1);
					name = name.Substring(0,name.Length-6);
					temp.Add(S.path);
				}
			}
			return temp.ToArray();
		}

		static string OutputFilePath() {
			string path = Application.dataPath + "/../build";
			path  = Directory.GetParent(path).FullName;
			
			path = System.IO.Path.Combine (path, _store);
			
			if (!System.IO.Directory.Exists (path)) {
				WriteToLog("Creating directory " + path);
				System.IO.Directory.CreateDirectory (path);
			}

			string _gameName = "";

			if (_gameName == "") 
				_gameName = RemoveSpecialCharacters(PlayerSettings.productName.Replace(" ",""));
			
			
			if (_gameName == "")
				_gameName = TabTale.Plugins.PSDK.PsdkUtils.BundleIdentifier;
			
			if (_gameName == "")
				_gameName = "game";
			
			_gameName += "_" + _store;
				
			_gameName += "_project";
			
			WriteToLog ("GameName = " + _gameName);
			
			path = System.IO.Path.Combine (path, _gameName);
			
			WriteToLog("OutPutFile path is: " + path);
			
			return path;
			
		}

		static void LogAndExitOnError(string outstr) {
			if (! System.String.IsNullOrEmpty (outstr)) {
				UnityEngine.Debug.LogError ("Unity build returned with error:" + outstr);
			} else {
				UnityEngine.Debug.Log ("=== Unity build succeeded ! ===");
			}
		}

		public static string RemoveSpecialCharacters(string str) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (char c in str) {
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		static void WriteToLog(string str){
			string msgPrefix = "APKBUILD -----> ";
			UnityEngine.Debug.Log (msgPrefix + str);
		}


	}
}
