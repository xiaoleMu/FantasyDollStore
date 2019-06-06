using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	public class PsdkSingular : IPsdkSingular {

		IPsdkSingular _impl;

		public PsdkSingular(IPsdkServiceManager sm)
		{
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkSingular(); break;
				#if UNITY_ANDROID
				case RuntimePlatform.Android: 		_impl = new AndroidPsdkSingular(sm.GetImplementation()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityPsdkSingular(); break;
			default: throw new System.Exception("Platform not supported for Singular.");
			}
		}

		public void LogEvent(string eventName, IDictionary<string,object> eventParams)
		{
			_impl.LogEvent (eventName, eventParams);
		}

		public void TutorialComplete()
		{
			_impl.TutorialComplete ();
		}

		public void psdkStartedEvent() {

		}

	}
}
