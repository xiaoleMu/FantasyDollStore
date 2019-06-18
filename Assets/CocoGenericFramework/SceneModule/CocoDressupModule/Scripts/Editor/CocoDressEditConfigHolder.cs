using System.Collections.Generic;

namespace CocoPlay
{
	public class CocoDressEditorConfigHolder
	{
		// directory
		public string originRootDirectory = CocoDressSettings.GetDefaultOriginRootDirectory (false);
		public string assetDirectory = CocoDressSettings.DEFAULT_ASSET_DIRECTORY;
		public string configDirectory = CocoDressSettings.DEFAULT_CONFIG_DIRECTORY;

		// settings
		public string globalConfigFileName = CocoDressSettings.DEFAULT_CONFIG_FILE_NAME;
		public bool prettyPrint = false;
		public bool randomSorting = true;

		// asset bundle
		public bool useAssetBundle = false;

		// global config
		// dress cover layers
		public Dictionary<string, string> dressCategoryCoverLayerIds = new Dictionary<string, string> ();
		public Dictionary<string, string> dressItemPrefixCoverLayerIds = new Dictionary<string, string> ();

		// main category (will record and resume)
		public List<string> dressMainCategoryIds = new List<string> ();

		// role
		public List<CocoDressEditorRoleDressConfigHolder> roleDressConfigHolders = new List<CocoDressEditorRoleDressConfigHolder> ();
		public List<CocoDressEditorRoleBodyConfigHolder> roleBodyConfigHolders = new List<CocoDressEditorRoleBodyConfigHolder> ();
		public List<CocoDressEditorRoleConfigHolder> roleConfigHolders = new List<CocoDressEditorRoleConfigHolder> ();
	}


	public class CocoDressEditorRoleDressConfigHolder
	{
		public string dressId = "common";
		public List<CocoDressEditorRoleDressSceneConfigHolder> sceneConfigHolders = new List<CocoDressEditorRoleDressSceneConfigHolder> ();
		public bool resetRandomSeed = true;
	}


	public class CocoDressEditorRoleDressSceneConfigHolder
	{
		public string sceneId = "common";
		public List<string> itemIdPrefixs = new List<string> ();
	}


	public class CocoDressEditorRoleBodyConfigHolder
	{
		public string bodyId = "common";
		public Dictionary<string, string> boneNames = new Dictionary<string, string> ();
		public Dictionary<string, string> rendererNames = new Dictionary<string, string> ();
	}


	public class CocoDressEditorRoleConfigHolder
	{
		public string roleId = "common";
		public string boneItemId = "player";
		public List<string> basicItemIds = new List<string> ();
		public string dressId = "common";
		public string bodyId = "common";
		public bool enableShadow = false;
	}


	public class CocoDressEditorAssetBundleConfigHolder
	{
		public string platformDirectory;
		public bool shouldGenerated = true;
		public Dictionary<string, CocoAssetBundleLocation> assetBundleLocations = new Dictionary<string, CocoAssetBundleLocation> ();
	}
}