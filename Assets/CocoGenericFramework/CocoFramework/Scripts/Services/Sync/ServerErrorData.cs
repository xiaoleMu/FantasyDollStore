using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	[Serializable]
	public struct ServerErrorData 
	{
		public string message;
		public string type;
		public int code;
		public string title;
		public string when;
		public override string ToString() 
		{ 
			return "ServerErrorData: message=" + message + " type=" + type + " code=" + code
				+ " title=" + title  + " when=" + when; 
		}
	}
}
