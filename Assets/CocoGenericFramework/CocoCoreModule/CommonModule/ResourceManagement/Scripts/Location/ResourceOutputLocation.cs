using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceOutputLocation
	{
		#region Settings

		public static string EditorConfigFullFilePath {
			get {
				return ResourceSettings.CombinePath (Application.dataPath, ResourceSettings.EDITOR_ROOT_DIRECTORY,
					"Editor", ResourceSettings.EDITOR_CONFIG_FILE_NAME);
			}
		}

		public static string FullRootPath {
			get {
				return ResourceSettings.CombinePath (Application.dataPath, ResourceSettings.EDITOR_ROOT_DIRECTORY,
					ResourceSettings.EDITOR_OUTPUT_DIRECTORY);
			}
		}

		public static string GetAssetBundleFullRootPath (string platformDirectory)
		{
			return ResourceSettings.CombinePath (FullRootPath, platformDirectory, ResourceSettings.ASSET_BUNDLE_DIRECTORY);
		}

		#endregion
	}
}