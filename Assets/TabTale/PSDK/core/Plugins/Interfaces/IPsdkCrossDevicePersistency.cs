using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TabTale.Plugins.PSDK {

	public enum CDPResponse {
		LOCAL, REMOTE, INTERNAL_PSDK
	}

	public enum CDPReason {
		SERVER_CHANGE, ACCOUNT_CHANGE
	}

	public interface IPsdkCrossDevicePersistency : IPsdkService {
		void SetObjectForKey(string obj, string key);
		string ObjectForKey(string key);
		void OnStoreChangeResponse(CDPResponse response);
	}
}