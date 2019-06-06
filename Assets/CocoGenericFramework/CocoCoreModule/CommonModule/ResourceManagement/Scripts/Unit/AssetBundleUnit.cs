using System.Collections.Generic;

namespace CocoPlay.ResourceManagement
{
	public class AssetBundleUnit : LoadHolderUnit<AssetBundleHolder, AssetBundleLoadRequest, AssetBundleData>
	{
		protected override AssetBundleHolder CreateHolder (AssetBundleData data)
		{
			var locationHolder = ResourceManager.GetLoadedLocationHolder (data.InLocation.Id);
			if (locationHolder == null) {
				ResourceDebug.Log ("{0}->CreateHolder: can NOT create asset bundle holder because location [{1}] not be loaded.",
					GetType ().Name, data.InLocation.Id);
				return null;
			}

			var dependencies = GetDependencies (data.DependencyDatas);

			return new AssetBundleHolder (data.Id, dependencies, locationHolder);
		}

		private List<AssetBundleHolder> GetDependencies (List<AssetBundleData> dependencyDatas)
		{
			if (dependencyDatas == null) {
				return null;
			}

			var dependencies = new List<AssetBundleHolder> (dependencyDatas.Count);
			foreach (var dependencyData in dependencyDatas) {
				var dependency = GetOrCreateHolder (dependencyData);
				dependencies.Add (dependency);
			}
			return dependencies;
		}
	}
}