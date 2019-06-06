using UnityEngine;
using System.Collections;

namespace TabTale {

	[System.Serializable]
	public class ServerAddress
	{
		public string ip;
		public int port;
		
		public override string ToString ()
		{
			return string.Format("{0}:{1}", ip, port);
		}
	}
}