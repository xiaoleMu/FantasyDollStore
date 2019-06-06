#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	public class AndroidPsdkSingular : IPsdkSingular {

		private AndroidPsdkServiceMgr _psdkServiceMgr;
		private AndroidJavaObject _javaObject;


		public AndroidPsdkSingular(IPsdkServiceManager serviceMgr) {
			_psdkServiceMgr = serviceMgr as AndroidPsdkServiceMgr;
		}

		public AndroidJavaObject GetUnityJavaObject() {
			try {
				if (null == _javaObject)
					_javaObject = _psdkServiceMgr.GetUnityJavaObject().Call<AndroidJavaObject>("getSingular");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}

			return _javaObject;
		}

		public void LogEvent(string eventName, IDictionary<string,object> eventParams)
		{
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("logEvent",eventName,PsdkUtils.CreateJavaJSONObjectFromDictionary(eventParams));
			else {
				Debug.Log ("Event was not sent: " + eventName + " -> " + Json.Serialize(eventParams));
			}
		}

		public void TutorialComplete()
		{
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("reportTutorialComplete");
			else {
				Debug.Log ("TutorialComplete Event was not sent");
			}
		}

		public void psdkStartedEvent() {
			
		}
	}
}
#endif