using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class CategoryConfigData : IConfigData
	{
		public string id ;
		public string name ;
		public List<GameElementData> restrictions = new List<GameElementData>();
		public List<AssetData> assets = new List<AssetData>();
		public List<GenericPropertyData> properties = new List<GenericPropertyData>();
		public int index ;
		
		#region IConfigData implementation
		
		public string GetTableName ()
		{
			return "category_config";
		}
		
		public string GetId ()
		{
			return id;
		}
		
		public string ToLogString ()
		{
			string res = 
				@"CategoryConfigData: Id: {0}, Name: {1}, 
	Index: {2}
	Restrictions: {3},
	Assets: {4},
	Properties: {5}";
			
			return string.Format(res, id,name,index,
			                     restrictions.ArrayString(),
			                     assets.ArrayString(),
			                     properties.ArrayString());
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			CategoryConfigData c = new CategoryConfigData();
			c.id = id;
			c.name = name;
			c.index = index;
			c.restrictions = new List<GameElementData>(restrictions.Clone());
			c.assets = new List<AssetData>(assets.Clone());
			c.properties = new List<GenericPropertyData>(properties.Clone());

			return c;
		}
		
		#endregion
		

		
		
	}
}