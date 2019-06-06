using System;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class ItemData : ICloneable
	{
		public string 	itemConfigId;
		public int 		quantity;
		public string 	category;
		public bool 	inUse;
		public List<GenericPropertyData> properties = new List<GenericPropertyData>();

		#region ICloneable implementation

		public object Clone ()
		{
			ItemData c = new ItemData();
			c.itemConfigId = itemConfigId;
			c.quantity = quantity;
			c.category = category;
			c.inUse = inUse;
			c.properties = new List<GenericPropertyData>(properties.Clone());

			return c;
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[ItemData: ItemConfigId={0}, Quantity={1}, Catergory={2}, InUse={3}, Properties={4}]", itemConfigId, quantity, category, inUse, properties);
		}
	}
}
