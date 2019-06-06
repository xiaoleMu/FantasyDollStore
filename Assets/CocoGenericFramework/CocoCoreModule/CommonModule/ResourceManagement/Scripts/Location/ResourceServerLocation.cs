using System.IO;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceServerLocation
	{
		#region Settings

		private static string _rootUri = string.Empty;

		public static string AssetBundleFullRootUri {
			get { return ResourceSettings.CombinePath (RuntimeRootUri, ResourceManager.PlatformDirectory, ResourceSettings.ASSET_BUNDLE_DIRECTORY); }
		}

		#endregion


		#region Virtual

		private const string VIRTUAL_DIRECTORY = "server";

		public static string VirtualFullRootPath {
			get { return Path.Combine (ResourceSettings.VirtualFullRootPath, VIRTUAL_DIRECTORY); }
		}

		private static string VirtualRootUri {
			get { return "file://" + VirtualFullRootPath; }
		}

		public static string GetVirtualAssetBundleFullRootPath (string platformDirectory)
		{
			return ResourceSettings.CombinePath (VirtualFullRootPath, platformDirectory, ResourceSettings.ASSET_BUNDLE_DIRECTORY);
		}

		#endregion


		#region Runtime

		public static string RuntimeRootUri {
			get {
				if (string.IsNullOrEmpty (_rootUri)) {
					ResourceDebug.Log ("ResourceWebLocation->RootUri: uri NOT be set !");
					return string.Empty;
				}

				return Application.isEditor ? VirtualRootUri : _rootUri;
			}
			set { _rootUri = value; }
		}

		#endregion
	}
}