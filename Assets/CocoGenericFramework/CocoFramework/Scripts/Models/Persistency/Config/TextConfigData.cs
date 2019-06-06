using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class TextConfigData : IConfigData
	{
		public string id;
		public string key;
		public string value;
		public string lang;

		#region IConfigData implementation
		public string GetTableName ()
		{
			return "text_config";
		}
		public string GetId ()
		{
			return id;
		}
		public string ToLogString ()
		{
			string res = 
				@"TextConfigData: Id:{0}, Key: {1}, Value: {2}, Lang:{3}";
			return string.Format(res, id,key,value,lang);
		}
		public bool IsBlob()
		{
			return false;
		}
		public IConfigData Clone ()
		{
			TextConfigData c = new TextConfigData();
			c.id = id;
			c.key = key;
			c.value = value;
			c.lang = lang;

			return c;
		}
		#endregion
		
	}

}