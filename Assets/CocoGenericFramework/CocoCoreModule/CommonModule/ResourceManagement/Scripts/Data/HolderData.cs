using System.Collections.Generic;
using LitJson;

namespace CocoPlay.ResourceManagement
{
	public class HolderData
	{
		public string Id = string.Empty;
	}


	public class AssetBundleData : HolderData
	{
		public List<ResourceData> ResourceDatas = new List<ResourceData> ();

		public List<SceneData> SceneDatas = new List<SceneData> ();

		public List<string> Dependencies = new List<string> ();

		[JsonIgnore]
		public List<AssetBundleData> DependencyDatas;

		[JsonIgnore]
		public LocationData InLocation;
	}


	public class LocationData : HolderData
	{
		public LocationType Location;

		public List<AssetBundleData> AssetBundleDatas = new List<AssetBundleData> ();
	}
}