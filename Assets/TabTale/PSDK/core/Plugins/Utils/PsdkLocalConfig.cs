using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsdkLocalConfig {

	private Dictionary<string,object> _psdkConfig; 

	public PsdkLocalConfig(string configJson)
	{
		_psdkConfig = TabTale.Plugins.PSDK.Json.Deserialize (configJson) as Dictionary<string,object>;
		if(_psdkConfig == null){
			Debug.LogError("failed to parse local config json.");
		}
	}

	public string GetString(string[] path, string def)
	{
		object objValue = Get(path);
		if(objValue != null && objValue.GetType() == typeof(string)){
			return (string)objValue;
		}
		return def;
	}

	public long GetLong(string[] path, long def)
	{
		object objValue = Get(path);
		if(objValue != null && objValue.GetType() == typeof(long)){
			return (long)objValue;
		}
		return def;
	}

	public bool GetBool(string[] path, bool def)
	{
		object objValue = Get(path);
		if(objValue != null && objValue.GetType() == typeof(bool)){
			return (bool)objValue;
		}
		return def;
	}

	public List<object> GetArray(string[] path)
	{
		object objValue = Get(path);
		if(objValue != null && objValue.GetType() == typeof(List<object>)){
			return (List<object>)objValue;
		}
		return null;
	}

	public object Get(string[] path)
	{
		return Get(path,_psdkConfig,0);
	}

	private object Get(string[] path, object obj, int i)
	{
		if(i == path.Length-1){
			if(obj.GetType() == typeof(Dictionary<string,object>)){
				object valObject = null;
				if(((Dictionary<string,object>)obj).TryGetValue(path[i], out valObject)){
					return valObject;
				}
			}
			return null;
		}
		else {
			if(obj.GetType() == typeof(Dictionary<string,object>)){
				object valObject = null;
				if(((Dictionary<string,object>)obj).TryGetValue(path[i], out valObject)){
					return Get(path,valObject,i+1);
				}
			}
			return null;
		}

	}


}
