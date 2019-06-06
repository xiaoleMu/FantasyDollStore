using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

namespace Game.Build
{
	public class PreprocessBuild : IPreprocessBuild
	{
		public int callbackOrder {
			get { return 0; }
		}

		public void OnPreprocessBuild (BuildTarget target, string path)
		{
//			var platform = PreprocessSettingsData.Platform.Unknown;
//
//			switch (target) {
//			case BuildTarget.iOS:
//				platform = PreprocessSettingsData.Platform.iOS;
//				break;
//			case BuildTarget.Android:
//#if AMAZON
//				platform = PreprocessSettingsData.Platform.Amazon;
//#else
//				platform = PreprocessSettingsData.Platform.GooglePlay;
//#endif
//				break;
//			}
//
//			PreprocessPlatformSettings (platform);
		}
//		
//		[MenuItem ("Coco Common/Preprocess Settings", false, 300)]
//		private static void ShowPreprocessSettings ()
//		{
//			Selection.activeObject = PreprocessSettingsData.Instance;
//		}
//
//
//		#region Settings
//
//		private void PreprocessPlatformSettings (PreprocessSettingsData.Platform platform)
//		{
//			var platformSettings = PreprocessSettingsData.Instance.GetPlatformSettings (platform);
//			if (platformSettings == null) {
//				return;
//			}
//
//			EnableObfuscatorSettings (platformSettings.enableObfuscator);
//		}
//
//		#endregion
//
//
//		#region Obfuscator
//
//		private const string OBFUSCATOR_SETTINGS_PATH = "\\OPS\\Obfuscator.Pro\\Obfuscator_Settings.txt";
//
//		private const string OBFUSCATE_GLOBALLY_KEY = "ObfuscateGlobally";
//
//		private static void EnableObfuscatorSettings (bool enabled)
//		{
//			var settingsPath = Application.dataPath + OBFUSCATOR_SETTINGS_PATH;
//			if (!File.Exists (settingsPath)) {
//				return;
//			}
//
////			if (UpdateObfuscatorGlobally (settingsPath, enabled)) {
////				Obfuscator.Gui.GuiSettings.SettingsGotReloaded = false;
////			}
////		}
//
//		private static bool UpdateObfuscatorGlobally (string settingsPath, bool enabled)
//		{
//			var settingsLines = File.ReadAllLines (settingsPath);
//
//			var globallyIndex = -1;
//			for (var i = 0; i < settingsLines.Length; i++) {
//				if (settingsLines [i] != OBFUSCATE_GLOBALLY_KEY) {
//					continue;
//				}
//
//				globallyIndex = i + 1;
//				break;
//			}
//
//			if (globallyIndex < 0 || globallyIndex >= settingsLines.Length) {
//				return false;
//			}
//
//			var globallyValue = enabled ? "1" : "0";
//			if (globallyValue == settingsLines [globallyIndex]) {
//				return false;
//			}
//
//			settingsLines [globallyIndex] = globallyValue;
//
//			File.WriteAllLines (settingsPath, settingsLines);
//			return true;
//		}
//
//		#endregion
	}
}