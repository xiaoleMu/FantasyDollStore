#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	public class AndroidPsdkCrashTool : IPsdkCrashTool , IPsdkAndroidService
	{
		
		
		private AndroidPsdkServiceMgr _psdkServiceMgr;
		private AndroidJavaObject _javaObject;
		
		public AndroidPsdkCrashTool(IPsdkServiceManager serviceMgr) {
			_psdkServiceMgr = serviceMgr as AndroidPsdkServiceMgr;
		}
		
		
		public IPsdkCrashTool GetImplementation() {
			return this;
		}
		
		public AndroidJavaObject GetUnityJavaObject() {
			try {
				if (null == _javaObject)
					_javaObject = _psdkServiceMgr.GetUnityJavaObject().Call<AndroidJavaObject>("getCrashMonitoringToolService");
			}
			catch (System.Exception e) {
				Debug.LogException(e);
				return null;
			}
			
			return _javaObject;
		}
		
		public void psdkStartedEvent() {
			GetUnityJavaObject();
		}



		public void AddBreadCrumb(string crumb) {
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("addBreadCrumb",crumb);
			else {
				Debug.LogWarning ("Not calling android psdk CrashTool::addBreadCrumb !, cause object is null");
				Debug.Log ("Crumb was not sent: " + crumb);
			}
		}
		
		public void ClearAllBreradCrumbs(){
			AndroidJavaObject sjo = GetUnityJavaObject();
			if (null != sjo)
				sjo.Call("clearAllBreadCrumbs");
			else {
				Debug.LogWarning ("Not calling android psdk CrashTool::clearAllBreadCrumbs !, cause object is null");
			}
		}
	}
}
#endif
