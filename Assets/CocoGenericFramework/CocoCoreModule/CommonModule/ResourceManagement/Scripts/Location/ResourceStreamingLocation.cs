using System.IO;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceStreamingLocation
	{
		#region Settings

		public static string FullRootPath {
			get { return Path.Combine (Application.streamingAssetsPath, ResourceSettings.ROOT_DIRECTORY); }
		}

		public static string GetAssetBundleFullRootPath (string platformDirectory)
		{
			return ResourceSettings.CombinePath (FullRootPath, platformDirectory, ResourceSettings.ASSET_BUNDLE_DIRECTORY);
		}

		public static string GetConfigFullFilePath (string platformDirectory)
		{
			return ResourceSettings.CombinePath (FullRootPath, platformDirectory, ResourceSettings.CONFIG_DIRECTORY,
				ResourceSettings.CONFIG_FILE_NAME);
		}

		#endregion


		#region Virtual

		private const string VIRTUAL_DIRECTORY = "streaming";

		public static string VirtualFullRootPath {
			get { return Path.Combine (ResourceSettings.VirtualFullRootPath, VIRTUAL_DIRECTORY); }
		}

		public static string GetVirtualAssetBundleFullRootPath (string platformDirectory)
		{
			return ResourceSettings.CombinePath (VirtualFullRootPath, platformDirectory, ResourceSettings.ASSET_BUNDLE_DIRECTORY);
		}


		public static string GetVirtualConfigFullFilePath (string platformDirectory)
		{
			return ResourceSettings.CombinePath (VirtualFullRootPath, platformDirectory, ResourceSettings.CONFIG_DIRECTORY, ResourceSettings.CONFIG_FILE_NAME);
		}

		#endregion


		#region Runtime

		public static string RuntimeFullRootPath {
			get { return Application.isEditor ? VirtualFullRootPath : FullRootPath; }
		}

		public static string RuntimeFullRootUri {
			get {
				var fullRootPath = RuntimeFullRootPath;
				return fullRootPath.Contains ("://") ? fullRootPath : "file://" + fullRootPath;
			}
		}

		public static string RuntimeAssetBundleFullRootPath {
			get { return ResourceSettings.CombinePath (RuntimeFullRootPath, ResourceManager.PlatformDirectory, ResourceSettings.ASSET_BUNDLE_DIRECTORY); }
		}

		public static string RuntimeConfigFullFileUri {
			get {
				return ResourceSettings.CombinePath (RuntimeFullRootUri, ResourceManager.PlatformDirectory,
					ResourceSettings.CONFIG_DIRECTORY, ResourceSettings.CONFIG_FILE_NAME);
			}
		}

		#endregion
	}
}