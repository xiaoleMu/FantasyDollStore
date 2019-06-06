using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace TabTale
{
	public class ItemConfigData : IConfigData
	{
		public string 						id					= "";
		public string						name				= "";
		public string 						iapConfigId			= "";
		public int 							quantity			= 1;
		public string 						category			= "";
		public int 							index				= 1;
		public string		 				currencyKey			= ""; // DEPRECATED FIELD
		public ItemType						itemType			= ItemType.Item;
		public string						itemTypeKey			= "";
		public List<GameElementData> 		costs 				= new List<GameElementData>();
		public List<GameElementData> 		alternativeCosts 	= new List<GameElementData>();
		public List<GameElementData> 		restrictions 		= new List<GameElementData>();
		public List<AssetData> 				assets 				= new List<AssetData>();
		public List<GenericPropertyData> 	properties 			= new List<GenericPropertyData>();
		
		#region IConfigData implementation
		
		public string GetTableName ()
		{
			return "item_config";
		}
		
		public string GetId ()
		{
			return id;
		}
		
		public string ToLogString ()
		{
			string res = 
				@"ItemConfigData: Id: {0}, Name: {1}, IAPConfigID: {2}, Quantity: {3}, Category: {4}, Index: {5},
	CurrencyKey: {6},
	ItemType: {7},
	ItemKeyType: {8}
	Cost: {9},
	AlternativeCost: {10},
	Restrictions: {11},
	Assets: {12},
	Properties: {13}";
			
			return string.Format(res, id,name,iapConfigId,quantity,category,index,
				currencyKey, 
				itemType,
				itemTypeKey,
			    costs.ArrayString(),
			    alternativeCosts.ArrayString(),
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
			ItemConfigData c = new ItemConfigData();
			c.id = id;
			c.name = name;
			c.iapConfigId = iapConfigId;
			c.quantity = quantity;
			c.category = category;
			c.index = index;
			c.currencyKey = currencyKey;
			c.itemType = itemType;
			c.itemTypeKey = itemTypeKey;
			c.costs = new List<GameElementData>(costs.Clone());
			c.alternativeCosts = new List<GameElementData>(alternativeCosts.Clone());
			c.restrictions = new List<GameElementData>(restrictions.Clone());
			c.assets = new List<AssetData>(assets.Clone());
			c.properties = new List<GenericPropertyData>(properties.Clone());

			return c;
		}
		
		#endregion

	}
}
