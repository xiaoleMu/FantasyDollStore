using System;

namespace TabTale
{
	public class ConfigurationService
	{
		private Data.DataElement _connConfig;

		[PostConstruct]
		public void Init()
		{
			_connConfig = GameApplication.GetConfiguration("connection");
		}

		public string GetServerUrl()
		{
			string url = GetConfigParam("server");

			return url;
		}

		public string GetConfigParam(string key)
		{
			string paramValue = (_connConfig.IsNull || !_connConfig.ContainsKey(key)) ? "N/A" : (string)_connConfig[key];

			return paramValue;
		}
	}
}

