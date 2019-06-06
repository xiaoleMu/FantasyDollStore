using UnityEngine;
using System.Collections;

namespace TabTale {

	public class UnityLoggerProvider : CoreLogger.ILoggerProvider
	{
		public System.Action<string, string> LogTrace
		{
			get 
			{ 
				return (module, msg) => 
				{ 
					Debug.Log(string.Format("{0}:Trace:                  {1}:{2}" ,Time.time, module, msg));
				};
			}
		}	
		
		public System.Action<string, string> LogInfo
		{
			get 
			{ 
				return (module, msg) => 
				{ 
					Debug.Log(string.Format("{0}:Info:                  {1}:{2}" ,Time.time, module, msg));
				};
			}
		}	

		public System.Action<string, string> LogDebug
		{
			get 
			{ 
				return (module, msg) => 
				{ 
					Debug.Log(string.Format("{0}:Debug:                  {1}:{2}" ,Time.time, module, msg));
				};
			}
		}	

		public System.Action<string, string> LogNotice
		{
			get 
			{ 
				return (module, msg) => 
				{ 
					Debug.Log(string.Format("{0}:Notice:                  {1}:{2}" ,Time.time, module, msg));
				};
			}
		}	

		public System.Action<string, string> LogWarning
		{
			get 
			{ 
				return (module, msg) => 
				{ 
					Debug.LogWarning(string.Format("{0}:Warning:                  {1}:{2}" ,Time.time, module, msg));
				};
			}
		}	

		public System.Action<string, string> LogError
		{
			get 
			{ 
				return (module, msg) => 
				{ 
					Debug.LogError(string.Format("{0}:Error:                  {1}:{2}" ,Time.time, module, msg));
				};
			}
		}	

		public System.Action<string, string> LogCritical
		{
			get 
			{ 
				return (module, msg) => 
				{ 
					Debug.LogError(string.Format("{0}:Critical:                  {1}:{2}" ,Time.time, module, msg));
				};
			}
		}	


	}
}
