using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale {

	public static class CoreLogger
	{
		public interface ILoggerProvider
		{
			System.Action<string, string> LogTrace { get; }
			System.Action<string, string> LogInfo { get; }
			System.Action<string, string> LogDebug { get; }
			System.Action<string, string> LogNotice { get; }
			System.Action<string, string> LogWarning { get; }
			System.Action<string, string> LogError { get; }
			System.Action<string, string> LogCritical { get; }
		}

		public static bool Enabled = false;

		private static ILoggerProvider s_provider;

		private static System.Action<string, string> _logTraceModuleName = (module, msg) => { 
			Debug.Log(string.Format("{0}:Trace:{1}:{2}" ,Time.time, module, msg));
		};
		
		private static System.Action<int, string> _logTraceModuleNumber = (module, msg) => { 
			Debug.Log(string.Format("{0}:Trace:{1}:{2}" ,Time.time, s_modules[module].Name, msg));
		};
		
		private static System.Action<string, string> _logInfoModuleName = (module, msg) => { 
			Debug.Log(string.Format("{0}:Info:{1}:{2}" ,Time.time, module, msg));
		};
		
		private static System.Action<int, string> _logInfoModuleNumber = (module, msg) => { 
			Debug.Log(string.Format("{0}:Info:{1}:{2}" ,Time.time, s_modules[module].Name, msg));
		};
		
		private static System.Action<string, string> _logDebugModuleName = (module, msg) => { 
			Debug.Log(string.Format("{0}:Debug:{1}:{2}" ,Time.time, module, msg));
		};
		
		private static System.Action<int, string> _logDebugModuleNumber = (module, msg) => { 
			Debug.Log(string.Format("{0}:Debug:{1}:{2}" ,Time.time, s_modules[module].Name, msg));
		};
		
		private static System.Action<string, string> _logNoticeModuleName = (module, msg) => { 
			Debug.Log(string.Format("{0}:Notice:{1}:{2}" ,Time.time, module, msg));
		};
		
		private static System.Action<int, string> _logNoticeModuleNumber = (module, msg) => { 
			Debug.Log(string.Format("{0}:Notice:{1}:{2}" ,Time.time, s_modules[module].Name, msg));
		};
		
		private static System.Action<string, string> _logWarningModuleName = (module, msg) => { 
			Debug.LogWarning(string.Format("{0}:Warning:{1}:{2}" ,Time.time, module, msg));
		};
		
		private static System.Action<int, string> _logWarningModuleNumber = (module, msg) => { 
			Debug.LogWarning(string.Format("{0}:Warning:{1}:{2}" ,Time.time, s_modules[module].Name, msg));
		};
		
		private static System.Action<string, string> _logErrorModuleName = (module, msg) => { 
			Debug.LogError(string.Format("{0}:Warning:{1}:{2}" ,Time.time, module, msg));
		};
		
		private static System.Action<int, string> _logErrorModuleNumber = (module, msg) => { 
			Debug.LogError(string.Format("{0}:Warning:{1}:{2}" ,Time.time, s_modules[module].Name, msg));
		};
		
		private static System.Action<string, string> _logCriticalModuleName = (module, msg) => { 
			Debug.LogError(string.Format("{0}:Critical:{1}:{2}" ,Time.time, module, msg));
		};
		
		private static System.Action<int, string> _logCriticalModuleNumber = (module, msg) => { 
			Debug.LogError(string.Format("{0}:Critical:{1}:{2}" ,Time.time, s_modules[module].Name, msg));
		};

		public static ILoggerProvider Provider
		{
			get { return s_provider; }
			set
			{
				if(s_provider == value)
					return;

				SetProvider(value);
			}
		}

		private static void SetProvider(ILoggerProvider provider)
		{
			s_provider = provider;

			_logTraceModuleName = provider.LogTrace;
			_logTraceModuleNumber = (module, msg) => { 
				_logTraceModuleName(s_modules[module].Name, msg); 
			};

			_logInfoModuleName = provider.LogInfo;
			_logInfoModuleNumber = (module, msg) => { 
				_logInfoModuleName(s_modules[module].Name, msg); 
			};

			_logDebugModuleName = provider.LogDebug;
			_logDebugModuleNumber = (module, msg) => { 
				_logDebugModuleName(s_modules[module].Name, msg); 
			};

			_logNoticeModuleName = provider.LogNotice;
			_logNoticeModuleNumber = (module, msg) => { 
				_logNoticeModuleName(s_modules[module].Name, msg); 
			};

			_logWarningModuleName = provider.LogWarning;
			_logWarningModuleNumber = (module, msg) => { 
				_logWarningModuleName(s_modules[module].Name, msg); 
			};

			_logErrorModuleName = provider.LogError;
			_logErrorModuleNumber = (module, msg) => { 
				_logErrorModuleName(s_modules[module].Name, msg); 
			};

			_logCriticalModuleName = provider.LogCritical;
			_logCriticalModuleNumber = (module, msg) => { 
				_logCriticalModuleName(s_modules[module].Name, msg); 
			};
		}

		public static class Severity
		{
			/// <summary>
			/// This is used for tracing of functions - start, return etc.
			/// </summary>
			public const int Trace = 100;
			
			/// <summary>
			/// General, unimportant info
			/// </summary>
			public const int Info = 200;
			
			/// <summary>
			/// Info which can be used for tracking down bugs.
			/// </summary>
			public const int Debug = 300;
			
			/// <summary>
			/// Something we expected, but would like to know about.
			/// </summary>
			public const int Notice = 400;
			
			/// <summary>
			/// Something unexpected that may or not cause problems.
			/// </summary>
			public const int Warning = 500;
			
			/// <summary>
			/// Something unexpected that will definitely cause problems.
			/// </summary>
			public const int Error = 600;
			
			/// <summary>
			/// Something unexpectec that will cause problems now - basically, a crash.
			/// </summary>
			public const int Critical = 700;

			public static int ParseSeverity(string severity)
			{
				if(severity == "Trace")
					return Trace;

				if(severity == "Info")
					return Info;

				if(severity == "Debug")
					return Debug;

				if(severity == "Notice")
					return Notice;

				if(severity == "Warning")
					return Warning;

				if(severity == "Error")
					return Error;

				if(severity == "Critical")
					return Critical;

				return Trace;
			}
		}

		#region Trace

		[System.Diagnostics.Conditional("LOG_TRACE")]
		public static void LogTrace(string module, string msg)
		{
			_logTraceModuleName(module, msg);
		}

		[System.Diagnostics.Conditional("LOG_TRACE")]
		public static void LogTrace(int module, string msg)
		{
			_logTraceModuleNumber(module, msg);
		}

		[System.Diagnostics.Conditional("LOG_TRACE")]
		public static void LogTrace(string msg)
		{
			_logInfoModuleName(s_modules[0].Name, msg);
		}

		#endregion

		#region Info

		public static void LogInfo(string module, string msg)
		{
			if( !Enabled ) return;

			if(s_namesToModules[module].Threshold <= Severity.Info)
				_logInfoModuleName(module, msg);
		}

		public static void LogInfo(int module, string msg)
		{
			if( !Enabled ) return;

			if(s_modules[module].Threshold <= Severity.Info)
				_logInfoModuleNumber(module, msg);
		}

		public static void LogInfo(string msg)
		{
			if( !Enabled ) return;

			if(s_defaultModule.Threshold <= Severity.Info)
				_logInfoModuleName(s_modules[0].Name, msg);
		}

		#endregion

		#region Debug

		public static void LogDebug(string module, string msg)
		{
			if( !Enabled ) return;

			Module registeredModule = s_namesToModules[module];
			if(registeredModule.Threshold <= Severity.Debug)
				_logDebugModuleName(module, msg);
		}

		public static void LogDebug(int module, string msg)
		{
			if( !Enabled ) return;

			if(s_modules[module].Threshold <= Severity.Debug)
				_logDebugModuleNumber(module, msg);
		}

		public static void LogDebug(string msg)
		{
			if( !Enabled ) return;

			if(s_defaultModule.Threshold <= Severity.Debug)
				_logDebugModuleName(s_defaultModule.Name, msg);
		}

		#endregion

		#region Notice

		public static void LogNotice(string module, string msg)
		{
			if(s_namesToModules[module].Threshold <= Severity.Notice)
				_logNoticeModuleName(module, msg);
		}

		public static void LogNotice(int module, string msg)
		{
			if(s_modules[module].Threshold <= Severity.Notice)
				_logNoticeModuleNumber(module, msg);
		}

		public static void LogNotice(string msg)
		{
			if(s_defaultModule.Threshold <= Severity.Notice)
				_logNoticeModuleName(s_defaultModule.Name, msg);
		}

		#endregion

		#region Warning

		public static void LogWarning(string module, string msg)
		{
			if(s_namesToModules[module].Threshold <= Severity.Warning)
				_logWarningModuleName(module, msg);
		}

		public static void LogWarning(int module, string msg)
		{
			if(s_modules[module].Threshold <= Severity.Warning)
				_logWarningModuleNumber(module, msg);
		}

		public static void LogWarning(string msg)
		{
			if(s_defaultModule.Threshold <= Severity.Warning)
				_logWarningModuleName(s_defaultModule.Name, msg);
        }

		#endregion

		#region Error

		public static void LogError(string module, string msg)
		{
			if(s_namesToModules[module].Threshold <= Severity.Error)
				_logErrorModuleName(module, msg);
		}

		public static void LogError(int module, string msg)
		{
			if(s_modules[module].Threshold <= Severity.Error)
				_logErrorModuleNumber(module, msg);
		}

		public static void LogError(string msg)
		{
			if(s_defaultModule.Threshold <= Severity.Error)
				_logErrorModuleName(s_defaultModule.Name, msg);
		}

		#endregion

		#region Critical

		public static void LogCritical(string module, string msg)
		{
			if(s_namesToModules[module].Threshold <= Severity.Critical)
				_logCriticalModuleName(module, msg);
		}

		public static void LogCritical(int module, string msg)
		{
			if(s_modules[module].Threshold <= Severity.Critical)
				_logCriticalModuleNumber(module, msg);
		}

		public static void LogCritical(string msg)
		{
			if(s_defaultModule.Threshold <= Severity.Critical)
				_logCriticalModuleName(s_defaultModule.Name, msg);
		}

		#endregion

		public static void Log(int severity, string module, string msg)
		{
			if(severity >= Severity.Error)
			{
				LogError(module, msg);
			} else if(severity >= Severity.Warning)
			{
				LogWarning(module, msg);
			} else
			{
				LogDebug(module, msg);
			}
		}

		public static void Log(int severity, int module, string msg)
		{
			if(severity >= Severity.Error)
			{
				LogError(module, msg);
			} else if(severity >= Severity.Warning)
			{
				LogWarning(module, msg);
			} else
			{
				LogDebug(module, msg);
			}
		}

		public static void Log(string module, System.Exception ex)
		{
			LogError(module, ex.Message);
		}

		private const int MaxModules = 100;
		private static int s_numModules = 0;

		public static IEnumerable<string> Modules
		{
			get
			{
				for(int i=0;i<s_numModules;i++)
				{
					yield return s_modules[i].Name;
				}
			}
		}

		public static int GetThreshold(int module)
		{
			return s_modules[module].Threshold;
		}

		public static int GetThreshold(string module)
		{
			return s_namesToModules[module].Threshold;
		}

		public static void SetThreshold(int moduleIndex, int threshold)
		{
			s_modules[moduleIndex].Threshold = threshold;
		}

		public static void SetThreshold(string moduleName, int threshold)
		{
			if(!s_namesToModules.ContainsKey(moduleName))
			{
				int moduleIndex = RegisterModule(moduleName);
				s_modules[moduleIndex].Threshold = threshold;
				return;
			}

			Module registeredModule = s_namesToModules[moduleName];
			registeredModule.Threshold = threshold;
		}

		public static void SetThreshold(int threshold)
		{
			s_defaultModule.Threshold = threshold;
		}

		private class Module
		{
			public string Name;
			public int Threshold;
		}

		static Module s_defaultModule;
		static Module[] s_modules = new Module[MaxModules];
		static DefaultingDictionary<string, Module> s_namesToModules;

		public static readonly string DefaultModuleName = "Default";

		static CoreLogger()
		{
			s_defaultModule = new Module() { Name = DefaultModuleName};
			s_modules[0] = s_defaultModule;

			s_namesToModules = new DefaultingDictionary<string, Module>(s_defaultModule);
			s_namesToModules[DefaultModuleName] = s_defaultModule;
			s_numModules = 1;

			if(s_provider == null)
				SetProvider(new UnityLoggerProvider());
		}

		public static int RegisterModule(string moduleName, int defaultThreshold)
		{
			int numNow = s_numModules;

			int moduleIndex = RegisterModule(moduleName);
			if(s_numModules > numNow)
			{
				//first time
				SetThreshold(moduleName, defaultThreshold);
			}

			return moduleIndex;
		}

		public static int RegisterModule(string moduleName)
		{
			int i = s_modules.Where(m => m != null).FirstIndex(m => m.Name == moduleName);
			if(i != -1)
				return i;

			if(s_numModules >= MaxModules)
			{
				LogWarning("Logger", string.Format("unable to register module {0} - too many modules already!"));
				return 0;
			}

			LogDebug("Logger", string.Format("module {0} registered as {1}", moduleName, s_numModules));


			s_modules[s_numModules] = new Module() { Name = moduleName, Threshold = s_defaultModule.Threshold };
			s_namesToModules[moduleName] = s_modules[s_numModules];

			s_numModules++;

			return s_numModules - 1;
		}

	}
}
