using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {

	public static class PsdkLocation {
		public const string sessionStart = "sessionStart";
		public const string moreApps = "moreApps";
		public const string moreAppsInThisSeries = "moreAppsInThisSeries";
		public const string preLevel = "preLevel";
		public const string sceneTransitions = "sceneTransitions";
		public const string xmlMarketingHook1 = "xmlMarketingHook1";
		public const string xmlMarketingHook2 = "xmlMarketingHook2";
		public const string xmlMarketingHook3 = "xmlMarketingHook3";
		public const string xmlMarketingHook4 = "xmlMarketingHook4";
		public const string xmlMarketingHook5 = "xmlMarketingHook5";
		public const string postLevel = "postLevel";
		public const string replayLevel = "replayLevel";
		public const string interstitial = "interstitial";
		public const string retry = "retry";
		public const string fail = "fail";
		public const string achievementWon = "achievementWon";
		public const string backToMainMenu = "backToMainMenu";
		public const string genericLocation = "genericLocation";
		public const string inScene = "inScene";
	}

	public static class LocationMgrAttributes {
		public const long LOCATION_MGR_ATTR_NO_SOURCE 		= 0x00000000L;
		public const long LOCATION_MGR_ATTR_SOURCE_EXIST 	= 0x00000001L;
		public const long LOCATION_MGR_ATTR_PLAYING_MUSIC 	= 0x00000002L;
	}
	
	public interface IPsdkLocationManagerService : IPsdkService  {
		bool Setup();
		long Show(string location);
		long LocationStatus(string location);
		bool IsLocationReady(string location);
		bool IsViewVisible();
		void ReportLocation(string location);
		IPsdkLocationManagerService GetImplementation();
		string GetLocations();
		void LevelOfFirstPopupStatus(bool enabled);
	}
}
