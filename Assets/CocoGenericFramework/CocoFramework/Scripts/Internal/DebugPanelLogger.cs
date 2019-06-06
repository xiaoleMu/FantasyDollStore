using System;
using UnityEngine;

namespace TabTale
{
	public class DebugPanelLogger : ILogHandler
	{
		public delegate void LogDelegate(LogType logType, UnityEngine.Object context, string format, params object[] args);

		public event LogDelegate OnLogEvent = (t,c,f,a) => {};

		//private StreamWriter m_StreamWriter;
#if UNITY_2017_1_OR_NEWER
		private ILogHandler m_DefaultLogHandler = Debug.unityLogger.logHandler;
#else
		private ILogHandler m_DefaultLogHandler = Debug.logger.logHandler;
#endif

		public DebugPanelLogger()
		{
#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.logHandler = this;
#else
			Debug.logger.logHandler = this;
#endif
		}

		public void LogFormat (LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			//m_StreamWriter.WriteLine ( String.Format (format, args) );
			//m_StreamWriter.Flush ();
			m_DefaultLogHandler.LogFormat (logType, context, format, args);

			OnLogEvent(logType, context, format, args);

			//m_StreamWriter = new StreamWriter ();
			//m_StreamWriter.
		}

		public void LogException (Exception exception, UnityEngine.Object context)
		{
			m_DefaultLogHandler.LogException (exception, context);
		}
	}
}

