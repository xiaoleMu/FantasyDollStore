using System;

namespace TabTale
{
	public class IAPData : ICloneable
	{
		public StoreType store;
		public string iapId;

		public IAPData() { }
		public IAPData(StoreType store, string iapId)
		{
			this.store = store;
			this.iapId = iapId;
		}

		#region ICloneable implementation
		public object Clone ()
		{
			IAPData c = new IAPData();
			c.store = store;
			c.iapId = iapId;
			return c;
		}
		#endregion

		public override string ToString ()
		{
			return string.Format ("[IAPData: Store={0}, iapId={1}]", store.ToString(), iapId);
		}
	}
}