using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CocoPlay.Build
{
	public class PreprocessBuild : IPreprocessBuild
	{
		public int callbackOrder {
			get { return 0; }
		}

		public void OnPreprocessBuild (BuildTarget target, string path)
		{
			FixPlayerSettings ();
		}


		#region Player Settings

		private static void FixPlayerSettings ()
		{
			var platformType = MultiPlatformType.iOS;

#if UNITY_ANDROID
#if AMAZON
			platformType = MultiPlatformType.Amazon;
#else
			platformType = MultiPlatformType.GooglePlay;
#endif
#endif
			//Debug.LogError ("GlobalSettingEditor->FixSettingsOnLoad: platformType " + platformType);

			var platformItem = GlobalSettingEditor.GetPlatformItem (platformType);
			if (platformItem == null) {
				return;
			}

			// update debug settings
			var message = CocoDebugSettingsCreator.UpdateSettingsDataFromPSDKConfig ();
			Debug.LogFormat ("FixPlayerSettings: {0}", message);

			// update player settings
			GlobalSettingEditor.SetSplitBinaryOption (platformItem);
			GlobalSettingEditor.SetOrientationAndSplash (platformItem);
			GlobalSettingEditor.SetScriptingDefineSymbols (platformItem);
		}

		#endregion
	}
}