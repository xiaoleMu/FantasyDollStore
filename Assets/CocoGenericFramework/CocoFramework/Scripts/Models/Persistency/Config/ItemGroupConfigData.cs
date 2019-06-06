using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public enum ItemGroupConfigType{ Pack, Upgrades }

	public class ItemGroupConfigData : IConfigData
	{
		public string 						id				= "";
		public string 						name			= "";
		public string 						category		= "";
		public int 							index			= 1;
		public ItemGroupConfigType 			type			= ItemGroupConfigType.Pack;
		public List<string> 				items	 		= new List<string>();
		public List<GameElementData> 		restrictions 	= new List<GameElementData>();
		public List<AssetData> 				assets 			= new List<AssetData>();
		public List<GenericPropertyData> 	properties 		= new List<GenericPropertyData>();
		
		#region IConfigData implementation
		
		public string GetTableName ()
		{
			return "item_group_config";
		}
		
		public string GetId ()
		{
			return id;
		}
		
		public string ToLogString ()
		{
			string res = 
				@"ItemGroupConfigData: Id: {0}, Name: {1},  Category: {2}, Index: {3}
	Type: {4},
	Restrictions: {5},
	ItemIds: {6}
	Assets: {7},
	Properties: {8}";
			
			return string.Format(res, id,name,category,index,
			                     this.type,
			                     restrictions.ArrayString(),
			                     items.ArrayString(),
			                     assets.ArrayString(),
			                     properties.ArrayString());
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			ItemGroupConfigData c = new ItemGroupConfigData();
			c.id = id;
			c.name = name;
			c.category = category;
			c.index = index;
			c.type = type;
			c.items = new List<string>(items.Clone());
			c.restrictions = new List<GameElementData>(restrictions.Clone());
			c.assets = new List<AssetData>(assets.Clone());
			c.properties = new List<GenericPropertyData>(properties.Clone());
			
			return c;
		}
		
		#endregion
		
	}
}
