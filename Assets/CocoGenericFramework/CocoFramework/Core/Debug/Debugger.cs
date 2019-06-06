using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace TabTale {

	public class Debugger
	{
		//[Conditional("DEBUG")]
		public static void Assert(bool condition, string log = "")
		{
			if(!condition) 
			{
				CoreLogger.LogError(log);
				throw new UnityException(log);
			}
		}
	}
}