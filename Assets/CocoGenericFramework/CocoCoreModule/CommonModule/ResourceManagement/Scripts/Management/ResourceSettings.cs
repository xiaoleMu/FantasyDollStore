using System.IO;
using System.Linq;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceSettings
	{
		#region Global

		// resource root directory
		public const string ROOT_DIRECTORY = "_resources";

		// config directory
		public const string CONFIG_DIRECTORY = "config";

		// asset bundle directory
		public const string ASSET_BUNDLE_DIRECTORY = "assetbundle";

		// virtual root directory (editor only)
		public const string VIRTUAL_ROOT_DIRECTORY = "virtual";

		// runtime resources directory (should contain in asset bundle)
		public const string RUNTIME_RESOURCES_DIRECTORY = "RuntimeResources";

		// default resources directory (use by Resources.Load)
		public const string DEFAULT_RESOURCES_DIRECTORY = "Resources";

		// resource config file
		public const string CONFIG_FILE_NAME = "resource_config.json";

		#endregion


		#region Key

		public const string PLATFORM_DIRECTORY_IOS = "ios";

		public const string PLATFORM_DIRECTORY_GP = "google";

		public const string LOCATION_ID_STREAMING = "streaming";

		public const string LOCATION_ID_SERVER = "server";

		#endregion


		#region Editor Output

		// resource editor directory
		public const string EDITOR_ROOT_DIRECTORY = "_Resources";

		// output directory (asset bundle and config)
		public const string EDITOR_OUTPUT_DIRECTORY = "output";

		// resource editor config file
		public const string EDITOR_CONFIG_FILE_NAME = "editor_resource_config.json";

		#endregion


		#region Helper

		public static string VirtualRootPath {
			get { return CombinePath ("Assets", EDITOR_ROOT_DIRECTORY, VIRTUAL_ROOT_DIRECTORY); }
		}

		public static string VirtualFullRootPath {
			get { return CombinePath (Application.dataPath, EDITOR_ROOT_DIRECTORY, VIRTUAL_ROOT_DIRECTORY); }
		}

		public static string CombinePath (string rootPath, params string[] subPaths)
		{
			//return subPaths.Aggregate (rootPath, Path.Combine);
			return subPaths.Aggregate (rootPath, (current, subPath) => string.Format ("{0}{1}{2}", current, Path.DirectorySeparatorChar, subPath));
		}

		#endregion
	}
}