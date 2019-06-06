using LitJson;

namespace CocoPlay.ResourceManagement
{
	public class ResourceData
	{
		public string Path = string.Empty;

		public string Type = string.Empty;

		[JsonIgnore]
		public AssetBundleData InAssetBundle;
	}


	public class SceneData
	{
		public string SceneName = string.Empty;

		[JsonIgnore]
		public AssetBundleData InAssetBundle;
	}
}