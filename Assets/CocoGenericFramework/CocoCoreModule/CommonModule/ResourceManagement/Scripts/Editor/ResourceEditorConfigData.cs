using System.Collections.Generic;
using LitJson;
using UnityEditor;

namespace CocoPlay.ResourceManagement
{
	public enum CompressOption
	{
		Uncompressed = 0,
		StandardLZMA,
		ChunkBasedLZ4,
	}


	public class ResourceEditorConfigData
	{
		public List<ResourceEditorPlatformData> PlatformDatas = new List<ResourceEditorPlatformData> ();

		public int CurrPlatformIndex = 0;
	}


	public class ResourceEditorPlatformData
	{
		public string Platform = string.Empty;

		public ResourceEditorAssetBundleBuildOptions AssetBundleBuildOptions = new ResourceEditorAssetBundleBuildOptions ();
		public ResourceEditorConfigGenerateOptions ConfigGenerateOptions = new ResourceEditorConfigGenerateOptions ();
		public ResourceEditorTargetCopyOptions TargetCopyOptions = new ResourceEditorTargetCopyOptions ();

		[JsonIgnore]
		public ConfigData ConfigData;

		[JsonIgnore]
		public readonly ResourceEditorFoldout<ResourceEditorFoldout<ResourceEditorFoldout<bool>>> ConfigFoldout =
			new ResourceEditorFoldout<ResourceEditorFoldout<ResourceEditorFoldout<bool>>> ();

		[JsonIgnore]
		public List<ResourceEditorAssetBundleData> EditorAssetBundleDatas;
	}


	public class ResourceEditorAssetBundleData
	{
		public string Id = string.Empty;
		public LocationType Location = LocationType.Streaming;
		public string OdrTag = "odr";
	}


	public class ResourceEditorFoldout<T>
	{
		public bool IsOut = false;
		public readonly Dictionary<string, T> Contents = new Dictionary<string, T> ();
	}


	public class ResourceEditorAssetBundleBuildOptions
	{
		public BuildTarget BuildTarget = BuildTarget.iOS;
		public CompressOption CompressOption = CompressOption.ChunkBasedLZ4;
		public bool ClearFolders = true;
	}


	public class ResourceEditorTargetCopyOptions
	{
		public bool ClearFolders = true;
		public bool ClearOtherPlatforms = false;
	}


	public class ResourceEditorConfigGenerateOptions
	{
		public bool CollectSprite = true;
	}
}