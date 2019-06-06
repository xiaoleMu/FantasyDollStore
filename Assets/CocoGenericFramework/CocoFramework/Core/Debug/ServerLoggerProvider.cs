using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using TabTale.Data;

namespace TabTale
{
	public class ServerLoggerProvider : MonoBehaviour, CoreLogger.ILoggerProvider
	{

		public  const string IP_KEY = "ServerLoggerProvider_IP_KEY";
		public  const string PORT_KEY = "ServerLoggerProvider_PORT_KEY";
		private CoreLogger.ILoggerProvider _oldProvider;
		private System.Action<string, string> _oldLogTrace;
		private System.Action<string, string> _oldLogInfo;
		private System.Action<string, string> _oldLogDebug;
		private System.Action<string, string> _oldLogNotice;
		private System.Action<string, string> _oldLogWarning;
		private System.Action<string, string> _oldLogError;
		private System.Action<string, string> _oldLogCritical;

		private System.Action<string, string> BuildLogMethod (string severity, bool passthrough, System.Action<string, string> fallback)
		{
			//if (_socket == null)
			//	return fallback;

			string format = "+log|TabTaleLog|{0}|" + severity + "|{1}:{2}\r\n";

			if (passthrough) {
				return (module, msg) =>
				{
					if(_socket == null)
					{
						fallback(module, msg);
						return;
					}

					try {
						_socket.Send (string.Format (format, module, Time.time, msg).ToByteArray ());
					} catch (System.Net.Sockets.SocketException ex) {
						Debug.Log("ServerLoggerProvider: socket.Send fail 1 "+ex.Message);
					};
					fallback (module, msg);
				};
			} else {
				return (module, msg) => 
				{
					if(_socket == null)
						return;

					try {
						_socket.Send (string.Format (format, module, Time.time, msg).ToByteArray ());	
					} catch (System.Net.Sockets.SocketException ex) {
						Debug.Log("ServerLoggerProvider: socket.Send fail 2 "+ex.Message);
					};
				};
			}
			;
		}

		public System.Action<string, string> LogTrace { 
			get { return BuildLogMethod ("trace", passthrough, _oldLogTrace); }
		}

		public System.Action<string, string> LogInfo { 
			get { return BuildLogMethod ("info", passthrough, _oldLogInfo); }
		}

		public System.Action<string, string> LogDebug { 
			get { return BuildLogMethod ("debug", passthrough, _oldLogDebug); }
		}

		public System.Action<string, string> LogNotice { 
			get { return BuildLogMethod ("notice", passthrough, _oldLogNotice); }
		}
		
		public System.Action<string, string> LogWarning { 
			get { return BuildLogMethod ("warning", passthrough, _oldLogWarning); }
		}

		public System.Action<string, string> LogError { 
			get { return BuildLogMethod ("error", passthrough, _oldLogError); }
		}

		public System.Action<string, string> LogCritical { 
			get { return BuildLogMethod ("critical", passthrough, _oldLogCritical); }
		}

		public ServerAddress defaultServer = new ServerAddress () { ip = "127.0.0.1", port = 28777};
		Socket _socket;
		public bool passthrough = true;
		public int sendTimeout = 1000;

		public bool HasSavedServerAddress ()
		{
			return (PlayerPrefs.HasKey (IP_KEY) && PlayerPrefs.HasKey (PORT_KEY) && PlayerPrefs.GetString(IP_KEY)!="" && PlayerPrefs.GetInt(PORT_KEY)!=0);
		}

		public ServerAddress GetServerAddress ()
		{
			if (HasSavedServerAddress())
				return new ServerAddress () { ip = PlayerPrefs.GetString(IP_KEY), port = PlayerPrefs.GetInt(PORT_KEY)};

			DataElement logServerConfig = GameApplication.GetConfiguration ("serverLogger");
			if (logServerConfig != null)
				return new ServerAddress () { ip = logServerConfig["ip"], port = logServerConfig["port"]};

			return defaultServer;

		}

		public void SetServerAddress (string ip, int port)
		{
			PlayerPrefs.SetString (IP_KEY, ip);
			PlayerPrefs.SetInt (PORT_KEY, port);
		}

		IPEndPoint GetRemoteEndPoint ()
		{
			IPAddress remoteIPAddress;
			IPEndPoint remoteEndPoint;

			ServerAddress address = GetServerAddress ();

			remoteIPAddress = IPAddress.Parse (address.ip);
			remoteEndPoint = new IPEndPoint (remoteIPAddress, address.port);

			return remoteEndPoint;
		}

		void Awake ()
		{
			DontDestroyOnLoad (this);

			_oldProvider = CoreLogger.Provider;

			_oldLogTrace = _oldProvider.LogTrace;
			_oldLogInfo = _oldProvider.LogInfo;
			_oldLogDebug = _oldProvider.LogDebug;
			_oldLogNotice = _oldProvider.LogNotice;
			_oldLogWarning = _oldProvider.LogWarning;
			_oldLogError = _oldProvider.LogError;
			_oldLogCritical = _oldProvider.LogCritical;

			CoreLogger.Provider = this;

			Connect ();
		}

		public void Connect ()
		{

			if (_socket != null) {
				_socket.Disconnect (false);
				_socket = null;
			}

			CoreLogger.LogDebug ("ServerLoggerProvider", "creating socket...");
			
			try {
				AddressFamily addressFamily = AddressFamily.InterNetwork;
				SocketType socketType = SocketType.Stream;
				ProtocolType protocolType = ProtocolType.Tcp;
				_socket = new Socket (addressFamily, socketType, protocolType);
				CoreLogger.LogInfo ("ServerLoggerProvider", "socket created");
				_socket.SendTimeout = sendTimeout;
			} catch (Exception ex) {
				CoreLogger.LogWarning ("ServerLoggerProvider", string.Format ("failed to create socket: {0}, stack: {1} - reverting to previous logger", ex.Message, ex.StackTrace));
				_socket = null;
				return;
			}
			
			IPEndPoint remoteEndPoint = GetRemoteEndPoint ();
			
			CoreLogger.LogInfo ("ServerLoggerProvider", string.Format ("server is in {0}", remoteEndPoint));
			
			try {
				_socket.Connect (remoteEndPoint);
			} catch (Exception ex) {
				CoreLogger.LogWarning ("ServerLoggerProvider", "failed to connect to socket: " + ex.Message);
				_socket = null;
				return;
			}

			foreach (string module in CoreLogger.Modules) {
				string msg = string.Format ("+node|{0}|TabTaleLog\r\n", module);
				int sent = _socket.Send (msg.ToByteArray (Encoding.ASCII));
				if (sent != msg.Length) {
					_oldLogWarning ("ServerLoggerProvider", "failed to send registraion of module " + module);
				}
			}

			LogDebug ("ServerLoggerProvider", "provider set");
		}

		public bool IsConnected(){
			if(_socket==null)
				return false;

			return _socket.Connected;
		}

		void OnApplicationQuit ()
		{
			if (_socket != null)
				_socket.Close ();
		}

	}
}