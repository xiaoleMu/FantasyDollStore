using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;


// This is a script added to diable GooglePlayGames from iOS
using System.Runtime.InteropServices;

namespace TabTale.PSDK.Social.Editor {
	
	[InitializeOnLoad]
	public static class PSDK_GPGSUtils
	{
		static PSDK_GPGSUtils() {
			fixPlayerSettings();
		}
		
		//[MenuItem("Window/Fix MainLibProj aar")]
		private static void fixPlayerSettings() {
			AddScriptDefineSymbol (BuildTargetGroup.iOS, "NO_GPGS");
			PsdkInternalSettingsData.Instance.socialGamePackage = true;
			if (UnityEditorInternal.InternalEditorUtility.inBatchMode) {
				// Builder
				UnityEngine.Debug.Log("Social, re-generating MainLibProj/AndroidManifest.xml and MainLibProj.aar");
				DeleteMainLibProjAAR ();
				GooglePlayGames.Editor.GPGSUtil.GenerateAndroidManifest();
			}
		}
			
		private static void DeleteMainLibProjAAR() {
			var files = Directory.GetFiles(Application.dataPath, "MainLibProj.*aar", SearchOption.AllDirectories);
			foreach (var file in files) {
				UnityEngine.Debug.Log ("Deleting " + file.ToString());
				System.IO.File.Delete (file);
			}
		}

		static void AddScriptDefineSymbol(BuildTargetGroup btg, string synmbol) {
			string sds = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
			if (! sds.Contains(synmbol)) {
				if (sds != "") sds+= ";";
				sds+=synmbol;
				PlayerSettings.SetScriptingDefineSymbolsForGroup(btg,sds);
			}
		}

		public static void CompressMainLibProjToAAR() {
			try 
			{
				string scriptBaseFilename = "aarMainLibProj.py";
				string[] pythonScripts = Directory.GetFiles(Path.Combine(Application.dataPath,PsdkUtils.PsdkRootPath),scriptBaseFilename,SearchOption.AllDirectories);
				if (pythonScripts.Length ==0) {
					UnityEngine.Debug.LogError("didn't find pythin script " + scriptBaseFilename + " for creating GPGS MainLibProj AAR !");
					return;
				}
				string pythonScriptFullPath = pythonScripts[0];

				DirectoryInfo psdkRootDirInfo = new DirectoryInfo (Path.Combine(Application.dataPath,PsdkUtils.PsdkRootPath));
				DirectoryInfo[] MainProjLibs = psdkRootDirInfo.GetDirectories ("*",SearchOption.AllDirectories);
				UnityEngine.Debug.Log (MainProjLibs.Length);

				foreach (DirectoryInfo directorySelected in MainProjLibs)
	        	{
					if (! directorySelected.ToString().EndsWith("MainLibProj"))
						continue;

					Process myCustomProcess = new Process ();
					myCustomProcess.StartInfo.FileName = "python";
					myCustomProcess.StartInfo.WorkingDirectory = directorySelected.ToString();
					myCustomProcess.StartInfo.Arguments = string.Format ("\"{0}\"",pythonScriptFullPath);
					UnityEngine.Debug.Log (myCustomProcess.StartInfo.FileName + " " + myCustomProcess.StartInfo.Arguments); 		
					myCustomProcess.StartInfo.UseShellExecute = false;
					myCustomProcess.StartInfo.RedirectStandardOutput = true;
					myCustomProcess.StartInfo.RedirectStandardError = true;
					myCustomProcess.Start (); 
					myCustomProcess.WaitForExit ();
					string strOutput = myCustomProcess.StandardOutput.ReadToEnd();
					string strErrorOutput = myCustomProcess.StandardError.ReadToEnd();
					int rc = myCustomProcess.ExitCode;
					myCustomProcess.Close ();
					if (rc != 0) { // failed
						UnityEngine.Debug.LogError ("Failed in "+ scriptBaseFilename +", rc:" + rc);
						UnityEngine.Debug.LogError (strErrorOutput);
						UnityEngine.Debug.Log (strOutput);
						if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
							EditorApplication.Exit(rc);
						throw new System.Exception("Failed in exceuting  "+ scriptBaseFilename + " , rc:" + rc);
					} else {
						if (strErrorOutput.Length > 0)
							UnityEngine.Debug.Log(strErrorOutput);
						if (strOutput.Length > 0)
							UnityEngine.Debug.Log(strOutput);
					}


				}
			}
			catch (System.Exception e) {
				UnityEngine.Debug.LogException(e);
			}
		}




	}
}