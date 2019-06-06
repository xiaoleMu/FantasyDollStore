using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {

	public enum PSDKAudienceMode {
		NON_CHILDREN,
		CHILDREN,
		MIXED_NON_CHILDREN,
		MIXED_CHILDREN,
		MIXED_UNKNOWN
	}

	public interface IPsdkAudience : IPsdkService {
		IPsdkAudience GetImplementation();
		bool Setup ();
		void SetBirthYear (int birthYear);
		int GetAge ();
		PSDKAudienceMode GetAudienceMode ();
	}
}