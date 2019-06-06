using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {
	
	public interface IPsdkSingular : IPsdkService {

		void LogEvent(string eventName, IDictionary<string,object> eventParams);
		void TutorialComplete ();
	}
}