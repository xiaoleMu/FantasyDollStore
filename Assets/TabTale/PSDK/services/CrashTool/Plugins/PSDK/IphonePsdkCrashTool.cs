using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TabTale.Plugins.PSDK;
using UnityEngine;

namespace TabTale.Plugins.PSDK
{
	internal class IphonePsdkCrashTool : IPsdkCrashTool
	{
		[DllImport("__Internal")]
		private static extern void psdkCrashTool_addBreadCrumb(string crumb);
		
		[DllImport("__Internal")]
		private static extern void psdkCrashTool_clearAllBreadCrumbs();

		
		public IphonePsdkCrashTool(IPsdkServiceManager sm) {
		}
		
		public IPsdkCrashTool GetImplementation() {
			return this;
		}

		public void AddBreadCrumb(string crumb) {
			psdkCrashTool_addBreadCrumb(crumb);
		}
		
		public void ClearAllBreradCrumbs(){
			psdkCrashTool_clearAllBreadCrumbs();
		}

		public void psdkStartedEvent() {
		}
		
	}
}
