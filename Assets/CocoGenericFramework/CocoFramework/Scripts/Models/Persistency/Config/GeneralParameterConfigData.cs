using UnityEngine;
using System.Collections;

namespace TabTale
{
public class GeneralParameterConfigData : IConfigData {

		public string id;
		public string type;
		public string value;
		public string description;


		#region IConfigData implementation

		public string GetTableName ()
		{
			return "general_parameter_config";
		}

		public string GetId ()
		{
			return id;
		}

		public string ToLogString ()
		{
			return string.Format ("GeneralParameterConfigData: Id:{0}, Type:{1}, Value:{2}, Description:{3}", id, type, value, description);
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			GeneralParameterConfigData c = new GeneralParameterConfigData();
			c.id = id;
			c.type = type;
			c.value = value;
			c.description = description;

			return c;
		}

		#endregion


}
}