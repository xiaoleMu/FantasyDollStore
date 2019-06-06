using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TabTale 
{
	public class GSDKConfigModel : ConfigModel<GSDKConfigData>
	{
		public List<BindingData> GetBindings()
		{
			GSDKConfigData data = _configs.FirstOrDefault(config => config.id == "Bindings") as GSDKConfigData;

			if(data != null)
			{
				return data.bindings;
			}

			return new List<BindingData>();
		}
	}
}
