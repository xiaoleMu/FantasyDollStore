using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class InventoryStateData : IStateData
	{
		public List<ItemData> items = new List<ItemData>();
		#region IStateData implementation
		
		public string GetStateName ()
		{
			return "inventoryState";
		}
		
		public string ToLogString ()
		{
			string log = "InventoryStateData: ";
			foreach(ItemData item in items)
				log += item.ToString() + "\n";
			return log;
		}
		
		public IStateData Clone ()
		{
			InventoryStateData c = new InventoryStateData();
			c.items = new List<ItemData>(items.Clone());
			return c;
		}
		
		#endregion
	}
}