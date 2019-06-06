using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using TabTale.Plugins.PSDK;

[InitializeOnLoad]
public static class FixPlayerSettingsPostProcess
{
	static FixPlayerSettingsPostProcess() {
		fixPlayerSettings();
	}

	[PostProcessBuild(98)]
	public static void OnPostProcessBuild( BuildTarget target, string path )
	{
		fixPlayerSettings();
	}

	private static void fixPlayerSettings() {
		fixAotCompilationOptions();
		fixApiCompatibilityLevel();
		fixScriptingBackend();
		PlayerSettings.iOS.allowHTTPDownload = false;
	}
	
	private static void fixAotCompilationOptions() {
		if (! UnityEditor.PlayerSettings.aotOptions.Contains("trampolines")) {
			Debug.Log ("Adding nrgctx-trampolines=8096,nimt-trampolines=8096,ntrampolines=4048 to AOT Compilation Options Player Settings !");
			if (UnityEditor.PlayerSettings.aotOptions.Trim(new char[] {' '})  == "") 
				UnityEditor.PlayerSettings.aotOptions = "nrgctx-trampolines=8096,nimt-trampolines=8096,ntrampolines=4048";
			else 
				UnityEditor.PlayerSettings.aotOptions += ",nrgctx-trampolines=8096,nimt-trampolines=8096,ntrampolines=4048";
		}
	}


	private static void fixApiCompatibilityLevel() {
		if ( UnityEditor.PlayerSettings.apiCompatibilityLevel != ApiCompatibilityLevel.NET_2_0) {
			if ( PsdkVersionsMgr.IsGLDInstalled()) {
				Debug.Log ("Setting API Compatibility Level to .NET 2.0 at Player Settings, for GLD Unzip !");
				UnityEditor.PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
			}
		}
	}
	
        private static void fixScriptingBackend() {
                try {
#if UNITY_IOS
                        //Debug.Log ("Setting scripting implemenation (backend) to il2cpp");
                        PlayerSettings.SetPropertyInt("ScriptingBackend",(int)ScriptingImplementation.IL2CPP,BuildTargetGroup.iOS);
#if UNITY_4_6
			if (PsdkSettingsData.Instance.universalBuild) {
				PlayerSettings.SetPropertyInt("Architecture",(int)iPhoneArchitecture.Universal,BuildTargetGroup.iPhone);
			}
#endif
#endif
                }
                catch (System.Exception e) {
                        Debug.LogException(e);
                }
        }


}
