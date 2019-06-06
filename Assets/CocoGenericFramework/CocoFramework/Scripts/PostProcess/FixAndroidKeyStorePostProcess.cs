#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public static class FixAndroidKeyStorePostProcess
{
	static FixAndroidKeyStorePostProcess() {
		fixKeyStore();
	}


	[MenuItem("Tools/Update keystore")]
	private static void fixKeyStore() {

		if (PlayerSettings.Android.keyaliasPass == "")
			PlayerSettings.Android.keyaliasPass = "storybook";

		if (PlayerSettings.Android.keystorePass == "")
			PlayerSettings.Android.keystorePass = "storybook"; 
	}
	

}
#endif