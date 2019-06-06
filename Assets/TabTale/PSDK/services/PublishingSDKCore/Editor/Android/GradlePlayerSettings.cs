#if UNITY_ANDROID && UNITY_5_6_OR_NEWER
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text.RegularExpressions;


[InitializeOnLoad]
public static class GradlePlayerSettings
{
	private static IDictionary<string,object> _appsDbDict = null;

	//[MenuItem("Window/PSDK/RonTest")]
	//static void menuTest() {
	//	copyMainTemplateGradle ();
	//}

	static GradlePlayerSettings() {
		setStore();
		if (UnityEditorInternal.InternalEditorUtility.inBatchMode) {
		setPlayerSettings ();
		copyMainTemplateGradle ();
		}
	}

	static void setStore() {
		string psdkLibDirPath = Path.Combine(Application.dataPath, "Plugins/Android/psdk_lib");

		if (!File.Exists (psdkLibDirPath)) {
			UnityEngine.Debug.Log ("Not set store becasue psdk_lib does not exists.");
			return;
		}

		string unityConfigFilePath = Path.Combine(psdkLibDirPath, "unity_config.json");
		IDictionary<string,object> unityConfigJSON = null;
		if (!File.Exists (unityConfigFilePath)) {
			File.Create (unityConfigFilePath).Dispose ();
			unityConfigJSON = new Dictionary<string,object> ();
		} else {
			string unityConfigText = System.IO.File.ReadAllText (unityConfigFilePath);
			unityConfigJSON = TabTale.Plugins.PSDK.Json.Deserialize (unityConfigText) as IDictionary<string,object>;
		}

		if (PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android).Contains ("AMAZON")) {
			unityConfigJSON ["store"] = "amazon";
		} else {
			unityConfigJSON ["store"] = "google";
		}

		System.IO.File.WriteAllText(unityConfigFilePath, TabTale.Plugins.PSDK.Json.Serialize(unityConfigJSON));
	}

	static void copyMainTemplateGradle() {
		string mainTemplateGradleProjFilePath = Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle");
		if (File.Exists (mainTemplateGradleProjFilePath)) {
			UnityEngine.Debug.Log ("Using custom mainTemplate.gradle");
			return;
		}

		string mainTemplateGradleFilePath = Path.Combine (EditorApplication.applicationPath, "../PlaybackEngines/AndroidPlayer/Tools/GradleTemplates/mainTemplate.gradle");
		UnityEngine.Debug.Log ("Copy mainTemplate.gradle from: " + mainTemplateGradleFilePath);
		if (!File.Exists(mainTemplateGradleFilePath)) {
			LogAndExitOnError("Failed to copy mainTemplate.gradle from " + mainTemplateGradleFilePath);
		}

		File.Copy (mainTemplateGradleFilePath, mainTemplateGradleProjFilePath);
		string mainTemplateGradleFileContent = File.ReadAllText(mainTemplateGradleProjFilePath);
		mainTemplateGradleFileContent = mainTemplateGradleFileContent.Replace("useProguard **PROGUARD_DEBUG**", "//minifyEnabled **MINIFY_DEBUG**");
		mainTemplateGradleFileContent = mainTemplateGradleFileContent.Replace("useProguard **PROGUARD_RELEASE**", "//useProguard **PROGUARD_RELEASE**");

		string pattern = @"[\s]classpath \'com\.android\.tools\.build:gradle:.*";
		string replacement = "classpath 'com.android.tools.build:gradle:3.1.0'";
		mainTemplateGradleFileContent = Regex.Replace (mainTemplateGradleFileContent, pattern, replacement);
		
        	//string oldString = "defaultConfig {";
        	//string newString = "defaultConfig { \n multiDexEnabled true \n ndk { abiFilters 'armeabi-v7a','x86' }";
        	//mainTemplateGradleFileContent = mainTemplateGradleFileContent.Replace(oldString, newString);
		
		string oldString = "buildscript {";
		string newString = "configurations.all { \n" +
		    "resolutionStrategy.cacheChangingModulesFor 0, 'seconds' \n" +
		    "resolutionStrategy.cacheDynamicVersionsFor 0, 'seconds' \n" +
		"} \n" +
		"buildscript { \n" +
		    "configurations.all { \n" +
			"resolutionStrategy.cacheChangingModulesFor 0, 'seconds' \n" +
			"resolutionStrategy.cacheDynamicVersionsFor 0, 'seconds' \n" +
		    "} \n" +
		    "String awsS3AccessKeyTemp = null \n" +
		    "String awsS3SecretKeyTemp = null \n" +
		    "if (project.hasProperty('awsS3AccessKey') && project.hasProperty('awsS3SecretKey') && \n" +
		            "project.awsS3AccessKey && project.awsS3SecretKey) { \n" +
		        "awsS3AccessKeyTemp = awsS3AccessKey \n" +
		        "awsS3SecretKeyTemp = awsS3SecretKey \n" +
		    "} \n" +
		    "else if (System.getenv()['AWS_ACCESS_KEY_ID'] && System.getenv()['AWS_SECRET_ACCESS_KEY']) { \n" +
		        "awsS3AccessKeyTemp = System.getenv()['AWS_ACCESS_KEY_ID'] \n" +
		        "awsS3SecretKeyTemp = System.getenv()['AWS_SECRET_ACCESS_KEY'] \n" +
		    "} \n" +
		    "if (!awsS3AccessKeyTemp) { \n" +
		        "throw new GradleException('awsS3AccessKey and awsS3SecretKey does not exists') \n" +
		    "} \n" +
		    "allprojects { \n" +
		        "ext { \n" +
		            "awsS3AccessKey = awsS3AccessKeyTemp \n" +
		            "awsS3SecretKey = awsS3SecretKeyTemp \n" +
		        "} \n" +
		    "} \n";
        mainTemplateGradleFileContent = mainTemplateGradleFileContent.Replace(oldString, newString);

		oldString = "apply plugin: 'com.android.application'";
		newString = "apply plugin: 'com.android.application' \n" +
		   			"apply plugin: 'com.tabtale.android' \n" +
		    		"apply plugin: 'com.tabtale.plugins.publishingsdk' \n" +
					"psdk { \n" +
					    "sdkVersion '" + GetPSDKVersion() + "' \n" +
					    "config 'compile' \n" +
					    "buildType 'debug' \n" +
					    "downloadConfigFile 'false' \n" +
					    "s3AccessKey getProperty('awsS3AccessKey') \n" +
					    "s3SecretKey getProperty('awsS3SecretKey') \n" +
					"} \n";
        mainTemplateGradleFileContent = mainTemplateGradleFileContent.Replace(oldString, newString);

		if(!mainTemplateGradleFileContent.Contains("google()")) {
			oldString = "repositories {";
			newString = "repositories { \n google()";
			Regex r = new Regex(oldString);
			mainTemplateGradleFileContent = r.Replace(mainTemplateGradleFileContent, newString, 1);
		}	

		oldString = "dependencies {";
		newString = "repositories { \n" +
						"maven { url 'http://download.safedk.com/maven' } \n" +
				        "maven { \n" +
				            "url 's3://com.tabtale.repo/android/maven/gradle-plugins' \n" +
				            "credentials(AwsCredentials) { \n" +
				                "accessKey getProperty('awsS3AccessKey') \n" +
				                "secretKey getProperty('awsS3SecretKey') \n" +
				            "} \n" +
						"} \n" +
				        "maven { \n" +
				            "name 'backup' \n" +
				            "url 's3://com.tabtale.repo/android/maven/backup' \n" +
				            "credentials(AwsCredentials) { \n" +
				                "accessKey getProperty('awsS3AccessKey') \n" +
				                "secretKey getProperty('awsS3SecretKey') \n" +
				            "} \n" +
				        "} \n" +
				    "} \n \n" +
					"dependencies { \n" +
						"classpath 'com.tabtale.plugins:publishingsdk:1.4.+' \n" +
						"classpath 'com.tabtale:android:1.0' \n" +
						"classpath 'com.safedk:SafeDKGradlePlugin:1.+' \n";

		Regex r1 = new Regex(oldString);
		mainTemplateGradleFileContent = r1.Replace(mainTemplateGradleFileContent, newString, 1);
		
		File.WriteAllText(mainTemplateGradleProjFilePath, mainTemplateGradleFileContent);
	}

	static void setPlayerSettings() {
		IDictionary<string,object> appsDbDict = GetAppsDBJSON();

		string minSDKversion = ((appsDbDict["google"] as IDictionary<string,object>)["minSDKversion"] as string);
		UnityEngine.Debug.Log ("Set minSDKversion to: " + minSDKversion);
		AndroidSdkVersions androidMinSdkVersion = (AndroidSdkVersions)Enum.Parse(typeof(AndroidSdkVersions), minSDKversion);
		if (! System.String.IsNullOrEmpty(minSDKversion)) {
			PlayerSettings.Android.minSdkVersion = androidMinSdkVersion;
		}

		string targetSDKversion = ((appsDbDict["google"] as IDictionary<string,object>)["targetSDKVersion"] as string);
		UnityEngine.Debug.Log ("Set targetSDKversion to: " + targetSDKversion);
		AndroidSdkVersions androidTargetSdkVersion = (AndroidSdkVersions)Enum.Parse(typeof(AndroidSdkVersions), targetSDKversion);
		if (! System.String.IsNullOrEmpty(targetSDKversion)) {
			PlayerSettings.Android.targetSdkVersion = androidTargetSdkVersion;
		}

		EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
		AssetDatabase.SaveAssets ();
	}

	static IDictionary<string,object> GetAppsDBJSON() {
		if (_appsDbDict != null) {
			return _appsDbDict;
		}

		string appsDbFilePath = null;
		string[] filePaths = Directory.GetFiles(System.IO.Path.Combine(Application.dataPath,".."), "appsDbJson.*.txt");
		if (filePaths != null && filePaths.Length > 0) {
			appsDbFilePath = filePaths[0];
		}
		else {
			LogAndExitOnError ("Missing " + appsDbFilePath);
		}

		string appsDbJson = System.IO.File.ReadAllText (appsDbFilePath);
		_appsDbDict = TabTale.Plugins.PSDK.Json.Deserialize (appsDbJson) as IDictionary<string,object>;

		if (_appsDbDict == null) {
			LogAndExitOnError("Wrong appsDb json:" + appsDbJson);			
		}

		return _appsDbDict;
	}

	static string GetPSDKVersion() {
		string versionFilePath = System.IO.Path.Combine(Application.dataPath, "StreamingAssets/psdk/versions/PSDKCore.unitypackage.version.txt");
		string version = System.IO.File.ReadAllText(versionFilePath);
		version = Regex.Replace(version, @"\t|\n|\r", "");

		return version;
	}

	static void LogAndExitOnError(string outstr) {
		if (! String.IsNullOrEmpty (outstr)) {
			UnityEngine.Debug.Log ("Unity build returned with error:" + outstr);
			EditorApplication.Exit (-1);
		} else {
			UnityEngine.Debug.Log ("=== Unity build succeeded ! ===");
			EditorApplication.Exit (0);
		}
	}
}
#endif
