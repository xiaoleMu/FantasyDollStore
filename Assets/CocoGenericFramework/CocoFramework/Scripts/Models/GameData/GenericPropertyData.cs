using System;
namespace TabTale
{
	public enum PropertyType
	{
		Bool,
		Float,
		Image,
		Int,
		Prefab,
		String,
	}

	public class GenericPropertyData : ICloneable
	{
		public string id;

		public PropertyType type;

		public string value;

		public GenericPropertyData() { }

		public GenericPropertyData(string id, PropertyType type, string value) : this()
		{
			this.id = id;
			this.type = type;
			this.value = value;
		}

		public object Clone()
		{
			var c = new GenericPropertyData(id,type,value);
			return c;
		}

		public override string ToString ()
		{
			return string.Format ("[GenericPropertyData: Id={0}, Type={1}, Value={2}]", id, type, value);
		}
	}
}
