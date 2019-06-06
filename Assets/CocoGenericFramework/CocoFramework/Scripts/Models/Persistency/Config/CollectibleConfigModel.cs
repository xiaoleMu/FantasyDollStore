using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public class CollectibleConfigModel : ConfigModel<CollectibleConfigData>
	{
		#region Getters
		public string GetName(string id)
		{
			return _configs.FirstOrDefault(x => x.GetId() == id).name;
		}
		public List<AssetData> GetAssets(string id)
		{
			return _configs.FirstOrDefault(x => x.GetId() == id).assets;
		}

		public List<GenericPropertyData> GetProperies(string id)
		{
			return _configs.FirstOrDefault(x => x.GetId() == id).properties;
		}

		public List<CollectibleConfigData> GetAllCollectible()
		{
			return _configs.OrderBy(c => c.id).ToList();
		}
		#endregion
	}
}

