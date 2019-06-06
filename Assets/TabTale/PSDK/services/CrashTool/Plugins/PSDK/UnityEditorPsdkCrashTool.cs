using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TabTale.Plugins.PSDK;
//using Json = TabTale.Plugins.PSDK.PSDKMiniJSON;

namespace TabTale.Plugins.PSDK
{
	public class UnityEditorPsdkCrashTool : IPsdkCrashTool 
    {

		public UnityEditorPsdkCrashTool(IPsdkServiceManager sm) {
		}

		public IPsdkCrashTool GetImplementation() {
			return this;
		}

		public void LogEvent(long targets, string eventName, IDictionary<string,object> eventParams, bool timed){}
		
		public void EndLogEvent(string eventName, IDictionary<string,object> eventParams){
		}

		public void psdkStartedEvent() {
		}

		public void AddBreadCrumb(string crumb) {
			UnityEngine.Debug.Log ("UnityEditorPsdkCrashTool adding bread crumb:" + crumb);
		}
		
		public void ClearAllBreradCrumbs(){
			UnityEngine.Debug.Log ("UnityEditorPsdkCrashTool aClearAllBreradCrumbs !");
		}
	}
}
