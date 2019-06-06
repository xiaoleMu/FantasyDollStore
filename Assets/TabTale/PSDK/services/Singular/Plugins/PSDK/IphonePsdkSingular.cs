using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	public class IphonePsdkSingular : IPsdkSingular {

		[DllImport("__Internal")]
		private static extern void psdkTutorialComplete();

		[DllImport("__Internal")]
		private static extern void psdkSingularLogEvent(string eventName, string eventParamsJson);

		public void LogEvent(string eventName, IDictionary<string,object> eventParams)
		{
			psdkSingularLogEvent(eventName, PsdkUtils.BuildJsonStringFromDict(eventParams));
		}

		public void TutorialComplete()
		{
			psdkTutorialComplete ();
		}

		public void psdkStartedEvent() {

		}
	}
}
