using System;
using UnityEngine;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK
{
	public class PsdkCrashToolService : IPsdkCrashTool
	{
		IPsdkCrashTool _impl;
		
		public PsdkCrashToolService(IPsdkServiceManager sm)
		{
			switch (Application.platform) {
			case RuntimePlatform.IPhonePlayer: 	_impl = new IphonePsdkCrashTool(sm.GetImplementation()); break;
				#if UNITY_ANDROID
			case RuntimePlatform.Android: 		_impl = new AndroidPsdkCrashTool(sm.GetImplementation()); break;
				#endif
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor: 	_impl = new UnityEditorPsdkCrashTool(sm.GetImplementation()); break;
			default: throw new System.Exception("Platform not supported for CrashTool.");
			}
		}

		
		public void AddBreadCrumb(string crumb) {
			_impl.AddBreadCrumb(crumb);
		}
		
		public void ClearAllBreradCrumbs(){
			_impl.ClearAllBreradCrumbs ();
		}
		
		public IPsdkCrashTool GetImplementation() {
			return _impl;
		}
		
		public void psdkStartedEvent() {
            ClearAllBreradCrumbs();
            AddBreadCrumb(PsdkSerializedData.Instance.ToString());
			_impl.psdkStartedEvent();
		}
	}
}
