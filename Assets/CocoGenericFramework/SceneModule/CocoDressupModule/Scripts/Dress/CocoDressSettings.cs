using System.IO;
using CocoPlay.ResourceManagement;

namespace CocoPlay
{
	public class CocoDressSettings
	{
		// asset directory

		public const string DEFAULT_ORIGIN_ROOT_DIRECTORY = "_Game/CocoPlay/Role";
		public const string DEFAULT_ASSET_DIRECTORY = "role";
		public const string DEFAULT_OUTPUT_DIRECTORY = "output";
		public const string DEFAULT_CONFIG_DIRECTORY = "asset_config";

		public const string DEFAULT_CONFIG_FILE_NAME = "config_asset_global.json";

		// asset bundle
		public const string ASSET_BUNDLE_TAG_PREFIX = "dress_";
		public const string ASSET_BUNDLE_TAG_SHARE = ASSET_BUNDLE_TAG_PREFIX + "_s_h_a_r_e_";
		public const string ASSET_BUNDLE_TAG_CONFIG = ASSET_BUNDLE_TAG_PREFIX + "_c_o_n_f_i_g_";


		public static string EditorConfigFilePath {
			get { return ResourceSettings.CombinePath (DEFAULT_ORIGIN_ROOT_DIRECTORY, "Editor", "config_dress_editor.json"); }
		}

		public static string GetResourceTopDirectory (bool useAssetBundle)
		{
			return useAssetBundle ? ResourceSettings.RUNTIME_RESOURCES_DIRECTORY : ResourceSettings.DEFAULT_RESOURCES_DIRECTORY;
		}

		public static string GetDefaultOriginRootDirectory (bool useAssetBundle)
		{
			var resourceDirectory = GetResourceTopDirectory (useAssetBundle);
			return Path.Combine (DEFAULT_ORIGIN_ROOT_DIRECTORY, resourceDirectory);
		}

		public static string GetAssetBundleOutputDirectory ()
		{
			return Path.Combine (ResourceSettings.EDITOR_ROOT_DIRECTORY, ResourceSettings.EDITOR_OUTPUT_DIRECTORY);
		}
	}
}