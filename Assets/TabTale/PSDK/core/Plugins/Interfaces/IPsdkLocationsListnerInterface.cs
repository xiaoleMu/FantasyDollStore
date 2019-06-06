using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {

	public interface IPsdkLocationsListnerInterface  {

		void OnLocationLoaded(string location);
	 	void OnLocationFailed(string location, string errMessage);
	 	void OnShown(string location);
	 	void OnClosed(string location);
	 	void OnConfigurationLoaded();
		
		void SetAnalyticsDelegate (IPsdkAnalytics analyticsService);
		
	}

}
