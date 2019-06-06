using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TabTale 
{
	public class GeneralParameterConfigModel : ConfigModel<GeneralParameterConfigData>
	{
		public string GetString(string id, string defaultValue="")
		{
			GeneralParameterConfigData data = _configs.FirstOrDefault(config => config.id == id) as GeneralParameterConfigData;
			if (data == null)
			{
				CoreLogger.LogWarning(_loggerModule, "GetString: id=\"" + id + "\" not found. Returning default value.");
				return defaultValue;
			}
			if (data.type != "string")
			{
				CoreLogger.LogWarning(_loggerModule, "GetString: called for id=\"" + id + "\" that stored as type=" + data.type + ". Returning default value.");
				return defaultValue;
			}
			return data.value;
		}
		public int GetInt(string id, int defaultValue=0)
		{
			GeneralParameterConfigData data = _configs.FirstOrDefault(config => config.id == id) as GeneralParameterConfigData;
			if (data == null)
			{
				CoreLogger.LogWarning(_loggerModule, "GetInt: id=\"" + id + "\" not found. Returning default value.");
				return defaultValue;
			}
			if (data.type != "int")
			{
				CoreLogger.LogWarning(_loggerModule, "GetInt: called for id=\"" + id + "\" that stored as type=" + data.type + ". Returning default value.");
				return defaultValue;
			}
			int result = defaultValue;
			if (!Int32.TryParse(data.value, out result))
				return defaultValue;
			return result;
		}

		public float GetFloat(string id, float defaultValue=0f)
		{
			GeneralParameterConfigData data = _configs.FirstOrDefault(config => config.id == id) as GeneralParameterConfigData;
			if (data == null)
			{
				CoreLogger.LogWarning(_loggerModule, "GetFloat: id=\"" + id + "\" not found. Returning default value.");
				return defaultValue;
			}
			if (data.type != "float")
			{
				CoreLogger.LogWarning(_loggerModule, "GetFloat: called for id=\"" + id + "\" that stored as type=" + data.type + ". Returning default value.");
				return defaultValue;
			}
			float result = defaultValue;
			if (!float.TryParse(data.value, out result))
				return defaultValue;
			return result;
		}

		public bool GetBool(string id, bool defaultValue=false)
		{
			GeneralParameterConfigData data = _configs.FirstOrDefault(config => config.id == id) as GeneralParameterConfigData;
			if (data == null)
			{
				CoreLogger.LogWarning(_loggerModule, "GetBool: id=\"" + id + "\" not found. Returning default value.");
				return defaultValue;
            }
            if (data.type != "bool")
            {
                CoreLogger.LogWarning(_loggerModule, "GetBool: called for id=\"" + id + "\" that stored as type=" + data.type + ". Returning default value.");
                return defaultValue;
            }
            bool result = defaultValue;
            if (!Boolean.TryParse(data.value, out result))
                return defaultValue;
            return result;
        }
    }
}
