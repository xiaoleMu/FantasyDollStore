using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class GSDKConfigData : IConfigData 
	{	
		public string id;
		public List<BindingData> bindings = new List<BindingData>();
		
		#region IConfigData implementation
		
		public string GetTableName ()
		{
			return "gsdk_config";
		}

		public string GetId ()
		{
			return id;
		}

		public List<BindingData> GetBindings ()
		{
			return bindings;
		}
		
		public string ToLogString ()
		{
			return string.Format ("GsdkConfigData: Bindings:{0}", bindings.ArrayString());
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			GSDKConfigData c = new GSDKConfigData();
			c.id = id;
			c.bindings = new List<BindingData>(bindings.Clone());
			
			return c;
		}
		
		#endregion
		
		
	}
}