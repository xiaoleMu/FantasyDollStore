using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CocoPlay
{
	/// <summary>
	/// clean old plugins because it already be included in module
	/// for save the load time,  can be removed in future if no project use old plugin longer
	/// </summary>
	[InitializeOnLoad]
	public class CocoCleanExpiredPlugins
	{
		static CocoCleanExpiredPlugins ()
		{
			CleanExpiredPlugins ();
		}


		private static readonly string[] _expiredPluginPaths = {
			// CocoCommon (iOS)
			"Editor/CocoCommon",
			// ScreenSafeArea (iOS)
			"Plugins/iOS/SafeAreaImpl.mm"
		};

		[MenuItem ("CocoPlay/Scripts/Clean Expired Plugins", false, 66)]
		private static void CleanExpiredPlugins ()
		{
			var cleaned = false;

			try {
				foreach (var path in _expiredPluginPaths) {
					cleaned |= DeletePlugin (path);
				}
			}
			catch (Exception e) {
				Debug.LogWarningFormat ("CocoCleanExpiredPlugins: {0}", e);
			}

			if (cleaned) {
				AssetDatabase.Refresh ();
			}
		}

		private static bool DeletePlugin (string path)
		{
			var fullPath = Path.Combine (Application.dataPath, path);

			if (File.Exists (fullPath)) {
				File.Delete (fullPath);
			} else if (Directory.Exists (fullPath)) {
				Directory.Delete (fullPath, true);
			} else {
				return false;
			}

			// delete meta
			var metaPath = fullPath + ".meta";
			if (File.Exists (metaPath)) {
				File.Delete (metaPath);
			}

			Debug.LogWarningFormat ("CocoCleanExpiredPlugins: plugin [{0}] cleaned", path);
			return true;
		}
	}
}