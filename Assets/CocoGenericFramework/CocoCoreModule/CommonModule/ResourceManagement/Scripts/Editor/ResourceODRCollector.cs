#if UNITY_IOS
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.iOS;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceODRCollector
	{
		[InitializeOnLoadMethod]
		private static void SetupResourcesBuild ()
		{
			UnityEditor.iOS.BuildPipeline.collectResources += CollectResources;
		}

		private static Resource[] CollectResources ()
		{
			var configPath = ResourceStreamingLocation.GetConfigFullFilePath (ResourceSettings.PLATFORM_DIRECTORY_IOS);
			var configData = CocoData.LoadFromJsonFile<ConfigData> (configPath);
			if (configData == null) {
				Debug.Log ("ResourceODRCollector->CollectResources: resource config NOT found, will don't use ODR.");
				return new Resource[0];
			}

			var resources = new List<Resource> ();

			foreach (var locationData in configData.LocationDatas) {
				if (locationData.Location != LocationType.ODR) {
					continue;
				}

				var odrTag = locationData.Id;
				var directory = ResourceODRLocation.GetVirtualTagPath (odrTag);
				foreach (var assetBundleData in locationData.AssetBundleDatas) {
					CollectAssetBundle (assetBundleData, directory, odrTag, resources);
				}
			}

			return resources.ToArray ();
		}

		private static void CollectAssetBundle (AssetBundleData assetBundleData, string directory, string odrTag, List<Resource> resources)
		{
			var assetBundleName = assetBundleData.Id;
			var path = Path.Combine (directory, assetBundleName);
			resources.Add (new Resource (assetBundleName, path).AddOnDemandResourceTags (odrTag));

			//manifest
			var manifestName = assetBundleName + ".manifest";
			var manifestPath = string.Format ("{0}/{1}", directory, manifestName);
			resources.Add (new Resource (manifestName, manifestPath).AddOnDemandResourceTags (odrTag));

			Debug.LogFormat ("ResourceODRCollector->CollectResources: resource name [{0}], path [{1}], odrTag [{2}]", assetBundleName, path, odrTag);
		}
	}
}
#endif