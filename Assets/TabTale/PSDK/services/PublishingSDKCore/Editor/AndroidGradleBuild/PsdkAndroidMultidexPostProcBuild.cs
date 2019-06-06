#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;
using System.Diagnostics;
using System.Collections.Generic;


namespace Tabtale.Editor.PSDK {

	public static class PsdkAndroidMultidexPostProcBuild {

		[PostProcessBuild(2147483647)]
		public static void OnPostProcessBuild( BuildTarget target, string path )
		{
			#if UNITY_5_6_OR_NEWER
				copyGradleWrapper (Path.Combine(path, PlayerSettings.productName));
				return;
			#endif

			// check if building android google project, otherwise there is nothing to do
			if (target != BuildTarget.Android ){
				UnityEngine.Debug.Log("BuildTarget:" + target);
				return;
			}

#if UNITY_5
			if (! EditorUserBuildSettings.exportAsGoogleAndroidProject) { // here I should check if AcceptExternalModificationsToPlayer - e.g. a google project is being built
				UnityEngine.Debug.Log("EditorUserBuildSettings.exportAsGoogleAndroidProject:" + EditorUserBuildSettings.exportAsGoogleAndroidProject.ToString());
				return;
			}
#else
#endif
			AdaptToGradleWrapperProject (path);

			if (! UnityEditorInternal.InternalEditorUtility.inBatchMode &&  PsdkSettingsData.Instance.buildAPK) // if to automatically build project
				BuildApk (path);
			
		}

		//[ MenuItem("PSDK/testAndroidBuild")]
		static void testMenu() {
//			UnityEngine.Debug.Log (EditorUserBuildSettings.exportAsGoogleAndroidProject);
			string productName = "gradle_project_2";
			string projectPath = Application.dataPath + "/../build/" + productName;
			deleteUnneededFiles (projectPath);
//			AdaptToGradleWrapperProject (projectPath);
//			if (UnityEditorInternal.InternalEditorUtility.inBatchMode || PsdkSettingsData.Instance.buildAPK) // if to automatically build project
//				BuildApk (projectPath);
		}

		static void AdaptToGradleWrapperProject (string projectPath) {
			//deleteUnneededFiles (projectPath);
			RenameAppDir(projectPath);
			createGradleSettings (projectPath);
			createGradleProperties (projectPath);
			createGradleBuildFiles (projectPath);
			copyLibsToTTUnityProject (projectPath);
			copyMissingJarstoMultidexProject (projectPath); // mainly psdk providers
			copyMissingAARstoMultidexProject (projectPath); // mainly psdk providers
			copyGradleWrapper (projectPath);
			adjustManifest (projectPath);
			createKeystoreFile (projectPath);
		}

        static void RenameAppDir(string projectPath) {
            Directory.Move(Path.Combine(projectPath, PlayerSettings.productName),
                            Path.Combine(projectPath, "app"));
        }

		static void deleteUnneededFiles(string projectPath) {
			string[] files = { "android-support-v4.jar" };
			string[] dirs = { "google-play-services_lib" };

			foreach (string file in files) {
				foreach (string fileToDelete in Directory.GetFiles(projectPath, file, SearchOption.AllDirectories)) {
					File.Delete(fileToDelete);
				}
			}
			foreach (string dir in dirs) {
				string dirPath = Path.Combine(projectPath,dir);
				if (Directory.Exists(dirPath))
					Directory.Delete(dirPath,true);
			}
		}

		static void copyKeystoreFile(string projectPath) {
			string keyStoreFileSrc = PlayerSettings.Android.keystoreName;
			if (string.IsNullOrEmpty (keyStoreFileSrc)) {
				UnityEngine.Debug.LogError("NULL Keystore file !!!");
			}
			string keyStoreFileDst = Path.Combine (projectPath, "g.ks");
			if (! keyStoreFileSrc.StartsWith ("/")) { // not full path full path 
				keyStoreFileSrc = Path.Combine("..",keyStoreFileSrc);
				keyStoreFileSrc = Path.Combine(Application.dataPath,keyStoreFileSrc);
			}
			File.Copy(keyStoreFileSrc,keyStoreFileDst,true);
		}

		static void createKeystoreFile(string projectPath) {
			copyKeystoreFile (projectPath);
			string str = "";
			str += "keyPassword=" + PlayerSettings.Android.keyaliasPass + "\n";
			str += "storePassword=" + PlayerSettings.Android.keystorePass + "\n";
			str += "keystore=g.ks" + "\n";
			str += "keyAlias=" + PlayerSettings.Android.keyaliasName + "\n";
			string outputFile = Path.Combine (projectPath, "keystore.properties");
			File.WriteAllText (outputFile, str);
		}

		static bool IsAARExists(string projectPath, string filename) {
			var files = Directory.GetFiles(projectPath, filename + ".aar", SearchOption.AllDirectories);
			if (files.Length > 0) {
				return true;
			}
			return false;
		}

		static void createGradleSettings (string projectPath) {
			string psdkLibDirPath = Path.Combine(projectPath, "psdk_lib");
			if (Directory.Exists(psdkLibDirPath)) {
				Directory.Delete(psdkLibDirPath, true);
			}
			
			string socialLibDirPath = Path.Combine(projectPath, "social_lib");
			if (Directory.Exists(socialLibDirPath)) {
				Directory.Delete(socialLibDirPath, true);
			}
			
			string str = "";
			foreach (string item in  Directory.GetDirectories(projectPath)) {
				string filename = Path.GetFileName(item);
				if (filename  == "gradle" ) continue;
				if (filename.StartsWith("." )) continue;
				if (filename  == "build" ) continue;
				if (filename  == "google-play-services_lib" ) continue;
				// not adding directory when an aar already exists
				if (IsAARExists (projectPath, filename)) {
					Directory.Delete (item,true);
					continue;
				}

				// not adding ttunity project, if the src folder was not exported, and the jar already exported to main project.
				if (filename  == "ttunity" && ! Directory.Exists(Path.Combine(projectPath,Path.Combine(filename,"src")))) continue;
				str += "include ':" + filename + "'\n";
			}

			string additionToSettingsPath = Path.Combine (AndroidGradleSourceFolderPath, "_root_bottom_settings_._gradle_");
			if (File.Exists (additionToSettingsPath)) {
				string additions = File.ReadAllText(additionToSettingsPath);
				str += "\n" + additions + "\n";
			}

			File.WriteAllText(Path.Combine(projectPath,"settings.gradle"),str);
		}

		static private string AndroidGradleSourceFolderPath {
			get {
				string prePath = Path.Combine(Application.dataPath,TabTale.Plugins.PSDK.PsdkUtils.PsdkRootPath);
				prePath = Path.Combine(prePath,"services");
				prePath = Path.Combine(prePath,"PublishingSDKCore");
				prePath = Path.Combine(prePath,"Editor");
				prePath = Path.Combine(prePath,"AndroidGradleBuild");
				return prePath;
			}
		}

		static void createGradleProperties (string projectPath) {
            string[] arguments = System.Environment.GetCommandLineArgs();
            string gatewayEnv = "";
			int argumentIndex = System.Array.IndexOf(arguments,"-gatewayEnv");

            if (argumentIndex > 0 && arguments.Length > (argumentIndex + 1)) {
                gatewayEnv = arguments[argumentIndex + 1];
            }

			string prePath = AndroidGradleSourceFolderPath;

			string dstFile = Path.Combine (projectPath, "gradle.properties");
			if (! File.Exists (dstFile)) {
				if (File.Exists (Path.Combine (prePath, "root_gradle.properties")))
					File.Copy (Path.Combine (prePath, "root_gradle.properties"), dstFile, false);
				else 
					File.Copy (Path.Combine (prePath, "_root_gradle_._properties_"), dstFile, false);
			}

			string str = File.ReadAllText (dstFile);
			str += "\n";
			str += "productName=" + PlayerSettings.productName +"\n";
			#if UNITY_5_6_OR_NEWER
		            str += "bundleIdentifier=" + PlayerSettings.applicationIdentifier +"\n";
		        #else
		           str += "bundleIdentifier=" + PlayerSettings.bundleIdentifier +"\n";
		        #endif
			if(!string.IsNullOrEmpty(gatewayEnv)) {
			    str += "gatewayEnv=" + gatewayEnv + "\n";
			}

            if (PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Contains("AMAZON")) {
				UnityEngine.Debug.Log("----------STORE ------------ AMAZON ------------");
                str += "storeName=amazon\n";
			}
            else {
               	str += "storeName=google\n";
                UnityEngine.Debug.Log("----------STORE ------------ GOOGLE ------------");
			}
 
            UnityEngine.Debug.Log(str);

			if (PsdkInternalSettingsData.Instance.socialGamePackage) {
				str += "googlePlayGames=true\n";
			}
			else {
			    str += "googlePlayGames=false\n";
			}

			File.WriteAllText(Path.Combine(projectPath,"gradle.properties"),str);
		}
		

		static void createGradleBuildFiles (string projectPath){
			string prePath = AndroidGradleSourceFolderPath;
			// copy root project build.gradle
			string dstFile = Path.Combine (projectPath, "build.gradle");
			if (! File.Exists (dstFile)) {
				if (File.Exists (Path.Combine (prePath, "root_build.gradle")))
					File.Copy (Path.Combine (prePath, "root_build.gradle"), dstFile, false);
				else 
					File.Copy (Path.Combine (prePath, "_root_build_._gradle_"), dstFile, false);
			}
			// copy min projecy build.grasle
			dstFile = Path.Combine(projectPath, Path.Combine ("app", "build.gradle"));
			if (! File.Exists (dstFile)) {
				if (File.Exists(Path.Combine (prePath, "app_build.gradle")))
					File.Copy (Path.Combine (prePath, "app_build.gradle"), dstFile, false);
				else
					File.Copy (Path.Combine (prePath, "_app_build_._gradle_"), dstFile, false);
			}
		}


		static void copyMissingJarstoMultidexProject (string projectPath) {
			
			string dstDir = Path.Combine(projectPath,Path.Combine("app","libs"));
			if (! Directory.Exists (dstDir))
				Directory.CreateDirectory (dstDir);
			List<string> jars = new List<string> ();
			foreach (string jarFile in Directory.GetFiles(Application.dataPath,"*.gradle_jar",SearchOption.AllDirectories)){
					jars.Add(jarFile);
			}
			foreach (string srcFile in jars) {
				if (File.Exists(srcFile)) {
					//UnityEngine.Debug.Log("copy " + srcFile + " -> " + Path.Combine(dstDir,Path.GetFileName(srcFile)));
					File.Copy(srcFile, Path.Combine(dstDir,Path.GetFileNameWithoutExtension(srcFile)+".jar"),true);
				}
				else {
					UnityEngine.Debug.LogError("Didn't find file: " + srcFile);
					if (UnityEditorInternal.InternalEditorUtility.inBatchMode ) {
						EditorApplication.Exit(-1);
					}
				}
			}
		}

		static void copyMissingAARstoMultidexProject (string projectPath) {
			
			string dstDir = Path.Combine(projectPath,Path.Combine("app","libs"));
			if (! Directory.Exists (dstDir))
				Directory.CreateDirectory (dstDir);
			List<string> aars = new List<string> ();
			foreach (string aarFile in Directory.GetFiles(Application.dataPath,"*.gradle_aar",SearchOption.AllDirectories)){
				aars.Add(aarFile);
			}
			foreach (string srcFile in aars) {
				if (File.Exists(srcFile)) {
					//UnityEngine.Debug.Log("copy " + srcFile + " -> " + Path.Combine(dstDir,Path.GetFileName(srcFile)));
					File.Copy(srcFile, Path.Combine(dstDir,Path.GetFileNameWithoutExtension(srcFile)+".aar"),true);
				}
				else {
					UnityEngine.Debug.LogError("Didn't find file: " + srcFile);
					if (UnityEditorInternal.InternalEditorUtility.inBatchMode ) {
						EditorApplication.Exit(-1);
					}
				}
			}
		}

		static void copyLibsToTTUnityProject (string projectPath) {

			if (! Directory.Exists(Path.Combine(projectPath,"ttunity")))
				return;

			string dstDir = Path.Combine(projectPath,Path.Combine("ttunity","libs"));
			string srcFolder = Path.Combine (projectPath, Path.Combine ("app", "libs"));
			if (! Directory.Exists (dstDir))
				Directory.CreateDirectory (dstDir);
			string [] files = { "publishingsdkcore.jar" , "unity-classes.jar"};
			foreach (string file in files) {
				string srcFile = Path.Combine (srcFolder, file);
				if (File.Exists(srcFile)) 
					File.Copy(srcFile, Path.Combine(dstDir,file),true);
				else {
					UnityEngine.Debug.LogError("Didn't find file: " + srcFile);
					if (UnityEditorInternal.InternalEditorUtility.inBatchMode ) {
						EditorApplication.Exit(-1);
					}
				}
			}
		}

		static void copyGradleWrapper (string projectPath){
			string prePath = AndroidGradleSourceFolderPath;
			string dstFile = Path.Combine (projectPath, "gradlew");
			if (! File.Exists(dstFile))
				File.Copy(Path.Combine(prePath,"gradlew"),dstFile,false);
			dstFile = Path.Combine (projectPath, "gradlew.bat");
			if (! File.Exists(dstFile))
				File.Copy(Path.Combine(prePath,"gradlew.bat"),dstFile,false);
			string gradleFolderPath = Path.Combine (projectPath, "gradle");
			string wrapperFolderPath = Path.Combine (gradleFolderPath, "wrapper");
			if (! Directory.Exists (gradleFolderPath)) {
				Directory.CreateDirectory(gradleFolderPath);
				Directory.CreateDirectory(wrapperFolderPath);
				string srcGradleFolder = Path.Combine(prePath,"gradle");
				srcGradleFolder = Path.Combine(srcGradleFolder,"wrapper");
				dstFile = Path.Combine (wrapperFolderPath, "gradle-wrapper.jar");
				if (! File.Exists(dstFile))
					File.Copy(Path.Combine(srcGradleFolder,"gradle-wrapper.zip"),dstFile,false);
				dstFile = Path.Combine (wrapperFolderPath, "gradle-wrapper.properties");
				if (! File.Exists(dstFile))
					File.Copy(Path.Combine(srcGradleFolder,"gradle-wrapper.properties"),dstFile,false);
			}
		}
		

		static void adjustManifest (string projectPath){
			// ' android:debuggable="false"'
			string toReplaceWithEmpty = " android:debuggable=\"false\"";
			foreach (string item in  Directory.GetDirectories(projectPath)) {
				string manifestPath = Path.Combine(item,"AndroidManifest.xml");
				if (! File.Exists(manifestPath))
					continue;
				string manifestContent = File.ReadAllText(manifestPath);
				if (manifestContent.Contains(toReplaceWithEmpty)) {
					string newContect = manifestContent.Replace(toReplaceWithEmpty,"");
					File.WriteAllText(manifestPath,newContect);
				}
			}
		}
		

		static void BuildApk(string projectPath)
		{
			ProcessStartInfo proc = new ProcessStartInfo();
			if (! proc.EnvironmentVariables.ContainsKey("ANDROID_HOME")) {
				System.String android_sdk_root = UnityEditor.EditorPrefs.GetString ("AndroidSdkRoot");
				if (!System.String.IsNullOrEmpty (android_sdk_root)) {
					proc.EnvironmentVariables.Add ("ANDROID_HOME", android_sdk_root);
				} else {
					UnityEngine.Debug.LogWarning ("Please set ANDROID_HOME, or android sdk root at unity preferences !");
				}
			}
			string command = projectPath + "/gradlew";
			proc.FileName = command;
			proc.WorkingDirectory = projectPath;
			string arguments = "assembleRelease --refresh-dependencies";
			proc.Arguments = arguments;
			proc.WindowStyle = ProcessWindowStyle.Minimized;
			proc.CreateNoWindow = true;
			UnityEngine.Debug.Log(proc.WorkingDirectory + ": " + command + " " + arguments);
			proc.UseShellExecute = false;
			proc.CreateNoWindow = true;
			proc.ErrorDialog = false;
			proc.WindowStyle = ProcessWindowStyle.Hidden;
			proc.RedirectStandardOutput = true;
			proc.RedirectStandardError = true;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				proc.FileName = projectPath + "/gradlew.bat";
			}
			Process p = new Process();
			p.StartInfo = proc;
			p.ErrorDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);
			p.OutputDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);
			p.Start();
			p.BeginErrorReadLine();
			p.BeginOutputReadLine();
			p.WaitForExit();
			int rc = p.ExitCode;
			p.Close();
			if (rc != 0)
			{ // failed
				UnityEngine.Debug.LogError("Failed in building APK, rc:" + rc);
				if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
					EditorApplication.Exit(rc);
				throw new System.Exception("Failed in building multidex APK, rc:" + rc);
			}
			else
			{
				UnityEngine.Debug.Log("succeeded !");
			}

		}
		


	}

}
#endif
