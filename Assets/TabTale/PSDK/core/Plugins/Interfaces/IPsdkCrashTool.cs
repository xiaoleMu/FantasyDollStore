using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {

	public interface IPsdkCrashTool  : IPsdkService {

		void AddBreadCrumb(string crumb); 
	
        void ClearAllBreradCrumbs();

		IPsdkCrashTool GetImplementation();
	}
}
