using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale.Plugins.PSDK {

	public interface IPsdkExternalConfiguration  : IPsdkService {

		bool Setup();
		string GetGooglePlayLicenceKey();
		string GetExperimentGroup ();
		IPsdkExternalConfiguration GetImplementation();
	}
}
