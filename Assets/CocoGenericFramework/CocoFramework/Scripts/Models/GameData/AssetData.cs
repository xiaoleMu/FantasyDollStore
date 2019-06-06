using System;

namespace TabTale
{
	public enum AssetType
	{
		Image,
		Prefab,
		String,
		ServerImage
	}

	public class AssetData : ICloneable
	{
		public string id;
		public AssetType assetType;
		public string value;

		public AssetData() { }
		public AssetData(string id, AssetType type, string value)
		{
			this.id = id;
			this.assetType = type;
			this.value = value;
		}

		#region ICloneable implementation
		public object Clone ()
		{
			AssetData c = new AssetData();
			c.id = id;
			c.assetType = assetType;
			c.value = value;
			return c;
		}
		#endregion

		public override string ToString ()
		{
			return string.Format ("[AssetData: Id={0}, Type={1}, Value={2}]", id, assetType, value);
		}
	}
}
