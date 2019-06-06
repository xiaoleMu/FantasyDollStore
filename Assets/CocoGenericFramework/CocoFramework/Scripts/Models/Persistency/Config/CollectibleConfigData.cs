using UnityEngine;
using System.Collections.Generic;

namespace TabTale
{
	public class CollectibleConfigData : IConfigData
	{
		public string id;
		public string name;
		public List<AssetData> assets = new List<AssetData>();
		public List<GenericPropertyData> properties = new List<GenericPropertyData>();

		#region ConfigData implementation
		public string GetTableName ()
		{
			return "collectible_config";
		}
		public string GetId ()
		{
			return id;
		}
		public string ToLogString ()
		{
			string res = @"CollectibleConfigData: Id:{0}, Name: {1}, Assets: {2}, Properties:{3}";
			return string.Format(res, id, name, assets.ArrayString(), properties.ArrayString());
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			CollectibleConfigData c = new CollectibleConfigData();
			c.id = id;
			c.name = name;
			c.assets = new List<AssetData>(assets.Clone());
			c.properties = new List<GenericPropertyData>(properties.Clone());

			return c;
		}
		#endregion
	}
}

