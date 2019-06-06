using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	public class CommandLineParser
	{
		private string[] _arguments;
		public CommandLineParser()
		{
			_arguments = System.Environment.GetCommandLineArgs();

			foreach(string arg in _arguments)
			{
				string[] split = arg.Split('=');
				if(split != null)
				{
					if(split.Length == 2)
					{
						_keyValues[split[0]] = split[1];
					}
				}
			}
		}

		private IDictionary<string, string> _keyValues = new Dictionary<string, string>();
		public string this[string key]
		{
			get 
			{
				string val;
				if(_keyValues.TryGetValue(key, out val))
					return val;
				return null ;
			}
		}
	}
}
